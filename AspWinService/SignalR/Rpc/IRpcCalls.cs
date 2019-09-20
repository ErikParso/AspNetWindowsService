using System.Threading.Tasks;

namespace AspWinService.SignalR.Rpc
{
    public interface IRpcCalls
    {
        Task MethodCall(MethodParams methodParams);
    }
}
