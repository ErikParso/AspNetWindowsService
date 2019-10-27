using ClientManagerService.Extensions;
using ClientManagerService.Model;
using System.Collections.Generic;

namespace ClientManagerService.Services
{
    /// <summary>
    /// Service used to work with ClientName and ClientAuthor configuration items to determine client executable file name.
    /// </summary>
    public class ClientNameService
    {
        private readonly Dictionary<string, string> clientExecutables = new Dictionary<string, string>()
        {
            { "win32", "NorisWin32.exe" },
            { "clientwinforms", "ASOL.Green.Client.WinForms.exe" }
        };

        /// <summary>
        /// Resolves client executable name from ClientName configuration item.
        /// </summary>
        /// <param name="config">Client configuration.</param>
        /// <returns>Client executable file name.</returns>
        public string GetClientExecutableName(ClientConfig config)
        {
            var clientNameSetting = config.Items.GetValueString("Client", "ClientName");
            return clientExecutables.TryGetValue(clientNameSetting?.ToLower() ?? string.Empty, out string clientExeName)
                ? clientExeName
                : clientExecutables["win32"];
        }

        /// <summary>
        /// Checks if provided value is valid value for "ClientName" config option.
        /// </summary>
        /// <param name="clientName">ClientName config option value.</param>
        /// <returns>If ClientName value is valid.</returns>
        public bool IsSupportedClientName(string clientName)
            => !string.IsNullOrWhiteSpace(clientName) && clientExecutables.ContainsKey(clientName.ToLower());
    }
}
