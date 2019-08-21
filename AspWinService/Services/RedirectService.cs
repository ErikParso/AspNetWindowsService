using System.Threading.Tasks;
using WSData;
using WSUpdate;

namespace AspWinService.Services
{
    public class RedirectService
    {
        public async Task<string> GetApplicationServerAddress(string versionManagerAddress)
        {
            var dataClient = new DataSoapClient(DataSoapClient.EndpointConfiguration.DataSoap);
            dataClient.Endpoint.Address = new System.ServiceModel.EndpointAddress(versionManagerAddress + "/Data.asmx");
            return (await dataClient.GetInfoAsync("GETREDIRECTINFO", string.Empty)).Body.GetInfoResult;
        }

        public async Task<ClientUpdateSoapClient> GetUpdateClient(string versionManagerAddress)
        {
            var appServeraddress = await GetApplicationServerAddress(versionManagerAddress);
            var updateClient = new ClientUpdateSoapClient(ClientUpdateSoapClient.EndpointConfiguration.ClientUpdateSoap);
            updateClient.Endpoint.Address = new System.ServiceModel.EndpointAddress(appServeraddress + "/ClientUpdate.asmx");
            return updateClient;
        }
    }
}
