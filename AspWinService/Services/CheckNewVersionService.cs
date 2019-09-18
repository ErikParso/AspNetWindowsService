using AspWinService.ManifestProcessing;
using System.Linq;
using System.ServiceModel;
using System.Threading.Tasks;
using WSUpdate;

namespace AspWinService.Services
{
    public class CheckNewVersionService
    {
        private readonly ClientInfoService clientInfoService;
        private readonly UpdateProcessorService updateProcessorService;
        private readonly RedirectService redirectService;

        public CheckNewVersionService(
            ClientInfoService clientInfoService,
            UpdateProcessorService updateProcessorService,
            RedirectService redirectService)
        {
            this.clientInfoService = clientInfoService;
            this.updateProcessorService = updateProcessorService;
            this.redirectService = redirectService;
        }

        public async Task<bool> CheckNewVersion(string clientId)
        {
            ClientUpdateManifest currentManifest = null;
            var clientInfo = clientInfoService.GetClientInfo(clientId);
            var updateClient = await redirectService.GetUpdateClient(clientInfo.Config.ApplicationServer);

            var loader = new ClientManifestFileAccessor(clientInfo.InstallDir, clientInfo.InstallDir, updateClient);
            var parser = new UpdateManifestParser(clientInfo.InstallDir, clientInfo.InstallDir);

            if (loader.ExistsCurrentManifestFile)
            {
                currentManifest = parser.ParseManifest(loader.ReadCurrentManifestFileContent());
            }
            string updateManifestContent = await loader.ReadUpdateManifestFileContent();
            var updateManifest = parser.ParseManifest(updateManifestContent);

            var execPlan = updateProcessorService.CreateExecutePlan(updateClient, clientInfo.InstallDir, clientInfo.InstallDir, currentManifest, updateManifest);
            return execPlan.Any(i => i.Action == UpdateProcessorService.ExecutePlanItemAction.UpdateFile);
        }
    }
}
