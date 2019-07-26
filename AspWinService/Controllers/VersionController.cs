using Microsoft.AspNetCore.Mvc;
using System;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;

namespace AspWinService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VersionController : ControllerBase
    {
        private const string version = "1.0.1";

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var latest = await GetLatestVersion();
            return Ok(new { Local = version, Latest = latest, ServiceDir = new FileInfo(Assembly.GetEntryAssembly().Location).Directory.ToString() });
        }

        [HttpPost]
        public async Task<IActionResult> CheckVersionsAndUpgrade()
        {
            try
            {
                await CheckVersionsAndUpgradeAsync();
            }
            catch (Exception e)
            {
                return Ok(e);
            }
            return Ok();
        }

        private async Task<string> GetLatestVersion()
        {
            using (var httpClient = new HttpClient())
                return await httpClient.GetStringAsync("http://localhost:5100/api/version");
        }

        private async Task CheckVersionsAndUpgradeAsync()
        {
            var latest = await GetLatestVersion();

            if (latest != version)
            {
                using (var httpClient = new HttpClient())
                {
                    using (var request = new HttpRequestMessage(HttpMethod.Get, $"http://localhost:5100/api/version/{latest}"))
                    {
                        if (!Directory.Exists("Installer"))
                            Directory.CreateDirectory("Installer");
                        using (
                            Stream contentStream = await (await httpClient.SendAsync(request)).Content.ReadAsStreamAsync(), 
                            stream = new FileStream("Installer/MyProduct.msi", FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite))
                        {
                            await contentStream.CopyToAsync(stream);
                        }
                    }
                }
                var p = new Process();
                string myExeDir = new FileInfo(Assembly.GetEntryAssembly().Location).Directory.ToString();
                p.StartInfo = new ProcessStartInfo($@"{myExeDir.TrimEnd('/')}/Installer/MyProduct.msi")
                {
                    UseShellExecute = true
                };
                p.Start();
            }
        }
    }
}
