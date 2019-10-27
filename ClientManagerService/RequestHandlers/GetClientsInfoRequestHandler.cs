using ClientManagerService.Extensions;
using ClientManagerService.Model;
using ClientManagerService.Requests;
using ClientManagerService.Services;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ClientManagerService.RequestHandlers
{
    /// <summary>
    /// Get Helios Green client info request handler.
    /// </summary>
    public class GetClientsInfoRequestHandler : IRequestHandler<GetClientsInfoRequest, IEnumerable<ClientInfoExtended>>
    {
        private readonly ClientInfoService clientInfoService;
        private readonly CheckNewVersionService checkNewVersionService;
        private readonly CurrentUserService currentUserService;
        private readonly ClientLockService clientLockService;
        private readonly RedirectService redirectService;
        private readonly CertificateValidator certificateValidator;

        /// <summary>
        /// Initializes GetClientsInfoRequestHandler.
        /// </summary>
        /// <param name="clientInfoService">Client info service.</param>
        /// <param name="checkNewVersionService">Check new version service.</param>
        /// <param name="currentUserService">Current user service.</param>
        /// <param name="clientLockService">Client lock service.</param>
        /// <param name="certificateValidator"></param>
        /// <param name="redirectService"></param>
        public GetClientsInfoRequestHandler(
            ClientInfoService clientInfoService,
            CheckNewVersionService checkNewVersionService,
            CurrentUserService currentUserService,
            ClientLockService clientLockService,
            RedirectService redirectService,
            CertificateValidator certificateValidator)
        {
            this.clientInfoService = clientInfoService;
            this.checkNewVersionService = checkNewVersionService;
            this.currentUserService = currentUserService;
            this.clientLockService = clientLockService;
            this.redirectService = redirectService;
            this.certificateValidator = certificateValidator;
        }

        /// <summary>
        /// Filters Helios Green clients available for current user. 
        /// Asynchronously checks for upgrades for each client if specified in request.
        /// </summary>
        /// <param name="request">Request object</param>
        /// <param name="cancellationToken">Cancelation token.</param>
        /// <returns>Collection of available Helios Green clients installations.</returns>
        public async Task<IEnumerable<ClientInfoExtended>> Handle(GetClientsInfoRequest request, CancellationToken cancellationToken)
        {
            var currentUser = currentUserService.Account();

            var clients = clientInfoService.GetClientsInfo()
                .Where(c => string.IsNullOrWhiteSpace(c.UserName) || c.UserName == currentUser)
                .Select(c => c.ToClientInfoExtended(UpgradeInfo.NotChecked))
                .ToList();

            if (request.CheckForUpgrades)
            {
                // parallel upgrade check to speed up process
                var tasks = clients.Select(async c =>
                {
                    try
                    {
                        if(!c.Config.Items.GetValueBool("LogIn", "IntegratedWindowsAuthentication"))
                            await redirectService.CheckApplicationServerAvailable(c.Config);

                        using (var clientLock = clientLockService.GetClientLockContext(c.ClientId))
                        {
                            await clientLock.Lock();

                            var needUpgrade = c.Config.Items.GetValueBool("LogIn", "IntegratedWindowsAuthentication")
                               ? UpgradeInfo.NotChecked
                               : await checkNewVersionService.CheckNewVersion(c.ClientId)
                                    ? UpgradeInfo.UpgradeAvailable
                                    : UpgradeInfo.IsActual;
                            c.UpgradeInfo = needUpgrade;
                        }
                    }
                    catch (Exception e)
                    {
                        c.UpgradeInfo = UpgradeInfo.UpgradeCheckFailed;
                    }
                });
                await Task.WhenAll(tasks);
            }

            return clients;
        }
    }
}
