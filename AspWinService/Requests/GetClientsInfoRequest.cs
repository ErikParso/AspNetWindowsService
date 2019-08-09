using AspWinService.Model;
using MediatR;
using System.Collections.Generic;

namespace AspWinService.Requests
{
    public class GetClientsInfoRequest: IRequest<IEnumerable<ClientInfo>>
    {

    }
}
