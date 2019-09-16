using AspWinService.Model;
using MediatR;

namespace AspWinService.Requests
{
    public class GetUserInfoRequest : IRequest<CurrentUserInfo>
    {
    }
}
