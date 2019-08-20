using AspWinService.Model;
using AspWinService.Requests;
using AspWinService.Services;
using MediatR;
using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace AspWinService.RequestHandlers
{
    public class NewClientInstallationRequestHandler : IRequestHandler<NewClientInstallationRequest, ClientInfo>
    {
        private readonly ClientInfoService clientInfoService;
        private readonly DownloadService downloadService;
        private readonly ManifestService manifestService;

        public NewClientInstallationRequestHandler(
            ClientInfoService clientInfoService,
            DownloadService downloadService,
            ManifestService manifestService)
        {
            this.clientInfoService = clientInfoService;
            this.downloadService = downloadService;
            this.manifestService = manifestService;
        }

        public async Task<ClientInfo> Handle(NewClientInstallationRequest request, CancellationToken cancellationToken)
        {
            var installDir = Path.Combine(request.InstallDir, request.ClientName);
            var tempDir = Path.Combine(Path.GetTempPath(), $@"HeliosGreenTemp\{request.ClientName}");

            if (!Directory.Exists(installDir))
                Directory.CreateDirectory(installDir);

            if (!Directory.Exists(tempDir))
                Directory.CreateDirectory(tempDir);

            manifestService.LoadConfig(installDir, request.ApplicationServer, "CZ");
            await downloadService.DownloadClient(request.InstallationProcessId, tempDir, installDir, request.ApplicationServer);

            var clientInfo = new ClientInfo()
            {
                ClientId = request.ClientId,
                ClientName = request.ClientName,
                InstallDir = installDir,
                Version = "undefined",
                ApplicationServer = request.ApplicationServer,
                Extensions = Enumerable.Empty<string>()
            };
            clientInfoService.AddClientInfo(clientInfo);

            return clientInfo;
        }
    }
}
