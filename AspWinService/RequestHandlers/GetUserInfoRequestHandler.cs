using AspWinService.Model;
using AspWinService.Requests;
using AspWinService.Services;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace AspWinService.RequestHandlers
{
    public class GetUserInfoRequestHandler : IRequestHandler<GetUserInfoRequest, CurrentUserInfo>
    {
        private readonly CurrentUserService currentUserService;

        public GetUserInfoRequestHandler(CurrentUserService currentUserService)
        {
            this.currentUserService = currentUserService;
        }

        public Task<CurrentUserInfo> Handle(GetUserInfoRequest request, CancellationToken cancellationToken)
        {
            return Task.FromResult(new CurrentUserInfo()
            {
                UserName = currentUserService.Account(),
                AppLocalPath = currentUserService.GetUserPath(Constants.CSIDL_LOCAL_APPDATA),

                CommonDesktop = Environment.GetFolderPath(Environment.SpecialFolder.CommonDesktopDirectory),
                CommonPrograms = Environment.GetFolderPath(Environment.SpecialFolder.CommonPrograms),

                UserDesktop = currentUserService.GetUserPath(Constants.CSIDL_DESKTOPDIRECTORY),
                UserPrograms = currentUserService.GetUserPath(Constants.CSIDL_PROGRAMS)
            });
        }
    }
}
