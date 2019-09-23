using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace AspWinService.SignalR.Rpc
{
    public class RpcHub<Q, S> : Hub<IRpcHub<Q>>
    {
        private readonly ConcurrentDictionary<Guid, TaskCompletionSource<S>> _pendingMethodCalls =
            new ConcurrentDictionary<Guid, TaskCompletionSource<S>>();

        /// <summary>
        /// Contact client using "request" method and wait for response.
        /// </summary>
        /// <param name="clientId">Connected client id.</param>
        /// <param name="request">Request object.</param>
        /// <returns></returns>
        public async Task<S> MethodCall(string clientId, Q request)
        {
            var methodId = Guid.NewGuid();

            TaskCompletionSource<S> methodCallCompletionSource = new TaskCompletionSource<S>();
            if (_pendingMethodCalls.TryAdd(methodId, methodCallCompletionSource))
            {
                if (string.IsNullOrWhiteSpace(clientId))
                {
                    await Clients.All.Request(methodId, request);
                }
                else
                {
                    await Clients.Client(clientId).Request(methodId, request);
                }

                return await methodCallCompletionSource.Task;
            }

            throw new Exception("Couldn't call the method.");
        }

        /// <summary>
        /// Client response.
        /// </summary>
        /// <param name="methodId">Request method id.</param>
        /// <param name="response">Response object.</param>
        /// <returns></returns>
        public Task MethodResponse(Guid methodId, S response)
        {
            if (_pendingMethodCalls.TryRemove(methodId, out TaskCompletionSource<S> methodCallCompletionSource))
            {
                methodCallCompletionSource.SetResult(response);
            }

            return Task.CompletedTask;
        }

        /// <summary>
        /// Get connection id.
        /// </summary>
        /// <returns></returns>
        public string GetConnectionId()
        {
            return Context.ConnectionId;
        }
    }
}
