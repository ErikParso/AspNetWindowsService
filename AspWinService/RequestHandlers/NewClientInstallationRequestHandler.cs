using AspWinService.Model;
using AspWinService.Requests;
using AspWinService.Services;
using MediatR;
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

            // delete clients with same name from disc and database
            var existingClients = clientInfoService.GetClientsInfo().Where(c => c.ClientName == request.ClientName);
            foreach(var existingClient in existingClients)
            {
                if (Directory.Exists(existingClient.InstallDir))
                    Directory.Delete(existingClient.InstallDir, true);
            }
            clientInfoService.DeleteClientInfo(c => c.ClientName == request.ClientName);


            if (!Directory.Exists(installDir))
                Directory.CreateDirectory(installDir);

            if (!Directory.Exists(tempDir))
                Directory.CreateDirectory(tempDir);


            manifestService.LoadConfig(installDir, request.ApplicationServer, request.Language);
            await downloadService.DownloadClient(request.InstallationProcessId, tempDir, installDir, request.ApplicationServer);

            var clientInfo = new ClientInfo()
            {
                ClientId = request.ClientId,
                ClientName = request.ClientName,
                InstallDir = installDir,
                Extensions = Enumerable.Empty<string>(),
                Config =  new ClientConfig()
                {
                    Language = request.Language,
                    ApplicationServer = request.ApplicationServer,
                    Items = request.ConfigItems,
                }
            };
            clientInfoService.AddClientInfo(clientInfo);

            return clientInfo;
        }
    }
}
