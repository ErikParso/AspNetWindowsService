using ClientManagerService.Model;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace ClientManagerService.Services
{
    /// <summary>
    /// Service used to update client configuration using <see cref="Constants.ClientConfigUpdateXml"/> file.
    /// </summary>
    public class ClientConfigUpdateService
    {
        /// <summary>
        /// Modifies client config values using data in <see cref="Constants.ClientConfigUpdateXml"/> file.
        /// </summary>
        /// <param name="clientInfo">Client info to be modified.</param>
        public void UpdateClientConfig(ClientInfo clientInfo)
        {
            var clientConfigUpdateFileName = Path.Combine(clientInfo.InstallDir, Constants.ClientConfigUpdateXml);
            if (File.Exists(clientConfigUpdateFileName))
            {
                var configItems = clientInfo.Config.Items.ToList();
                XDocument xdoc = XDocument.Load(clientConfigUpdateFileName);
                var configSetItems = xdoc.Root.Elements();
                foreach (var setResetItem in configSetItems)
                {
                    var section = setResetItem.Attribute("section").Value;
                    var key = setResetItem.Attribute("key").Value;
                    if (setResetItem.Name == "Set")
                    {
                        configItems.RemoveAll(i => i.Section == section && i.Key == key);
                        configItems.Add(new ClientConfigItem
                        {
                            Section = section,
                            Key = key,
                            Value = setResetItem.Attribute("value").Value
                        });
                    }
                    else if (setResetItem.Name == "Reset")
                    {
                        configItems.RemoveAll(i => i.Section == section && i.Key == key);
                    }
                }
                clientInfo.Config.Items = configItems;
            }
        }
    }
}
