using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace ClientManagerService.Services
{
    /// <summary>
    /// Provides lock context for specific client. Within client lock context only 1 process cant work with client at same time.
    /// </summary>
    public class ClientLockService
    {
        private readonly ConcurrentDictionary<string, SemaphoreSlim> clientLockingContext;

        /// <summary>
        /// Initializes <see cref="ClientLockContext"/>.
        /// </summary>
        public ClientLockService()
        {
            clientLockingContext = new ConcurrentDictionary<string, SemaphoreSlim>();
        }

        /// <summary>
        /// Gets lock context for specific client.
        /// </summary>
        /// <param name="clientId">Client id.</param>
        /// <returns>Client lock context.</returns>
        public ClientLockContext GetClientLockContext(string clientId)
        {
            var semaphore = GetClientSemaphore(clientId);
            var lockContext = new ClientLockContext(semaphore);
            return lockContext;
        }

        private SemaphoreSlim GetClientSemaphore(string clientId)
            => clientLockingContext.GetOrAdd(clientId, clientId => new SemaphoreSlim(1, 1));

        /// <summary>
        /// Client lock context.
        /// </summary>
        public class ClientLockContext : IDisposable
        {
            private readonly SemaphoreSlim clientSemaphore;

            /// <summary>
            /// Inititalizes <see cref="ClientLockContext"/>.
            /// </summary>
            /// <param name="clientSemaphore">Semaphore assignet to client.</param>
            public ClientLockContext(SemaphoreSlim clientSemaphore)
            {
                this.clientSemaphore = clientSemaphore;
            }

            /// <summary>
            /// Locks client.
            /// </summary>
            /// <returns></returns>
            public async Task Lock()
            {
                await clientSemaphore.WaitAsync();
            }

            /// <summary>
            /// Unlocks client.
            /// </summary>
            public void Dispose()
            {
                clientSemaphore.Release();
            }
        }
    }
}
