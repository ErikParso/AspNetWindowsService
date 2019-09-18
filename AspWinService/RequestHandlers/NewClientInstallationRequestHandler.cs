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
        private readonly CurrentUserService currentUserService;

        public NewClientInstallationRequestHandler(
            ClientInfoService clientInfoService,
            DownloadService downloadService,
            ManifestService manifestService,
            CurrentUserService currentUserService)
        {
            this.clientInfoService = clientInfoService;
            this.downloadService = downloadService;
            this.manifestService = manifestService;
            this.currentUserService = currentUserService;
        }

        public async Task<ClientInfo> Handle(NewClientInstallationRequest request, CancellationToken cancellationToken)
        {
            var installDir = Path.Combine(request.InstallDir, request.ClientName);
            var tempDir = Path.Combine(Path.GetTempPath(), $@"HeliosGreenTemp\{request.ClientName}");

            // delete clients with same name from disc and database
            var existingClients = clientInfoService.GetClientsInfo().Where(c => c.ClientName == request.ClientName);
            foreach (var existingClient in existingClients)
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
                UserName = request.InstallForAllUsers ? string.Empty : currentUserService.Account(),
                Extensions = Enumerable.Empty<string>(),
                Config = new ClientConfig()
                {
                    Language = request.Language,
                    ApplicationServer = request.ApplicationServer,
                    Items = request.ConfigItems,
                    ConfigFileName = string.IsNullOrWhiteSpace(request.ConfigName)
                        ? string.Empty
                        : request.ConfigName.EndsWith(".config")
                            ? request.ConfigName
                            : request.ConfigName + ".config"
                }
            };
            clientInfoService.AddClientInfo(clientInfo);

            return clientInfo;
        }
    }
}
