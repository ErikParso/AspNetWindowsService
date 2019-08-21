using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace AspWinService.SignalR
{
    public class ProgressHub : Hub<IProgressHub>
    {
        public async Task ReportProgress(string processId, string itemId, string message, int progress, int imageIndex)
            =>  await Clients.All.ReportProgress(processId, itemId, message, progress, imageIndex);

        public async Task FinishProcess(string processId, string itemId)
            => await Clients.All.FinishProcess(processId, itemId);
    }
}
