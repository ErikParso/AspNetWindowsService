using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;

namespace AspWinService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InstallerController : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> GetInfo()
        {
            return Ok(new {
                Local = GetCurrentVersion(),
                Latest = await GetLatestVersion()
            });
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
                return StatusCode(StatusCodes.Status500InternalServerError, e);
            }
            return Ok("Installation started.");
        }

        private async Task CheckVersionsAndUpgradeAsync()
        {
            var latest = await GetLatestVersion();
            var current = GetCurrentVersion();

            if (latest != current)
            {
                using (var httpClient = new HttpClient())
                {
                    using (var request = new HttpRequestMessage(HttpMethod.Get, $"http://localhost:5100/api/installer/download"))
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

                string myExeDir = new FileInfo(Assembly.GetEntryAssembly().Location).Directory.ToString();
                string msiPath = Path.Combine(myExeDir, @"Installer\MyProduct.msi");
                //TODO: not working 
                ProcessExtensions.StartProcessAsCurrentUser(msiPath);
            }
        }

        private async Task<string> GetLatestVersion()
        {
            using (var httpClient = new HttpClient())
                return await httpClient.GetStringAsync("http://localhost:5100/api/installer");
        }

        private string GetCurrentVersion()
            => JsonConvert.DeserializeObject<dynamic>(System.IO.File.ReadAllText("VersionInfo.json")).version;
    }
}
