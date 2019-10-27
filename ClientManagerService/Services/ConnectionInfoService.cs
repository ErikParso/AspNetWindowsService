using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using WSDataSSL;

namespace ClientManagerService.Services
{
    /// <summary>
    /// Uses <see cref="DataSoapClient"/> to get connect info from application server.
    /// Connection info is cached within request scope.
    /// </summary>
    public class ConnectionInfoService
    {
        private readonly ConcurrentDictionary<string, string> connectionsInfo;
        private readonly SemaphoreSlim semaphore;

        /// <summary>
        /// Inititalizes <see cref="ConnectionInfoService"/>.
        /// </summary>
        public ConnectionInfoService()
        {
            connectionsInfo = new ConcurrentDictionary<string, string>();
            semaphore = new SemaphoreSlim(1, 1);
        }

        /// <summary>
        /// Gets connection info from Application Server and caches it in dictionary.
        /// </summary>
        /// <param name="dataSoapClient"></param>
        /// <returns>Connection info string.</returns>
        public async Task<string> GetConnectionInfo(DataSoapClient dataSoapClient)
        {
            try
            {
                await semaphore.WaitAsync();

                var url = dataSoapClient.Endpoint.Address.Uri.AbsoluteUri;
                if (!connectionsInfo.TryGetValue(url, out string connectionInfo))
                {
                    connectionInfo = (await dataSoapClient.GetInfoAsync("GETCONNECTINFO", string.Empty)).Body.GetInfoResult;
                    connectionsInfo.TryAdd(url, connectionInfo);
                }
                return connectionInfo;
            }
            finally
            {
                semaphore.Release();
            }
        }

        /// <summary>
        /// Gets bashx property from connection info.
        /// </summary>
        /// <param name="dataSoapClient"></param>
        /// <returns>Whether to use bashx downloader to download files from application server.</returns>
        public async Task<bool> GetUseBashx(DataSoapClient dataSoapClient)
            => (await GetConnectionInfo(dataSoapClient)).Contains("BASHX=1");
    }
}
