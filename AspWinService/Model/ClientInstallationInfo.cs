using System.Collections.Generic;

namespace AspWinService.Model
{
    public class ClientInstallationInfo
    {
        public string ClientName { get; set; }

        public string InstallDir { get; set; }

        public string Version { get; set; }

        public IEnumerable<string> Extensions { get; set; }
    }
}
