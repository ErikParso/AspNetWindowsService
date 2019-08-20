using AspWinService.Model;
using MediatR;

namespace AspWinService.Requests
{
    public class ClientDeleteRequest : IRequest<ClientInfo>
    {
        public string DeleteProcessId { get; set; }
        public string ClientId { get; set; }
    }
}
