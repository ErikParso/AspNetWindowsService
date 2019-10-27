using ClientManagerService.Model;
using ClientManagerService.Requests;
using ClientManagerService.Services;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ClientManagerService.RequestHandlers
{
    /// <summary>
    /// Check new Helios Green client version availability request handler.
    /// </summary>
    public class CheckNewVersionAvailableRequestHandler : IRequestHandler<CheckNewVersionAvailableRequest, UpgradeInfo>
    {
        private readonly CheckNewVersionService checkNewVersionService;
        private readonly ClientLockService clientLockService;
        private readonly RedirectService redirectService;
        private readonly ClientInfoService clientInfoService;

        /// <summary>
        /// Initializes CheckNewVersionAvailableRequestHandler.
        /// </summary>
        /// <param name="checkNewVersionService"></param>
        /// <param name="clientLockService"></param>
        /// <param name="redirectService"></param>
        /// <param name="clientInfoService"></param>
        public CheckNewVersionAvailableRequestHandler(
            CheckNewVersionService checkNewVersionService,
            ClientLockService clientLockService,
            RedirectService redirectService,
            ClientInfoService clientInfoService)
        {
            this.checkNewVersionService = checkNewVersionService;
            this.clientLockService = clientLockService;
            this.redirectService = redirectService;
            this.clientInfoService = clientInfoService;
        }

        /// <summary>
        /// Checks if there is an upgrade available for Helios Green client specified by request.
        /// </summary>
        /// <param name="request">Request object.</param>
        /// <param name="cancellationToken">Cancelation token.</param>
        /// <returns>
        /// If an upgrade is available for specified Helios Green client.
        /// </returns>
        public async Task<UpgradeInfo> Handle(CheckNewVersionAvailableRequest request, CancellationToken cancellationToken)
        {
            try
            {
                var clientInfo = clientInfoService.GetClientInfo(request.ClientId);
                await redirectService.CheckApplicationServerAvailable(clientInfo.Config);

                using (var clientLock = clientLockService.GetClientLockContext(request.ClientId))
                {
                    await clientLock.Lock();

                    return await checkNewVersionService.CheckNewVersion(request.ClientId)
                          ? UpgradeInfo.UpgradeAvailable
                          : UpgradeInfo.IsActual;
                }
            }
            catch
            {
                return UpgradeInfo.UpgradeCheckFailed;
            }
        }
    }
}
