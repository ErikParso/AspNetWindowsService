using ClientManagerService.SignalR;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Threading.Tasks;

namespace ClientManagerService.Services
{
    /// <summary>
    /// Service used to report progress of installation, upgrade and delete process.
    /// </summary>
    public class ProgressService
    {
        private readonly IHubContext<ProgressHub, IProgressHub> progressHubContext;

        /// <summary>
        /// Initializes ProgressService.
        /// </summary>
        /// <param name="progressHubContext">Progress hub context.</param>
        public ProgressService(IHubContext<ProgressHub, IProgressHub> progressHubContext)
        {
            this.progressHubContext = progressHubContext;
        }

        /// <summary>
        /// Uses progress hub to notify Cleint Manager App about progress.
        /// </summary>
        /// <param name="processId">Process id.</param>
        /// <param name="imageIndex">Image index.</param>
        /// <param name="fileName">Processing file name.</param>
        /// <param name="downloaded">Current step.</param>
        /// <param name="total">Total number of steps.</param>
        /// <param name="status">Current step status (dots).</param>
        /// <returns>Log item key.</returns>
        public async Task<string> WriteLog(string processId, int imageIndex, string fileName, int downloaded, int total, string status)
        {
            var itemId = Guid.NewGuid().ToString();
            await progressHubContext.Clients.All.ReportProgress(processId, itemId, fileName, (int)((double)downloaded / total * 100), imageIndex);
            return itemId;
        }

        /// <summary>
        /// Updates log item.
        /// </summary>
        /// <param name="logItemKey">Log item key.</param>
        /// <param name="imageIndex">Image index. Represents current log item status.</param>
        public void UpdateLogItemImageIndex(string logItemKey, int imageIndex)
        {
            //TODO: not implemented yet.
        }

        /// <summary>
        /// Updates log item sub item.
        /// </summary>
        /// <param name="logItemKey">Log item key.</param>
        /// <param name="subItemKey">Log item subkey.</param>
        /// <param name="newValue">Log item value.</param>
        public void UpdateLogItemSubItem(string logItemKey, string subItemKey, string newValue)
        {
            //TODO: not implemented yet.
        }
    }
}
