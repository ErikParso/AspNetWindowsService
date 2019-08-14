using AspWinService.Services;
using System.Threading.Tasks;
using WSUpdate;

namespace AspWinService.Tools
{
    public sealed class FileDownloadProviderWS : FileDownloadProvider
    {
        private readonly ClientUpdateSoapClient updateClient;

        public FileDownloadProviderWS(string targetFileName, UpdateProcessorService.ExecutePlanItem executePlanItem, string logItemKey, ClientUpdateSoapClient updateClient, ProgressService progressService)
            : base(targetFileName, executePlanItem, logItemKey, progressService)
        {
            this.updateClient = updateClient;
        }

        protected override async Task<LoadClientFileDescriptor> CallDownloadRequest(string flags)
        {
            if (ExecutePlanItem.PluginIdentity == null)
            {
                return await updateClient.LoadClientFileAsync(
                    ExecutePlanItem.ClientIdentity.Author,
                    ExecutePlanItem.ClientIdentity.Name,
                    ExecutePlanItem.File.FileName, flags);
            }
            else
            {
                return await updateClient.LoadPluginFileAsync(
                    ExecutePlanItem.ClientIdentity.Author, ExecutePlanItem.ClientIdentity.Name,
                    ExecutePlanItem.PluginIdentity.Author, ExecutePlanItem.PluginIdentity.Name,
                    ExecutePlanItem.File.FileName, flags);
            }
        }
    }
}
