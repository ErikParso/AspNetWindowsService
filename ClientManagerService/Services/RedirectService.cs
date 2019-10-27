using ClientManagerService.Exceptions;
using ClientManagerService.Model;
using Microsoft.Extensions.Logging;
using System;
using System.ServiceModel;
using System.Threading.Tasks;
using TestServiceReference;
using WSDataSSL;
using WSUpdate;

namespace ClientManagerService.Services
{
    /// <summary>
    /// Service used to work with Version Manager server to access Applciation Server and build Application Server Soap Client.
    /// </summary>
    public class RedirectService
    {
        private readonly ProxyService proxyService;
        private readonly CredentialService credentialService;
        private readonly CertificateValidationService certificateValidationService;
        private readonly ILogger<RedirectService> logger;

        /// <summary>
        /// Initializes RedirectService
        /// </summary>
        /// <param name="proxyService">Proxy service.</param>
        /// <param name="credentialService">Windows credentials service.</param>
        public RedirectService(
            ProxyService proxyService,
            CredentialService credentialService,
            CertificateValidationService certificateValidationService,
            ILogger<RedirectService> logger)
        {
            this.proxyService = proxyService;
            this.credentialService = credentialService;
            this.certificateValidationService = certificateValidationService;
            this.logger = logger;
        }

        /// <summary>
        /// Gets application server address from Version Manager server.
        /// Uses proxy and credentials settings in client config to connect Client Redirect server.
        /// </summary>
        /// <param name="clientConfig">Helios Green client configuration.</param>
        /// <returns>Client redirect address (Application server)</returns>
        public async Task<string> GetClientRedirectAddress(ClientConfig clientConfig)
        {
            if (clientConfig.ApplicationServer.StartsWith("https://localhost:5035")) //TODO: Remove and leave only else node
            {
                var testClient = new Service1Client(Service1Client.EndpointConfiguration.BasicHttpsBinding_IService1);
                testClient.Endpoint.Address = new EndpointAddress(clientConfig.ApplicationServer + "/Service1.svc");

                proxyService.SetProxy(clientConfig, testClient.Endpoint.Binding as BasicHttpBinding);
                await credentialService.SetWinCredentials(clientConfig, testClient.ClientCredentials.Windows);
                if (clientConfig.ApplicationServer.ToLower().StartsWith("https://"))
                {
                    (testClient.Endpoint.Binding as BasicHttpBinding).Security.Mode = BasicHttpSecurityMode.Transport;
                    certificateValidationService.SetCertificateValidation(testClient.ClientCredentials);
                }
                return await testClient.GetInfoAsync();
            }
            else
            {
                var dataClient = new DataSoapClient(DataSoapClient.EndpointConfiguration.DataSoap);
                dataClient.Endpoint.Address = new EndpointAddress(clientConfig.ApplicationServer + "/Data.asmx");

                proxyService.SetProxy(clientConfig, dataClient.Endpoint.Binding as BasicHttpBinding);
                await credentialService.SetWinCredentials(clientConfig, dataClient.ClientCredentials.Windows);
                if (clientConfig.ApplicationServer.ToLower().StartsWith("https://"))
                {
                    (dataClient.Endpoint.Binding as BasicHttpBinding).Security.Mode = BasicHttpSecurityMode.Transport;
                    certificateValidationService.SetCertificateValidation(dataClient.ClientCredentials);
                }
                return (await dataClient.GetInfoAsync("GETREDIRECTINFO", string.Empty)).Body.GetInfoResult;
            }   
        }

        /// <summary>
        /// Gets Application Server address from Version Manager server. Creates soap client to communicate with Application Server Update service.
        /// Uses proxy and credentials settings in client config to connect Client Redirect server and create Application Server update soap client.
        /// </summary>
        /// <param name="clientConfig">Helios Green client configuration.</param>
        /// <returns>Application Server soap client.</returns>
        public async Task<ClientUpdateSoapClient> GetUpdateClient(ClientConfig clientConfig)
        {
            var appServeraddress = await GetClientRedirectAddress(clientConfig);
            var updateClient = new ClientUpdateSoapClient(ClientUpdateSoapClient.EndpointConfiguration.ClientUpdateSoap);
            updateClient.Endpoint.Address = new EndpointAddress(appServeraddress + "/ClientUpdate.asmx");
            proxyService.SetProxy(clientConfig, updateClient.Endpoint.Binding as BasicHttpBinding);
            await credentialService.SetWinCredentials(clientConfig, updateClient.ClientCredentials.Windows);
            if (appServeraddress.ToLower().StartsWith("https://"))
            {
                (updateClient.Endpoint.Binding as BasicHttpBinding).Security.Mode = BasicHttpSecurityMode.Transport;
                certificateValidationService.SetCertificateValidation(updateClient.ClientCredentials);
            }
            return updateClient;
        }

        /// <summary>
        /// Gets Application Server address from Version Manager server. Creates soap client to communicate with Application Server Data service.
        /// Uses proxy and credentials settings in client config to connect Client Redirect server and create Application Server data soap client.
        /// </summary>
        /// <param name="clientConfig"></param>
        /// <returns></returns>
        public async Task<DataSoapClient> GetDataClient(ClientConfig clientConfig)
        {
            var appServeraddress = await GetClientRedirectAddress(clientConfig);
            var dataClient = new DataSoapClient(DataSoapClient.EndpointConfiguration.DataSoap);
            dataClient.Endpoint.Address = new EndpointAddress(appServeraddress + "/Data.asmx");

            proxyService.SetProxy(clientConfig, dataClient.Endpoint.Binding as BasicHttpBinding);
            await credentialService.SetWinCredentials(clientConfig, dataClient.ClientCredentials.Windows);
            if (appServeraddress.ToLower().StartsWith("https://"))
            {
                (dataClient.Endpoint.Binding as BasicHttpBinding).Security.Mode = BasicHttpSecurityMode.Transport;
                certificateValidationService.SetCertificateValidation(dataClient.ClientCredentials);
            }
            return dataClient;
        }

        /// <summary>
        /// Gets Application Server address from Version Manager server. Tries to get available languages from Application Server.
        /// Uses proxy and credentials settings in client config to connect Client Redirect server and Application Server.
        /// </summary>
        /// <param name="clientConfig">Helios Green client configuration.</param>
        /// <returns>Message with available languages.</returns>
        public async Task<string> GetAvailableLanguages(ClientConfig clientConfig)
        {
            var dataClient = await GetDataClient(clientConfig);
            return (await dataClient.GetInfoAsync("GETLANGUAGES", string.Empty)).Body.GetInfoResult;
        }

        /// <summary>
        /// Gets Application Server address from Version Manager server. Tries to get available languages from Application Server.
        /// Uses proxy and credentials settings in client config to connect Client Redirect server and Application Server.
        /// </summary>
        /// <param name="clientConfig">Helios Green client configuration.</param>
        /// <exception cref="ApplicationServerNotAvailableException">Problem to connect application server.</exception>
        public async Task CheckApplicationServerAvailable(ClientConfig clientConfig)
        {
            try
            {
                await GetAvailableLanguages(clientConfig);
            }
            catch (Exception e)
            {
                logger.LogError(e, $"Unable to connect application server. VersionManagerAddress: {clientConfig.ApplicationServer}");
                throw new ApplicationServerNotAvailableException(e.Message);
            }
        }
    }
}
