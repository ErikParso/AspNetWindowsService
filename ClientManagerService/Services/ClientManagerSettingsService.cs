using ClientManagerService.Model;
using Newtonsoft.Json;
using System.IO;

namespace ClientManagerService.Services
{
    /// <summary>
    /// Parses <see cref="Constants.ClientManagerSettingsFileName"/> into <see cref="ClientManagerSettings"/>.
    /// </summary>
    public class ClientManagerSettingsService
    {
        /// <summary>
        /// Parses <see cref="Constants.ClientManagerSettingsFileName"/> into <see cref="ClientManagerSettings"/>.
        /// </summary>
        /// <returns><see cref="ClientManagerSettings"/></returns>
        public ClientManagerSettings GetClientManagerSettings()
        {
            ClientManagerSettings result = new ClientManagerSettings();
            if (!File.Exists(Constants.ClientManagerSettingsFileName))
            {
                var content = JsonConvert.SerializeObject(new ClientManagerSettings());
                File.WriteAllText(Constants.ClientManagerSettingsFileName, content);
            }
            else
            {
                var content = File.ReadAllText(Constants.ClientManagerSettingsFileName);
                result = JsonConvert.DeserializeObject<ClientManagerSettings>(content);
            }
            return result;
        }
    }
}
