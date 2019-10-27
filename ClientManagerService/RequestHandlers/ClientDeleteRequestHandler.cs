using ClientManagerService.Exceptions;
using ClientManagerService.Extensions;
using ClientManagerService.Model;
using ClientManagerService.Requests;
using ClientManagerService.Services;
using MediatR;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ClientManagerService.RequestHandlers
{
    /// <summary>
    /// Delete Helios Green request handler.
    /// </summary>
    public class ClientDeleteRequestHandler : IRequestHandler<ClientDeleteRequest, IEnumerable<ClientInfoExtended>>
    {
        private readonly ClientInfoService clientInfoService;
        private readonly ClientLockService clientLockService;

        /// <summary>
        /// Initializes ClientDeleteRequestHandler.
        /// </summary>
        /// <param name="clientInfoService">Client info service.</param>
        /// <param name="clientLockService">Client lock service.</param>
        public ClientDeleteRequestHandler(
            ClientInfoService clientInfoService,
            ClientLockService clientLockService)
        {
            this.clientInfoService = clientInfoService;
            this.clientLockService = clientLockService;
        }

        /// <summary>
        /// Deletes Helios Green client specified in request.
        /// </summary>
        /// <param name="request">Request object.</param>
        /// <param name="cancellationToken">Cancelation token.</param>
        /// <returns>Deleted client info.</returns>
        public async Task<IEnumerable<ClientInfoExtended>> Handle(ClientDeleteRequest request, CancellationToken cancellationToken)
        {
            using (var clientLock = clientLockService.GetClientLockContext(request.ClientId))
            {
                await clientLock.Lock();

                if (clientInfoService.GetClientInfo(request.ClientId) == null)
                    throw new ClientNotFoundException(request.ClientId);

                var clientsInfo = clientInfoService.DeleteClientInfo(c => c.ClientId == request.ClientId);

                foreach (var removedClient in clientsInfo)
                {
                    if (Directory.Exists(removedClient.InstallDir))
                        Directory.Delete(removedClient.InstallDir, true);
                }
                return await Task.FromResult(clientsInfo
                    .Select(c => c.ToClientInfoExtended(UpgradeInfo.UpgradeAvailable)));
            }
        }
    }
}
