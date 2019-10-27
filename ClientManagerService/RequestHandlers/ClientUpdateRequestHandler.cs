using ClientManagerService.Exceptions;
using ClientManagerService.Extensions;
using ClientManagerService.Model;
using ClientManagerService.Requests;
using ClientManagerService.Services;
using MediatR;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace ClientManagerService.RequestHandlers
{
    /// <summary>
    /// Helios Green client update request handler.
    /// </summary>
    public class ClientUpdateRequestHandler : IRequestHandler<ClientUpdateRequest, ClientInfoExtended>
    {
        private readonly ClientInfoService clientInfoService;
        private readonly DownloadService downloadService;
        private readonly ClientConfigUpdateService clientConfigUpdateService;
        private readonly ClientLockService clientLockService;
        private readonly RedirectService redirectService;

        /// <summary>
        /// Initializes ClientUpdateRequestHandler.
        /// </summary>
        /// <param name="clientInfoService">Client info service.</param>
        /// <param name="downloadService">Download service.</param>
        /// <param name="clientConfigUpdateService"></param>
        /// <param name="clientLockService"></param>
        /// <param name="redirectService"></param>
        public ClientUpdateRequestHandler(
            ClientInfoService clientInfoService,
            DownloadService downloadService,
            ClientConfigUpdateService clientConfigUpdateService,
            ClientLockService clientLockService,
            RedirectService redirectService)
        {
            this.clientInfoService = clientInfoService;
            this.downloadService = downloadService;
            this.clientConfigUpdateService = clientConfigUpdateService;
            this.clientLockService = clientLockService;
            this.redirectService = redirectService;
        }

        /// <summary>
        /// Use download service to update Helios Green client.
        /// </summary>
        /// <param name="request">Request object.</param>
        /// <param name="cancellationToken">Cancelation token.</param>
        /// <returns>Upgraded client info.</returns>
        public async Task<ClientInfoExtended> Handle(ClientUpdateRequest request, CancellationToken cancellationToken)
        {
            var clientInfo = clientInfoService.GetClientInfo(request.ClientId);
            if (clientInfo == null)
                throw new ClientNotFoundException(request.ClientId);

            await redirectService.CheckApplicationServerAvailable(clientInfo.Config);

            using (var clientLock = clientLockService.GetClientLockContext(request.ClientId))
            {
                await clientLock.Lock();

                var tempDir = Path.Combine(Path.GetTempPath(), $@"HeliosGreenTemp\{clientInfo.ClientName}");
                await downloadService.DownloadClient(request.UpdateProcessId, tempDir, clientInfo.InstallDir, clientInfo.Config);

                clientInfoService.ProcessClientInfo(request.ClientId, c =>
                {
                    clientConfigUpdateService.UpdateClientConfig(c);
                    clientInfo = c;
                });

                return clientInfo.ToClientInfoExtended(UpgradeInfo.IsActual);
            }
        }
    }
}
