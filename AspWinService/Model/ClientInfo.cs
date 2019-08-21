using System.Collections.Generic;

namespace AspWinService.Model
{
    public class ClientInfo
    {
        public string ClientId { get; set; }

        public string ClientName { get; set; }

        public string InstallDir { get; set; }

        public string ApplicationServer { get; set; }

        public IEnumerable<string> Extensions { get; set; }
    }
}
