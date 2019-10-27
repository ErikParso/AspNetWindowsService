using ClientManagerService.Model;
using ClientManagerService.Requests;
using ClientManagerService.Services;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ClientManagerService.RequestHandlers
{
    /// <summary>
    /// Geet current user info request handler.
    /// </summary>
    public class GetUserInfoRequestHandler : IRequestHandler<GetUserInfoRequest, CurrentUserInfo>
    {
        private readonly CurrentUserService currentUserService;

        /// <summary>
        /// Initializes GetUserInfoRequestHandler.
        /// </summary>
        /// <param name="currentUserService">Current user service.</param>
        public GetUserInfoRequestHandler(CurrentUserService currentUserService)
        {
            this.currentUserService = currentUserService;
        }

        /// <summary>
        /// Gets current user info with user name and special paths.
        /// </summary>
        /// <param name="request">Request object.</param>
        /// <param name="cancellationToken">Cancelation token.</param>
        /// <returns>Current user information.</returns>
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
