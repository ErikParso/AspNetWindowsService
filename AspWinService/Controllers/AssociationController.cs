using AspWinService.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Win32;
using Newtonsoft.Json;
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
            var clientsInfoString = System.IO.File.ReadAllText(Constants.installedClientsFile);
            var clientsInfo = JsonConvert.DeserializeObject<IEnumerable<ClientInstallationInfo>>(clientsInfoString).ToList();
            var clientInfo = clientsInfo.First(c => c.InstallDir == model.InstallDir);
            var servicePath = new FileInfo(Assembly.GetEntryAssembly().Location).DirectoryName;
            var ngClientPath = Path.Combine(servicePath, "..", "NgClient", "asp-win-service-ng-client.exe");
            CreateFileAssociation(model.Extension, $"HgFile", $"Helios green {clientInfo.ClientName} file", ngClientPath);

            if (clientInfo.Extensions == null)
                clientInfo.Extensions = new List<string>();
            clientInfo.Extensions = clientInfo.Extensions.Union(new[] { model.Extension });
            System.IO.File.WriteAllText(Constants.installedClientsFile, JsonConvert.SerializeObject(clientsInfo));

            return Ok();
        }

        /// <summary>
        /// Create a new file association
        /// </summary>
        /// <param name="extension">Extension of the file type, including the seperator) (ie: ".torrent")</param>
        /// <param name="key">File Type Key (can be referenced to create multiple extensions for one file type)</param>
        /// <param name="description">Description for the file type</param>
        /// <param name="path">Path (ie. '"C:\Executable.exe" "%1"')</param>
        private void CreateFileAssociation(string extension, string key, string description, string path)
        {
            RegistryKey classes = Registry.ClassesRoot;
            RegistryKey extensionKey = classes.CreateSubKey(extension);
            extensionKey.SetValue(null, key);

            RegistryKey typeKey = classes.CreateSubKey(key);
            typeKey.SetValue(null, description);

            RegistryKey shellKey = typeKey.CreateSubKey("shell");
            RegistryKey shellOpenKey = shellKey.CreateSubKey("open");
            RegistryKey shellOpenCommandKey = shellOpenKey.CreateSubKey("command");
            shellOpenCommandKey.SetValue(null, path);
        }

        private  bool DoesFileAssociationExists(string extension)
        {
            RegistryKey classes = Registry.ClassesRoot;
            RegistryKey extensionKey = classes.OpenSubKey(extension);
            return (extensionKey != null);
        }
    }
}