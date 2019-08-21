using AspWinService.Model;
using MediatR;

namespace AspWinService.Requests
{
    public class ClientUpdateRequest : IRequest<ClientInfo>
    {
        public string UpdateProcessId { get; set; }
        public string ClientId { get; set; }
    }
}
