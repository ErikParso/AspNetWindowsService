﻿using AspWinService.Model;
using AspWinService.Requests;
using AspWinService.Services;
using MediatR;
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
                AppLocalPath = currentUserService.GetUserPath()
            });
        }
    }
}
