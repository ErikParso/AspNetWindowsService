using System.Threading.Tasks;

namespace ClientManagerService.SignalR.Rpc
{
    public class PendingMethodCall<T> : TaskCompletionSource<T>
    {
        public PendingMethodCall(string methodId, string connectionId)
        {
            MethodId = methodId;
            ConnectionId = connectionId;
        }

        public readonly string MethodId;

        public readonly string ConnectionId;
    }
}
