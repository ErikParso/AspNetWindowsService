using AspWinService.Model;
using AspWinService.Tools;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Channels;

namespace AspWinService.Services
{
    public class ProxyService
    {
        internal Binding GetBinding(ClientConfig config)
        {
            var result = new BasicHttpBinding();

            if (config.Items.Any(c => c.Section == "LogIn" && c.Key == "UseDefaultProxy" && c.Value == "1"))
            {
                result.UseDefaultWebProxy = true;
            }

            var proxy = config.Items.FirstOrDefault(c => c.Section == "LogIn" && c.Key == "Proxy")?.Value ?? string.Empty;
            if (proxy.Contains(".pac"))
            {
                string ip = ProxyPacSupport.GetProxyForUrlUsingPac(config.ApplicationServer, proxy);
                proxy = string.IsNullOrWhiteSpace(ip) ? string.Empty : ip;
            }

            if (!string.IsNullOrWhiteSpace(proxy))
            {
                result.ProxyAddress = new System.Uri(proxy);
            }

            return result;
        }


    }
}
