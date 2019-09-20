using Microsoft.AspNetCore.SignalR;

namespace AspWinService.SignalR.Rpc
{
    public interface IRpcCaller<THub> : IRpc, IRpcResponseHandlers
        where THub : Hub<IRpcCalls>, IRpcResponseHandlers
    { }
}
