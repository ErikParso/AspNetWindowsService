using AspWinService.Model;
using AspWinService.Requests;
using AspWinService.Services;
using MediatR;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace AspWinService.RequestHandlers
{
    public class ClientDeleteRequestHandler : IRequestHandler<ClientDeleteRequest, IEnumerable<ClientInfo>>
    {
        private readonly ClientInfoService clientInfoService;

        public ClientDeleteRequestHandler(ClientInfoService clientInfoService)
        {
            this.clientInfoService = clientInfoService;
        }

        public Task<IEnumerable<ClientInfo>> Handle(ClientDeleteRequest request, CancellationToken cancellationToken)
        {
            var clientsInfo = clientInfoService.DeleteClientInfo(c => c.ClientId == request.ClientId);

            foreach (var removedClient in clientsInfo)
            {
                if (Directory.Exists(removedClient.InstallDir))
                    Directory.Delete(removedClient.InstallDir, true);
            }
            return Task.FromResult(clientsInfo);
        }
    }
}
