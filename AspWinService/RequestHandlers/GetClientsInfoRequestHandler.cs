using AspWinService.Model;
using AspWinService.Requests;
using AspWinService.Services;
using MediatR;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace AspWinService.RequestHandlers
{
    public class GetClientsInfoRequestHandler : IRequestHandler<GetClientsInfoRequest, IEnumerable<ClientInfoExtended>>
    {
        private readonly ClientInfoService clientInfoService;
        private readonly CheckNewVersionService checkNewVersionService;
        private readonly CurrentUserService currentUserService;

        public GetClientsInfoRequestHandler(
            ClientInfoService clientInfoService,
            CheckNewVersionService checkNewVersionService,
            CurrentUserService currentUserService)
        {
            this.clientInfoService = clientInfoService;
            this.checkNewVersionService = checkNewVersionService;
            this.currentUserService = currentUserService;
        }

        public async Task<IEnumerable<ClientInfoExtended>> Handle(GetClientsInfoRequest request, CancellationToken cancellationToken)
        {
            var currentUser = currentUserService.Account();

            var clients = clientInfoService.GetClientsInfo().Select(c => new ClientInfoExtended()
            {
                ClientId = c.ClientId,
                ClientName = c.ClientName,
                InstallDir = c.InstallDir,
                UserName = c.UserName,
                Extensions = c.Extensions,                
                Config = c.Config,
                NeedUpgrade = false
            })
                .Where(c => string.IsNullOrWhiteSpace(c.UserName) || c.UserName == currentUser)
                .ToList();

            if (request.CheckForUpgrades)
            {
                // check updates parallel to speed up process
                var tasks = clients.Select(async c =>
                {
                    var needUpgrade = await checkNewVersionService.CheckNewVersion(c.ClientId);
                    c.NeedUpgrade = needUpgrade;
                });
                await Task.WhenAll(tasks);
            }

            return clients;
        }
    }
}
