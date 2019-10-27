using ClientManagerService.Extensions;
using ClientManagerService.Model;
using ClientManagerService.Services.QueuedTasksService;
using ClientManagerService.SignalR;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using static ClientManagerService.Services.UpdateProcessorService;

namespace ClientManagerService.Services
{
    /// <summary>
    /// Client auto actualization background service.
    /// </summary>
    public class ClientAutoUpgradeService : BackgroundService
    {
        private readonly HashSet<ExecutePlanItemAction> pluginActions =
            new HashSet<ExecutePlanItemAction>() {
                ExecutePlanItemAction.RunOnAfterInstall,
                ExecutePlanItemAction.RunOnAfterUninstall,
                ExecutePlanItemAction.RunOnAfterUpdate,
                ExecutePlanItemAction.RunOnBeforeInstall,
                ExecutePlanItemAction.RunOnBeforeUninstall,
                ExecutePlanItemAction.RunOnBeforeUpdate,
            };

        private readonly IBackgroundTaskQueue taskQueue;
        private readonly ILogger<ClientAutoUpgradeService> logger;
        private readonly ClientInfoService clientInfoService;
        private readonly CheckNewVersionService checkNewVersionService;
        private readonly DownloadService downloadService;
        private readonly ClientConfigUpdateService clientConfigUpdateService;
        private readonly IHubContext<AutoActualizationHub, IAutoActualizationHub> hubContext;
        private readonly ClientLockService clientLockService;
        private readonly ClientManagerSettingsService clientManagerSettingsService;
        private readonly CancellationToken cancellationToken;

        private bool isUpgradeRunning = false;

        /// <summary>
        /// Initializes <see cref="ClientAutoUpgradeService"/>
        /// </summary>
        /// <param name="taskQueue"></param>
        /// <param name="applicationLifetime"></param>
        /// <param name="logger"></param>
        /// <param name="clientInfoService"></param>
        /// <param name="checkNewVersionService"></param>
        /// <param name="downloadService"></param>
        /// <param name="ClientConfigUpdateService"></param>
        /// <param name="certificateValidationService"></param>
        /// <param name="hubContext"></param>
        /// <param name="clientLockService"></param>
        /// <param name="clientManagerSettingsService"></param>
        public ClientAutoUpgradeService(
            IBackgroundTaskQueue taskQueue,
            IHostApplicationLifetime applicationLifetime,
            ILogger<ClientAutoUpgradeService> logger,
            ClientInfoService clientInfoService,
            CheckNewVersionService checkNewVersionService,
            DownloadService downloadService,
            ClientConfigUpdateService ClientConfigUpdateService,
            CertificateValidationService certificateValidationService,
            IHubContext<AutoActualizationHub, IAutoActualizationHub> hubContext,
            ClientLockService clientLockService,
            ClientManagerSettingsService clientManagerSettingsService)
        {
            this.taskQueue = taskQueue;
            this.logger = logger;
            this.clientInfoService = clientInfoService;
            this.checkNewVersionService = checkNewVersionService;
            this.downloadService = downloadService;
            clientConfigUpdateService = ClientConfigUpdateService;
            this.hubContext = hubContext;
            this.clientLockService = clientLockService;
            this.clientManagerSettingsService = clientManagerSettingsService;
            this.cancellationToken = applicationLifetime.ApplicationStopping;

            // Dont want to custom validate certification within auto actualization process.
            certificateValidationService.CanCustomValidateCertificate = false;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            logger.LogInformation("Starting client auto upgrade service.");
            await StartAutoUpgradeCycle(stoppingToken);
        }

        /// <summary>
        /// Starts auto actualization cycle.
        /// </summary>
        /// <param name="stoppingToken">Cancelation token.</param>
        private async Task StartAutoUpgradeCycle(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                if (!isUpgradeRunning)
                {
                    isUpgradeRunning = true;
                    QueueAutoUpgradeIteration();
                }

                await Task.Delay(
                    TimeSpan.FromMinutes(clientManagerSettingsService.GetClientManagerSettings().AutoActualizationIntervalMin),
                    cancellationToken);
            }
        }

        /// <summary>
        /// Queues auto actualization iteration in background tasks queue.
        /// </summary>
        public void QueueAutoUpgradeIteration()
        {
            taskQueue.QueueBackgroundWorkItem(async token =>
            {
                logger.LogInformation("Started client automatic actualization process...");

                var clients = clientInfoService.GetClientsInfo()
                    .ToList();

                var tasks = clients.Select(async c =>
                {
                    using (var clientLock = clientLockService.GetClientLockContext(c.ClientId))
                    {
                        await clientLock.Lock();
                        await AutoUpgradeClient(c);
                    }
                });

                await Task.WhenAll(tasks);

                isUpgradeRunning = false;
            });
        }

        private async Task AutoUpgradeClient(ClientInfo c)
        {
            await hubContext.Clients.All.clientUpgradeCheck(c.ClientId);
            var upgradecheckResults = await GetClientupgradeInfo(c);
            await hubContext.Clients.All.clientUpgradeCheckResult(c.ClientId, upgradecheckResults.upgradeInfo, upgradecheckResults.message);

            if (upgradecheckResults.canExecuteUpgrade)
            {
                var upgradeProcessId = Guid.NewGuid().ToString();
                await hubContext.Clients.All.clientAutoUpgrade(c.ClientId, upgradeProcessId);
                var autoUpgradeResult = await UpgradeClient(c, upgradeProcessId);
                await hubContext.Clients.All.clientAutoUpgradeResult(autoUpgradeResult, autoUpgradeResult
                    ? c.ToClientInfoExtended(UpgradeInfo.IsActual)
                    : c.ToClientInfoExtended(UpgradeInfo.UpgradeAvailable));
            }
        }

        private async Task<bool> UpgradeClient(ClientInfo c, string upgradeProcessId)
        {
            logger.LogInformation($"Automatic client upgrade has started. Client:'{c.ClientName}'");

            try
            {
                var tempDir = Path.Combine(Path.GetTempPath(), $@"HeliosGreenTemp\{c.ClientName}");
                await downloadService.DownloadClient(upgradeProcessId, tempDir, c.InstallDir, c.Config);

                clientInfoService.ProcessClientInfo(c.ClientId, c =>
                {
                    clientConfigUpdateService.UpdateClientConfig(c);
                });

                logger.LogInformation($"Automatic client upgrade finished successfully. Client:'{c.ClientName}'");
                return true;
            }
            catch (Exception e)
            {
                logger.LogError(e, $"Automatic client upgrade failed! Client:'{c.ClientName}'");
                return false;
            }
        }

        private async Task<(bool canExecuteUpgrade, UpgradeInfo upgradeInfo, string message)> GetClientupgradeInfo(ClientInfo clientInfo)
        {
            logger.LogInformation($"Automatic client upgrade check started. Client:'{clientInfo.ClientName}'");
            try
            {
                var credentialsNeeded = clientInfo.Config.Items.GetValueBool("LogIn", "IntegratedWindowsAuthentication");
                if (credentialsNeeded)
                {
                    logger.LogInformation($"Automatic client upgrade check finished. Client won't upgrade... Client:'{clientInfo.ClientName}', Reason:'credentials required'");
                    return (false, UpgradeInfo.NotChecked, "Automatic upgrade check failed. Credentials are required.");
                }

                var execPlan = await checkNewVersionService.GetExecutePlan(clientInfo.ClientId);

                var anyFilesToUpdate = execPlan.Any(i => i.Action == UpdateProcessorService.ExecutePlanItemAction.UpdateFile);
                if (!anyFilesToUpdate)
                {
                    logger.LogInformation($"Automatic client upgrade check finished. Client won't upgrade... Client:'{clientInfo.ClientName}', Reason:'no files to upgrade'");
                    return (false, UpgradeInfo.IsActual, "All files are up to date.");
                }

                var anyPluginActions = execPlan.Any(i => IsPluginExecutionAction(i));
                if (anyPluginActions)
                {
                    logger.LogInformation($"Automatic client upgrade check finished. Client won't upgrade... Client:'{clientInfo.ClientName}', Reason:'upgrade contains plugin actions'");
                    return (false, UpgradeInfo.UpgradeAvailable, "Actualization contains plugin actions.");
                }

                logger.LogInformation($"Automatic client upgrade check finished. Client will upgrade... Client:'{clientInfo.ClientName}'");
                return (true, UpgradeInfo.UpgradeAvailable, "Auto actualization will execute.");

            }
            catch (Exception e)
            {
                logger.LogError(e, $"Automatic client upgrade check failed! Client:{clientInfo.ClientName}");
                return (false, UpgradeInfo.UpgradeCheckFailed, "Upgrade check has failed! Server is not available, has invalid certification or client resources are blocked");
            }
        }

        private bool IsPluginExecutionAction(ExecutePlanItem executePlanItem)
            => pluginActions.Contains(executePlanItem.Action) && !string.IsNullOrWhiteSpace(executePlanItem.TargetFileName);
    }
}
