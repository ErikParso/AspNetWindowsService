using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ClientManagerService.SignalR.Rpc
{
    /// <summary>
    /// SignalR RPC hub. Allows to notify clients with request object and wait for clients response.
    /// </summary>
    /// <typeparam name="Q">Request object type.</typeparam>
    /// <typeparam name="S">Response object type.</typeparam>
    public class RpcHub<Q, S> : Hub<IRpcHub<Q>>
    {
        private readonly ConcurrentDictionary<string, List<PendingMethodCall<S>>> pendingMethodCalls
            = new ConcurrentDictionary<string, List<PendingMethodCall<S>>>();

        private readonly List<TaskCompletionSource<bool>> _pendingWaitingsForConnections =
            new List<TaskCompletionSource<bool>>();

        private readonly IHubContext<RpcHub<Q, S>> context;

        /// <summary>
        /// Initializes <see cref="RpcHub{Q, S}"/>.
        /// </summary>
        /// <param name="context">Hub context used to contact all clients.</param>
        public RpcHub(IHubContext<RpcHub<Q, S>> context)
        {
            this.context = context;
        }

        /// <summary>
        /// Contact client which invokes this request. Waits for clients response.
        /// If requested client has disconnected, method returns null.
        /// </summary>
        /// <param name="request">Reques object to client.</param>
        /// <returns>Clients response object.</returns>
        public async Task<S> RequestClient(Q request)
        {
            PendingMethodCall<S> pendingMethodCall = null;
            var connectionId = Context.ConnectionId;
            var methodId = Guid.NewGuid().ToString();

            if (pendingMethodCalls.TryGetValue(connectionId, out List<PendingMethodCall<S>> pendingMethods))
            {
                pendingMethodCall = new PendingMethodCall<S>(methodId, connectionId);
                pendingMethods.Add(pendingMethodCall);
                await Clients.Client(connectionId).Request(methodId, request);
            }

            return pendingMethodCall != null
                ? await pendingMethodCall.Task
                : await Task.FromResult(default(S));
        }

        /// <summary>
        /// Contact all connected clients. Waits for first client response.
        /// If no client is connected, <paramref name="clientConnectTimeout"/> is used.
        /// If no client connects within <paramref name="clientConnectTimeout"/>, method returns null.
        /// If requested client has disconnected, method returns null.
        /// </summary>
        /// <param name="request">Reques object to clients.</param>
        /// <param name="clientConnectTimeout">How long in ms will service wait for client to connect.</param>
        /// <returns>Clients response object.</returns>
        public async Task<S> RequestAllClients(Q request, int clientConnectTimeout = 1000)
        {
            PendingMethodCall<S> pendingMethodCall = null;
            var methodId = Guid.NewGuid().ToString();

            if (!pendingMethodCalls.Any() && clientConnectTimeout > 0)
            {
                await WaitForClientToConnect(clientConnectTimeout);
            }

            if (pendingMethodCalls.Any())
            {
                pendingMethodCall = new PendingMethodCall<S>(methodId, string.Empty);
                foreach (var pendingMethods in pendingMethodCalls.Values)
                {
                    pendingMethods.Add(pendingMethodCall);
                }
                await context.Clients.All.SendAsync("request", methodId, request);
            }

            return pendingMethodCall != null
                ? await pendingMethodCall.Task
                : await Task.FromResult(default(S));
        }

        private async Task<bool> WaitForClientToConnect(int clientConnectTimeout)
        {
            var waiting = new TaskCompletionSource<bool>();

            lock (_pendingWaitingsForConnections)
            {
                _pendingWaitingsForConnections.Add(waiting);
            }

            new Timer((s) =>
            {
                lock (_pendingWaitingsForConnections)
                {
                    _pendingWaitingsForConnections.Remove(waiting);
                }
                waiting.TrySetResult(false);
            }, false, clientConnectTimeout, Timeout.Infinite);

            return await waiting.Task;
        }

        /// <summary>
        /// Method used by requested client to response to service.
        /// Client will process request notification and responses to service using this method.
        /// </summary>
        /// <param name="methodId">Same method id as specified in request method.</param>
        /// <param name="response">Clients response.</param>
        public Task MethodResponse(string methodId, S response)
        {
            foreach (var methodCalls in pendingMethodCalls.Values)
            {
                var pendingMethods = methodCalls.Where(m => m.MethodId == methodId)
                    .ToList();
                foreach (var pendingMethod in pendingMethods)
                {
                    pendingMethod.TrySetResult(response);
                    methodCalls.Remove(pendingMethod);
                }
            }

            return Task.CompletedTask;
        }

        /// <summary>
        /// Calling this method client can receive connection id.
        /// </summary>
        public string GetConnectionId()
        {
            return Context.ConnectionId;
        }

        public override Task OnConnectedAsync()
        {
            pendingMethodCalls.TryAdd(Context.ConnectionId, new List<PendingMethodCall<S>>());

            lock (_pendingWaitingsForConnections)
            {
                foreach (var waiting in _pendingWaitingsForConnections)
                    waiting.TrySetResult(true);
                _pendingWaitingsForConnections.Clear();
            }

            return base.OnConnectedAsync();
        }

        public override Task OnDisconnectedAsync(Exception exception)
        {
            if (pendingMethodCalls.TryRemove(Context.ConnectionId, out List<PendingMethodCall<S>> pendingMethods))
            {
                foreach (var pendingMethod in pendingMethods)
                {
                    pendingMethod.SetResult(default);
                }
            }

            return base.OnDisconnectedAsync(exception);
        }
    }
}
