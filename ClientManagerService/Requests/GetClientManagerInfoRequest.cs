using ClientManagerService.Model;
using MediatR;

namespace ClientManagerService.Requests
{
    /// <summary>
    /// Get Client Manager versions info request.
    /// </summary>
    public class GetClientManagerInfoRequest : IRequest<ClientManagerInfo>
    {

    }
}
