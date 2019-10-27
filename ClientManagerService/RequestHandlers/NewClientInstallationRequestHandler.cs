using ClientManagerService.Extensions;
using ClientManagerService.Model;
using ClientManagerService.Requests;
using ClientManagerService.Services;
using MediatR;
using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ClientManagerService.RequestHandlers
{
    /// <summary>
    /// New Helios Green client installation request handler.
    /// </summary>
    public class NewClientInstallationRequestHandler : IRequestHandler<NewClientInstallationRequest, ClientInfoExtended>
    {
        private readonly ClientInfoService clientInfoService;
        private readonly DownloadService downloadService;
        private readonly ManifestService manifestService;
        private readonly CurrentUserService currentUserService;
        private readonly LinkService linkService;
        private readonly ClientConfigUpdateService clientConfigUpdateService;
        private readonly ClientNameService clientNameService;
        private readonly ClientLockService clientLockService;

        /// <summary>
        /// initializes NewClientInstallationRequestHandler.
        /// </summary>
        /// <param name="clientInfoService"></param>
        /// <param name="downloadService"></param>
        /// <param name="manifestService"></param>
        /// <param name="currentUserService"></param>
        /// <param name="linkService"></param>
        /// <param name="clientLockService"></param>
        /// <param name="clientConfigUpdateService"></param>
        /// <param name="clientNameService"></param>
        public NewClientInstallationRequestHandler(
            ClientInfoService clientInfoService,
            DownloadService downloadService,
            ManifestService manifestService,
            CurrentUserService currentUserService,
            LinkService linkService,
            ClientConfigUpdateService clientConfigUpdateService,
            ClientNameService clientNameService,
            ClientLockService clientLockService)
        {
            this.clientInfoService = clientInfoService;
            this.downloadService = downloadService;
            this.manifestService = manifestService;
            this.currentUserService = currentUserService;
            this.linkService = linkService;
            this.clientConfigUpdateService = clientConfigUpdateService;
            this.clientNameService = clientNameService;
            this.clientLockService = clientLockService;
        }

        /// <summary>
        /// Installs new Helios Green client using information in <paramref name="request"/>.
        /// Saves Helios Green client installation info and configuration values in ClientInstallations.json file.
        /// </summary>
        /// <param name="request">Request object.</param>
        /// <param name="cancellationToken">Cancelation token.</param>
        /// <returns>Installed Helios Green client info.</returns>
        public async Task<ClientInfoExtended> Handle(NewClientInstallationRequest request, CancellationToken cancellationToken)
        {
            using (var clientLock = clientLockService.GetClientLockContext(request.ClientId))
            {
                await clientLock.Lock();

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

                // delete clients with same id if exists (as reinstallation)
                var existingClients = clientInfoService.GetClientsInfo()
                    .Where(c => c.ClientId == request.ClientId);
                foreach (var existingClient in existingClients)
                {
                    if (Directory.Exists(existingClient.InstallDir))
                        Directory.Delete(existingClient.InstallDir, true);
                }
                clientInfoService.DeleteClientInfo(c => c.ClientId == request.ClientId);


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
                clientConfigUpdateService.UpdateClientConfig(clientInfo);
                clientInfoService.AddClientInfo(clientInfo);

                var executableName = clientNameService.GetClientExecutableName(config);
                var clientMainFile = Path.Combine(installDir, executableName);
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

                return clientInfo.ToClientInfoExtended(UpgradeInfo.IsActual);
            }
        }
    }
}
