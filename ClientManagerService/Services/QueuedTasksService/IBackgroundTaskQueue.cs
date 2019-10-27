using System;
using System.Threading;
using System.Threading.Tasks;

namespace ClientManagerService.Services.QueuedTasksService
{
    /// <summary>
    /// Background tasks queue.
    /// </summary>
    public interface IBackgroundTaskQueue
    {
        /// <summary>
        /// Enqueuses work item (task) into queue.
        /// </summary>
        /// <param name="workItem">Work item.</param>
        void QueueBackgroundWorkItem(Func<CancellationToken, Task> workItem);

        /// <summary>
        /// Gets Wirking item from queue.
        /// </summary>
        /// <param name="cancellationToken">Cancelation token.</param>
        /// <returns>Dequeued wirk item.</returns>
        Task<Func<CancellationToken, Task>> DequeueAsync(CancellationToken cancellationToken);
    }
}
