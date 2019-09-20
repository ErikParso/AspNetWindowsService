using AspWinService.Model;
using AspWinService.Tools;
using System;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Channels;

namespace AspWinService.Services
{
    public class ProxyService
    {
        public void  SetProxy(ClientConfig config, BasicHttpBinding binding)
        {
            if (config.Items.Any(c => c.Section == "LogIn" && c.Key == "UseDefaultProxy" && c.Value == "1"))
            {
                binding.UseDefaultWebProxy = true;
            }

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
