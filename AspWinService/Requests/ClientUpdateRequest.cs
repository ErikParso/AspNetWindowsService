using AspWinService.Model;
using MediatR;

namespace AspWinService.Requests
{
    public class ClientUpdateRequest : IRequest<ClientInfo>
    {
        public string InstallDir { get; set; }
    }
}
