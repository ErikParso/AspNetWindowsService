using AspWinService.Model;
using System.ServiceModel;
using System.Threading.Tasks;
using WSData;
using WSUpdate;

namespace AspWinService.Services
{
    public class RedirectService
    {
        private readonly ProxyService proxyService;
        private readonly CredentialService credentialService;

        public RedirectService(
            ProxyService proxyService,
            CredentialService credentialService)
        {
            this.proxyService = proxyService;
            this.credentialService = credentialService;
        }

        public async Task<string> GetApplicationServerAddress(ClientConfig clientConfig)
        {
            var dataClient = new DataSoapClient(DataSoapClient.EndpointConfiguration.DataSoap);
            dataClient.Endpoint.Address = new EndpointAddress(clientConfig.ApplicationServer + "/Data.asmx");
            proxyService.SetProxy(clientConfig, dataClient.Endpoint.Binding as BasicHttpBinding);
            credentialService.SetWinCredentials(clientConfig, dataClient.ClientCredentials.Windows);
            return (await dataClient.GetInfoAsync("GETREDIRECTINFO", string.Empty)).Body.GetInfoResult;
        }

        public async Task<string> GetAvailableLanguages(ClientConfig clientConfig)
        {
            var appServeraddress = await GetApplicationServerAddress(clientConfig);
            var dataClient = new DataSoapClient(DataSoapClient.EndpointConfiguration.DataSoap);
            dataClient.Endpoint.Address = new System.ServiceModel.EndpointAddress(appServeraddress + "/Data.asmx");
            proxyService.SetProxy(clientConfig, dataClient.Endpoint.Binding as BasicHttpBinding);
            credentialService.SetWinCredentials(clientConfig, dataClient.ClientCredentials.Windows);
            return (await dataClient.GetInfoAsync("GETLANGUAGES", string.Empty)).Body.GetInfoResult;
        }

        public async Task<ClientUpdateSoapClient> GetUpdateClient(ClientConfig clientConfig)
        {
            var appServeraddress = await GetApplicationServerAddress(clientConfig);
            var updateClient = new ClientUpdateSoapClient(ClientUpdateSoapClient.EndpointConfiguration.ClientUpdateSoap);
            updateClient.Endpoint.Address = new System.ServiceModel.EndpointAddress(appServeraddress + "/ClientUpdate.asmx");
            proxyService.SetProxy(clientConfig, updateClient.Endpoint.Binding as BasicHttpBinding);
            credentialService.SetWinCredentials(clientConfig, updateClient.ClientCredentials.Windows);
            return updateClient;
        }
    }
}
