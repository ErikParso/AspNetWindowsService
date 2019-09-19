using AspWinService.Model;
using System.Threading.Tasks;
using WSData;
using WSUpdate;

namespace AspWinService.Services
{
    public class RedirectService
    {
        private readonly ProxyService proxyService;

        public RedirectService(ProxyService proxyService)
        {
            this.proxyService = proxyService;
        }

        public async Task<string> GetApplicationServerAddress(ClientConfig clientConfig)
        {
            var dataClient = new DataSoapClient(DataSoapClient.EndpointConfiguration.DataSoap);
            dataClient.Endpoint.Address = new System.ServiceModel.EndpointAddress(clientConfig.ApplicationServer + "/Data.asmx");
            dataClient.Endpoint.Binding = proxyService.GetBinding(clientConfig);
            return (await dataClient.GetInfoAsync("GETREDIRECTINFO", string.Empty)).Body.GetInfoResult;
        }

        public async Task<string> GetAvailableLanguages(ClientConfig clientConfig)
        {
            var appServeraddress = await GetApplicationServerAddress(clientConfig);
            var dataClient = new DataSoapClient(DataSoapClient.EndpointConfiguration.DataSoap);
            dataClient.Endpoint.Address = new System.ServiceModel.EndpointAddress(appServeraddress + "/Data.asmx");
            dataClient.Endpoint.Binding = proxyService.GetBinding(clientConfig);
            return (await dataClient.GetInfoAsync("GETLANGUAGES", string.Empty)).Body.GetInfoResult;
        }

        public async Task<ClientUpdateSoapClient> GetUpdateClient(ClientConfig clientConfig)
        {
            var appServeraddress = await GetApplicationServerAddress(clientConfig);
            var updateClient = new ClientUpdateSoapClient(ClientUpdateSoapClient.EndpointConfiguration.ClientUpdateSoap);
            updateClient.Endpoint.Address = new System.ServiceModel.EndpointAddress(appServeraddress + "/ClientUpdate.asmx");
            updateClient.Endpoint.Binding = proxyService.GetBinding(clientConfig);
            return updateClient;
        }
    }
}
