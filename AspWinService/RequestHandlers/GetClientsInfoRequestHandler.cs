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

        public GetClientsInfoRequestHandler(
            ClientInfoService clientInfoService,
            CheckNewVersionService checkNewVersionService)
        {
            this.clientInfoService = clientInfoService;
            this.checkNewVersionService = checkNewVersionService;
        }

        public async Task<IEnumerable<ClientInfoExtended>> Handle(GetClientsInfoRequest request, CancellationToken cancellationToken)
        {
            var clients = clientInfoService.GetClientsInfo().Select(c => new ClientInfoExtended()
            {
                ApplicationServer = c.ApplicationServer,
                ClientId = c.ClientId,
                ClientName = c.ClientName,
                Extensions = c.Extensions,
                InstallDir = c.InstallDir,
                Language = c.Language,
                NeedUpgrade = false
            }).ToList();

            if (request.CheckForUpgrades)
            {
                foreach (var c in clients)
                {
                    c.NeedUpgrade = await checkNewVersionService.CheckNewVersion(c.ClientId);
                }
            }

            return clients;
        }
    }
}
