using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace AspWinService.SignalR.Rpc
{
    public class RpcHub : Hub<IRpcCalls>, IRpcResponseHandlers
    {
        private readonly IRpcCaller<RpcHub> _rpcCaller;

        public RpcHub(
            IRpcCaller<RpcHub> rpcCaller)
        {
            _rpcCaller = rpcCaller;
        }

        public Task MethodResponseHandler(MethodResponse response)
        {
            return _rpcCaller.MethodResponseHandler(response);
        }

        public string GetConnectionId()
        {
            return Context.ConnectionId;
        }
    }
}
