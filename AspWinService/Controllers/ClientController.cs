using AspWinService.Model;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
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
            var clientsInfoString = System.IO.File.ReadAllText(Constants.InstalledClientsFileName);
            var clientsInfo = JsonConvert.DeserializeObject<IEnumerable<ClientInfo>>(clientsInfoString);
            return Ok(clientsInfo);
        }

        [HttpGet("latestVersion")]
        public async Task<IActionResult> GetLatestVersion()
        {
            return Ok(await GetLatestVersionCore());
        }

        [HttpPost("{clientName}")]
        public IActionResult RunClient(string clientName)
        {
            var clientsInfoString = System.IO.File.ReadAllText(Constants.InstalledClientsFileName);
            var clientsInfo = JsonConvert.DeserializeObject<IEnumerable<ClientInfo>>(clientsInfoString);
            var installDir = clientsInfo.Where(c => c.ClientName == clientName).First().InstallDir;
            ProcessExtensions.StartProcessAsCurrentUser(Path.Combine(installDir, "HeliosGreenClient.exe"), null, installDir);
            return Ok();
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
            var clientsInfoString = System.IO.File.ReadAllText(Constants.InstalledClientsFileName);
            var clientsInfo = JsonConvert.DeserializeObject<List<ClientInfo>>(clientsInfoString).ToList();
            var version = JsonConvert.DeserializeObject<dynamic>(System.IO.File.ReadAllText(Path.Combine(installDir, "VersionInfo.json"))).version;
            var clientInfo = new ClientInfo()
            {
                ClientName = model.ClientName,
                InstallDir = installDir,
                Version = version
            };
            clientsInfo.Add(clientInfo);
            System.IO.File.WriteAllText(Constants.InstalledClientsFileName, JsonConvert.SerializeObject(clientsInfo));

            return Ok(clientInfo);
        }

        [HttpPut]
        public async Task<IActionResult> UpdateClient(ClientUpdateRequest model)
        {
            var installDir = model.InstallDir;

            if (Directory.Exists(installDir))
                Directory.Delete(installDir, true);

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
            var clientsInfoString = System.IO.File.ReadAllText(Constants.InstalledClientsFileName);
            var clientsInfo = JsonConvert.DeserializeObject<IEnumerable<ClientInfo>>(clientsInfoString).ToList();
            var clientInfo = clientsInfo.Where(c => c.InstallDir == installDir).First();
            clientInfo.Version = JsonConvert.DeserializeObject<dynamic>(System.IO.File.ReadAllText(Path.Combine(installDir, "VersionInfo.json"))).version;
            clientsInfo.RemoveAll(c => c.InstallDir == installDir);
            clientsInfo.Add(clientInfo);
            System.IO.File.WriteAllText(Constants.InstalledClientsFileName, JsonConvert.SerializeObject(clientsInfo));

            return Ok(clientInfo);
        }

        [HttpDelete()]
        public IActionResult DeleteClient([FromBody]ClientDeleteRequest model)
        {
            var clientsInfoString = System.IO.File.ReadAllText(Constants.InstalledClientsFileName);
            var clientsInfo = JsonConvert.DeserializeObject<IEnumerable<ClientInfo>>(clientsInfoString).ToList();
            var clientInfo = clientsInfo.Where(c => c.ClientName == model.ClientName).First();

            if (Directory.Exists(clientInfo.InstallDir))
                Directory.Delete(clientInfo.InstallDir, true);
            clientsInfo.Remove(clientInfo);
            System.IO.File.WriteAllText(Constants.InstalledClientsFileName, JsonConvert.SerializeObject(clientsInfo));

            return Ok(clientInfo);
        }

        private async Task<string> GetLatestVersionCore()
        {
            using (var httpClient = new HttpClient())
                return await httpClient.GetStringAsync("http://localhost:5100/api/client");
        }
    }
}
