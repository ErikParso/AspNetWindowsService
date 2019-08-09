using AspWinService.Model;
using AspWinService.Requests;
using AspWinService.Services;
using MediatR;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace AspWinService.RequestHandlers
{
    public class GetClientsInfoRequestHandler : IRequestHandler<GetClientsInfoRequest, IEnumerable<ClientInfo>>
    {
        private readonly ClientInfoService clientInfoService;

        public GetClientsInfoRequestHandler(ClientInfoService clientInfoService)
        {
            this.clientInfoService = clientInfoService;
        }

        public async Task<IEnumerable<ClientInfo>> Handle(GetClientsInfoRequest request, CancellationToken cancellationToken)
             => await Task.FromResult(clientInfoService.GetClientsInfo());     
    }
}
