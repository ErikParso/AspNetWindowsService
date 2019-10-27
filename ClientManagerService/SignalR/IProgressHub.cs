using System.Threading.Tasks;

namespace ClientManagerService.SignalR
{
    /// <summary>
    /// Progress Hub specification.
    /// </summary>
    public interface IProgressHub
    {
        /// <summary>
        /// Notifies client connected to this hub about process progress.
        /// </summary>
        /// <param name="processId">Process id.</param>
        /// <param name="itemId">Progress item id.</param>
        /// <param name="message">Current step message.</param>
        /// <param name="progress">Progress percentage.</param>
        /// <param name="imageIndex">
        /// Progress item state specified by image index.
        /// TODO: replace by some kind of ProgressItemState enum.
        /// </param>
        Task ReportProgress(string processId, string itemId, string message, int progress, int imageIndex);

        /// <summary>
        /// Notifies client connected to this hub abot process finish.
        /// </summary>
        /// <param name="processId">Process id.</param>
        /// <param name="itemId">Item id.</param>
        Task FinishProcess(string processId, string itemId);
    }
}
