using System;
using System.Threading.Tasks;

namespace ClientManagerService.SignalR.Rpc
{
    /// <summary>
    /// Specification for RpcHub.
    /// </summary>
    /// <typeparam name="Q">Request object type.</typeparam>
    public interface IRpcHub<Q>
    {
        Task Request(string methodId, Q request);
    }
}
