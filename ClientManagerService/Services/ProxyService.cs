using ClientManagerService.Model;
using ClientManagerService.Support;
using System.Linq;
using System.ServiceModel;

namespace ClientManagerService.Services
{
    /// <summary>
    /// Service used to set proxy configuration.
    /// </summary>
    public class ProxyService
    {
        /// <summary>
        /// Sets proxy in <paramref name="binding"/> reference as specified in Helios Green client configuration.
        /// </summary>
        /// <param name="config">Helios Green client configuration.</param>
        /// <param name="binding">HttpBinding reference.</param>
        public void SetProxy(ClientConfig config, BasicHttpBinding binding)
        {
            binding.UseDefaultWebProxy = config.Items.Any(c => c.Section == "LogIn" && c.Key == "UseDefaultProxy" && c.Value == "1");

            var proxy = config.Items.FirstOrDefault(c => c.Section == "LogIn" && c.Key == "Proxy")?.Value ?? string.Empty;
            if (proxy.Contains(".pac"))
            {
                string ip = ProxyPacSupport.GetProxyForUrlUsingPac(config.ApplicationServer, proxy);
                proxy = string.IsNullOrWhiteSpace(ip) ? string.Empty : ip;
            }

            if (!string.IsNullOrWhiteSpace(proxy))
            {
                binding.ProxyAddress = new System.Uri(proxy);
            }
        }
    }
}
