﻿using AspWinService.Model;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Net.Http;
using System.Threading.Tasks;

namespace AspWinService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClientController : ControllerBase
    {
        [HttpGet]
        public IActionResult GetClients()
        {
            var clientsInfoString = System.IO.File.ReadAllText(Constants.installedClientsFile);
            var clientsInfo = JsonConvert.DeserializeObject<IEnumerable<ClientInstallationInfo>>(clientsInfoString);
            return Ok(clientsInfo);
        }

        [HttpGet("latestVersion")]
        public async Task<IActionResult> GetLatestVersion()
        {
            return Ok(await GetLatestVersionCore());
        }

        [HttpPost]
        public async Task<IActionResult> InstallNewClient([FromBody]ClientInstallationRequest model)
        {
            var installDir = Path.Combine(model.InstallDir, model.ClientName);

            if (!Directory.Exists(installDir))
                Directory.CreateDirectory(installDir);

            var zipPath = Path.Combine(installDir, "HeliosGreen.zip");

            // Download zipped client.
            using (var httpClient = new HttpClient())
            {
                using (var request = new HttpRequestMessage(HttpMethod.Get, $"http://localhost:5100/api/client/download"))
                {
                    using (
                        Stream contentStream = await (await httpClient.SendAsync(request)).Content.ReadAsStreamAsync(),
                        stream = new FileStream(zipPath, FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite))
                    {
                        await contentStream.CopyToAsync(stream);
                    }
                }
            }

            // Unzip client and delete zip.
            ZipFile.ExtractToDirectory(zipPath, installDir);
            System.IO.File.Delete(zipPath);

            // Save client installation information.
            var clientsInfoString = System.IO.File.ReadAllText(Constants.installedClientsFile);
            var clientsInfo = JsonConvert.DeserializeObject<List<ClientInstallationInfo>>(clientsInfoString);
            var version = JsonConvert.DeserializeObject<dynamic>(System.IO.File.ReadAllText(Path.Combine(installDir, "VersionInfo.json"))).version;  
            clientsInfo.Add(new ClientInstallationInfo()
            {
                ClientName = model.ClientName,
                InstallDir = installDir,
                Version = version
            });
            System.IO.File.WriteAllText(Constants.installedClientsFile, JsonConvert.SerializeObject(clientsInfo));

            return Ok();
        }

        private async Task<string> GetLatestVersionCore()
        {
            using (var httpClient = new HttpClient())
                return await httpClient.GetStringAsync("http://localhost:5100/api/client");
        }
    }
}