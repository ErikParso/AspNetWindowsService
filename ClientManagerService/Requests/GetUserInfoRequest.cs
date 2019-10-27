using ClientManagerService.Model;
using MediatR;

namespace ClientManagerService.Requests
{
    /// <summary>
    /// Get current user info request.
    /// </summary>
    public class GetUserInfoRequest : IRequest<CurrentUserInfo>
    {

    }
}
