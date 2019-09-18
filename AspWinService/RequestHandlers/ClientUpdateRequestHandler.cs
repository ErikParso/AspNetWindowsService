using AspWinService.Model;
using AspWinService.Requests;
using AspWinService.Services;
using MediatR;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace AspWinService.RequestHandlers
{
    public class ClientUpdateRequestHandler : IRequestHandler<ClientUpdateRequest, ClientInfo>
    {
        private readonly ClientInfoService clientInfoService;
        private readonly DownloadService downloadService;

        public ClientUpdateRequestHandler(
            ClientInfoService clientInfoService,
            DownloadService downloadService)
        {
            this.clientInfoService = clientInfoService;
            this.downloadService = downloadService;
        }

        public async Task<ClientInfo> Handle(ClientUpdateRequest request, CancellationToken cancellationToken)
        {
            var clientInfo = clientInfoService.GetClientInfo(request.ClientId);
            var tempDir = Path.Combine(Path.GetTempPath(), $@"HeliosGreenTemp\{clientInfo.ClientName}");
            await downloadService.DownloadClient(request.UpdateProcessId, tempDir, clientInfo.InstallDir, clientInfo.Config.ApplicationServer);
            return clientInfo;
        }
    }
}
