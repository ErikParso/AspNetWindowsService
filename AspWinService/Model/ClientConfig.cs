using System.Collections.Generic;

namespace AspWinService.Model
{
    public class ClientConfig
    {
        public string ApplicationServer { get; set; }

        public string Language { get; set; }

        public IEnumerable<ClientConfigItem> Items { get; set;  }
    }

    public class ClientConfigItem
    {
        public string Section { get; set; }

        public string Key { get; set; }

        public string Value { get; set; }
    }
}
