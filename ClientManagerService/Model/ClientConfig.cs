using System.Collections.Generic;

namespace ClientManagerService.Model
{
    /// <summary>
    /// Replacement for Client.config file.
    /// </summary>
    public class ClientConfig
    {
        /// <summary>
        /// Version Manager address.
        /// </summary>
        public string ApplicationServer { get; set; }

        /// <summary>
        /// Language.
        /// </summary>
        public string Language { get; set; }

        /// <summary>
        /// Config file name. Default noris.config.
        /// </summary>
        public string ConfigFileName { get; set; }

        /// <summary>
        /// Client configuration items collection.
        /// </summary>
        public IEnumerable<ClientConfigItem> Items { get; set;  }
    }

    /// <summary>
    /// Client configuration item.
    /// </summary>
    public class ClientConfigItem
    {
        /// <summary>
        /// Config section.
        /// </summary>
        public string Section { get; set; }

        /// <summary>
        /// Config key.
        /// </summary>
        public string Key { get; set; }

        /// <summary>
        /// Config value.
        /// </summary>
        public string Value { get; set; }
    }
}
