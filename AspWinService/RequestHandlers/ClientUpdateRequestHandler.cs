using AspWinService.Model;
using AspWinService.Requests;
using AspWinService.Services;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace AspWinService.RequestHandlers
{
    public class ClientUpdateRequestHandler : IRequestHandler<ClientUpdateRequest, ClientInfo>
    {
        private readonly ClientInfoService clientInfoService;

        public ClientUpdateRequestHandler(ClientInfoService clientInfoService)
        {
            this.clientInfoService = clientInfoService;
        }

        public Task<ClientInfo> Handle(ClientUpdateRequest request, CancellationToken cancellationToken)
        {
            // Update simulation
            Thread.Sleep(5000);

            return Task.FromResult(clientInfoService.GetClientInfo(c => c.ClientId == request.ClientId));
        }
    }
}
