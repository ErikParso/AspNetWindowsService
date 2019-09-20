using System.Threading.Tasks;

namespace AspWinService.SignalR.Rpc
{
    public interface IRpc
    {
        Task<MethodResponse> MethodCall(string userId, MethodParams methodParams);
    }
}
