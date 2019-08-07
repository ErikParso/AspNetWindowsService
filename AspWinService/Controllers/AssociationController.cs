using AspWinService.Model;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace AspWinService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AssociationController : ControllerBase
    {
        [HttpPost]
        public IActionResult AddFileAssociation([FromBody]AssociationRequest model)
        {
            var clientsInfoString = System.IO.File.ReadAllText(Constants.InstalledClientsFileName);
            var clientsInfo = JsonConvert.DeserializeObject<IEnumerable<ClientInstallationInfo>>(clientsInfoString).ToList();
            var clientInfo = clientsInfo.First(c => c.InstallDir == model.InstallDir);
            var servicePath = new FileInfo(Assembly.GetEntryAssembly().Location).DirectoryName;
            var ngClientPath = Path.Combine(servicePath, "..", "NgClient", Constants.ServiceAppFileName);

            FileAssociations.SetAssociation(model.Extension, $"HG_{clientInfo.ClientName}", $"Helios Green {clientInfo.ClientName} file.", ngClientPath);

            if (clientInfo.Extensions == null)
                clientInfo.Extensions = new List<string>();
            clientInfo.Extensions = clientInfo.Extensions.Union(new[] { model.Extension });
            System.IO.File.WriteAllText(Constants.InstalledClientsFileName, JsonConvert.SerializeObject(clientsInfo));

            return Ok();
        }

        [HttpPost("runClient")]
        public IActionResult RunClient([FromBody]RunAssociatedClientRequest model)
        {
            var extension = Path.GetExtension(model.FilePath);
            var clientsInfoString = System.IO.File.ReadAllText(Constants.InstalledClientsFileName);
            var clientsInfo = JsonConvert.DeserializeObject<IEnumerable<ClientInstallationInfo>>(clientsInfoString).ToList();
            var clientInfo = clientsInfo.FirstOrDefault(c => c.Extensions?.Contains(extension) ?? false);

            if (clientInfo != null)
            {
                ProcessExtensions.StartProcessAsCurrentUser(Path.Combine(clientInfo.InstallDir, "HeliosGreenClient.exe"), model.FilePath, clientInfo.InstallDir);
                return Ok();
            }
            else
            {
                return BadRequest($"There is no client associated to files with extension {extension}.");
            }
        }

    }
}