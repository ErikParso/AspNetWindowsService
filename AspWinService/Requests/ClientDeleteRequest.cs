using AspWinService.Model;
using MediatR;
using System.Collections.Generic;

namespace AspWinService.Requests
{
    public class ClientDeleteRequest : IRequest<IEnumerable<ClientInfo>>
    {
        public string DeleteProcessId { get; set; }
        public string ClientId { get; set; }
    }
}
