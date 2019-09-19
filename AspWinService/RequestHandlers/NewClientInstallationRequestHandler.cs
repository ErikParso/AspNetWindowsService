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
        private readonly CurrentUserService currentUserService;
        private readonly LinkService linkService;

        public NewClientInstallationRequestHandler(
            ClientInfoService clientInfoService,
            DownloadService downloadService,
            ManifestService manifestService,
            CurrentUserService currentUserService,
            LinkService linkService)
        {
            this.clientInfoService = clientInfoService;
            this.downloadService = downloadService;
            this.manifestService = manifestService;
            this.currentUserService = currentUserService;
            this.linkService = linkService;
        }

        public async Task<ClientInfo> Handle(NewClientInstallationRequest request, CancellationToken cancellationToken)
        {
            var installDir = Path.Combine(request.InstallDir, request.ClientName);
            var tempDir = Path.Combine(Path.GetTempPath(), $@"HeliosGreenTemp\{request.ClientName}");

            var config = new ClientConfig()
            {
                Language = request.Language,
                ApplicationServer = request.ApplicationServer,
                Items = request.ConfigItems,
                ConfigFileName = string.IsNullOrWhiteSpace(request.ConfigName)
                    ? string.Empty
                    : request.ConfigName.EndsWith(".config")
                        ? request.ConfigName
                        : request.ConfigName + ".config"
            };

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
            await downloadService.DownloadClient(request.InstallationProcessId, tempDir, installDir, config);

            var clientInfo = new ClientInfo()
            {
                ClientId = request.ClientId,
                ClientName = request.ClientName,
                InstallDir = installDir,
                UserName = request.InstallForAllUsers ? string.Empty : currentUserService.Account(),
                Extensions = Enumerable.Empty<string>(),
                Config = config
            };
            clientInfoService.AddClientInfo(clientInfo);

            var clientMainFile = Path.Combine(installDir, Constants.NorisWin32Exe);
            var programsPath = request.InstallForAllUsers && request.LnkForAllUser
                ? Environment.GetFolderPath(Environment.SpecialFolder.CommonPrograms)
                : currentUserService.GetUserPath(Constants.CSIDL_PROGRAMS);
            programsPath += $@"\{Constants.AssecoSolutions}\{Constants.HeliosClients}\{request.ClientName}";
            linkService.CreateLinks(programsPath, request.ClientName, clientMainFile, installDir);

            if (request.DesktopIcon)
            {
                var desktop = request.InstallForAllUsers
                    ? Environment.GetFolderPath(Environment.SpecialFolder.CommonDesktopDirectory)
                    : currentUserService.GetUserPath(Constants.CSIDL_DESKTOPDIRECTORY);
                linkService.CreateDesktopLinks(desktop, request.ClientName, clientMainFile, installDir);
            }

            return clientInfo;
        }
    }
}
