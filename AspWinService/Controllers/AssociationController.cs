using AspWinService.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Win32;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;

namespace AspWinService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AssociationController : ControllerBase
    {
        private const string exeName = "asp-win-service-ng-client.exe";

        [HttpPost]
        public IActionResult AddFileAssociation([FromBody]AssociationRequest model)
        {
            var clientsInfoString = System.IO.File.ReadAllText(Constants.installedClientsFile);
            var clientsInfo = JsonConvert.DeserializeObject<IEnumerable<ClientInstallationInfo>>(clientsInfoString).ToList();
            var clientInfo = clientsInfo.First(c => c.InstallDir == model.InstallDir);
            var servicePath = new FileInfo(Assembly.GetEntryAssembly().Location).DirectoryName;
            var ngClientPath = Path.Combine(servicePath, "..", "NgClient", exeName);

            FileAssociations.SetAssociation(model.Extension, $"HG_{clientInfo.ClientName}", $"Helios Green {clientInfo.ClientName} file.", ngClientPath);

            if (clientInfo.Extensions == null)
                clientInfo.Extensions = new List<string>();
            clientInfo.Extensions = clientInfo.Extensions.Union(new[] { model.Extension });
            System.IO.File.WriteAllText(Constants.installedClientsFile, JsonConvert.SerializeObject(clientsInfo));

            return Ok();
        }

    }
}