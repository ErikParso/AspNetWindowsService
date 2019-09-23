using System;
using System.Threading.Tasks;

namespace AspWinService.SignalR.Rpc
{
    public interface IRpcHub<Q>
    {
        Task Request(Guid methodId, Q request);
    }
}
