using ClientManagerService.Model;
using System.Collections.Generic;
using System.Linq;

namespace ClientManagerService.Extensions
{
    /// <summary>
    /// Client config extensions.
    /// </summary>
    public static class ConfigExtensions
    {
        /// <summary>
        /// Gets config value as boolean.
        /// </summary>
        /// <param name="items">Config items collection.</param>
        /// <param name="section">Config section.</param>
        /// <param name="key">Config key</param>
        /// <returns>
        /// Boolean value of config item. False if config item not set.
        /// </returns>
        public static bool GetValueBool(this IEnumerable<ClientConfigItem> items, string section, string key)
            => items.Any(i =>
                i.Key.ToLower() == key.ToLower() &&
                i.Section.ToLower() == section.ToLower() &&
                (i.Value == "1" || i.Value.ToLower() == "true"));

        /// <summary>
        /// Gets config value as string.
        /// </summary>
        /// <param name="items">Config items collection.</param>
        /// <param name="section">Config section.</param>
        /// <param name="key">Config key</param>
        /// <returns>
        /// String value of config item. Null if config item not set.
        /// </returns>
        public static string GetValueString(this IEnumerable<ClientConfigItem> items, string section, string key)
            => items.FirstOrDefault(i => 
                i.Key.ToLower() == key.ToLower() && 
                i.Section.ToLower() == section.ToLower())?.Value;
    }
}
