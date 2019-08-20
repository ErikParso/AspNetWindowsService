using AspWinService.Model;
using AspWinService.Requests;
using AspWinService.Services;
using MediatR;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace AspWinService.RequestHandlers
{
    public class ClientDeleteRequestHandler : IRequestHandler<ClientDeleteRequest, ClientInfo>
    {
        private readonly ClientInfoService clientInfoService;

        public ClientDeleteRequestHandler(ClientInfoService clientInfoService)
        {
            this.clientInfoService = clientInfoService;
        }

        public Task<ClientInfo> Handle(ClientDeleteRequest request, CancellationToken cancellationToken)
        {
            var clientInfo = clientInfoService.DeleteClientInfo(c => c.ClientId == request.ClientId);

            if (Directory.Exists(clientInfo.InstallDir))
                Directory.Delete(clientInfo.InstallDir, true);

            return Task.FromResult(clientInfo);
        }
    }
}
