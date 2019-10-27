using Newtonsoft.Json;
using System.Collections.Generic;

namespace ClientManagerService.Model
{
    /// <summary>
    /// Helios Green client installation info.
    /// </summary>
    public class ClientInfo
    {
        /// <summary>
        /// Installed client id.
        /// </summary>
        public string ClientId { get; set; }

        /// <summary>
        /// Helios Green client name.
        /// </summary>
        public string ClientName { get; set; }

        /// <summary>
        /// Installation directory.
        /// </summary>
        public string InstallDir { get; set; }

        /// <summary>
        /// Helios Green client installation owner 
        /// or empty if pre machine installation scope specified.
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// Associated files extensions.
        /// </summary>
        public IEnumerable<string> Extensions { get; set; }

        /// <summary>
        /// Helios Green Client configuration (replacement for client.config).
        /// </summary>
        public ClientConfig Config { get; set; }
    }
}
