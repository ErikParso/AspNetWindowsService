using System.Threading.Tasks;

namespace AspWinService.SignalR.Rpc
{
    public interface IRpcResponseHandlers
    {
        Task MethodResponseHandler(MethodResponse response);
    }
}
