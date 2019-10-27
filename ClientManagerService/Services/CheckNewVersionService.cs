using ClientManagerService.Extensions;
using ClientManagerService.ManifestProcessing;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Threading.Tasks;
using WSUpdate;
using static ClientManagerService.Services.UpdateProcessorService;

namespace ClientManagerService.Services
{
    /// <summary>
    /// Service used to check, whether there is an upgrade available for Helios Green client.
    /// </summary>
    public class CheckNewVersionService
    {
        private readonly ClientInfoService clientInfoService;
        private readonly UpdateProcessorService updateProcessorService;
        private readonly RedirectService redirectService;
        private readonly ClientNameService clientNameService;

        /// <summary>
        /// Initializes CheckNewVersionService.
        /// </summary>
        /// <param name="clientInfoService">Client info service.</param>
        /// <param name="updateProcessorService">Update manifest file processor service.</param>
        /// <param name="redirectService">Client redirect service.</param>
        public CheckNewVersionService(
            ClientInfoService clientInfoService,
            UpdateProcessorService updateProcessorService,
            RedirectService redirectService,
            ClientNameService clientNameService) 
        {
            this.clientInfoService = clientInfoService;
            this.updateProcessorService = updateProcessorService;
            this.redirectService = redirectService;
            this.clientNameService = clientNameService;
        }

        /// <summary>
        /// Uses update manifest file processor to compare manifest files and create execute plan.
        /// Returns whether there are some items in execute plan and thus whether an update is available.
        /// </summary>
        /// <param name="clientId">Helios Green client id to check updates for.</param>
        /// <returns>
        /// Whether there is an upgrade for Helios Green client.
        /// </returns>
        public async Task<bool> CheckNewVersion(string clientId)
        {
            var execPlan = await GetExecutePlan(clientId);
            return execPlan.Any(i => i.Action == UpdateProcessorService.ExecutePlanItemAction.UpdateFile);
        }

        public async Task<IEnumerable<ExecutePlanItem>> GetExecutePlan(string clientId)
        {
            ClientUpdateManifest currentManifest = null;
            var clientInfo = clientInfoService.GetClientInfo(clientId);
            var updateClient = await redirectService.GetUpdateClient(clientInfo.Config);

            var loader = new ClientManifestFileAccessor(clientInfo.InstallDir, clientInfo.InstallDir, updateClient);
            var parser = new UpdateManifestParser(clientInfo.InstallDir, clientInfo.InstallDir);

            if (loader.ExistsCurrentManifestFile)
            {
                currentManifest = parser.ParseManifest(loader.ReadCurrentManifestFileContent());
            }
            var clientAuthor = clientInfo.Config.Items?.GetValueString("Client", "ClientAuthor");
            var clientName = clientInfo.Config.Items?.GetValueString("Client", "ClientName");
            string updateManifestContent = await loader.ReadUpdateManifestFileContent(
                clientAuthor ?? string.Empty,
                clientNameService.IsSupportedClientName(clientName) ? clientName : "Win32");
            var updateManifest = parser.ParseManifest(updateManifestContent);

            var execPlan = updateProcessorService.CreateExecutePlan(updateClient, clientInfo.InstallDir, clientInfo.InstallDir, currentManifest, updateManifest);
            return execPlan;
        }

    }
}
