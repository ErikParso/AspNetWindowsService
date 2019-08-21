using AspWinService.SignalR;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Threading.Tasks;

namespace AspWinService.Services
{
    public class ProgressService
    {
        private readonly IHubContext<ProgressHub, IProgressHub> progressHubContext;

        public ProgressService(IHubContext<ProgressHub, IProgressHub> progressHubContext)
        {
            this.progressHubContext = progressHubContext;
        }

        public async Task<string> WriteLog(string processId, int imageIndex, string fileName, int downloaded, int total, string status)
        {
            var itemId = Guid.NewGuid().ToString();
            await progressHubContext.Clients.All.ReportProgress(processId, itemId, fileName, (int)((double)downloaded / total * 100), imageIndex);
            return itemId;
        }

        public void UpdateLogItemImageIndex(string logItemKey, int imageIndex)
        {

        }

        public void UpdateLogItemSubItem(string logItemKey, string subItemKey, string newValue)
        {

        }
    }
}
