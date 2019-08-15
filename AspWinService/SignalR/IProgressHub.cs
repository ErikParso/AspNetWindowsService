using System.Threading.Tasks;

namespace AspWinService.SignalR
{
    public interface IProgressHub
    {
        Task ReportProgress(string processId, string itemId, string message, int progress, int imageIndex);
        Task FinishProcess(string processId, string itemId);
    }
}
