using ClientManagerService.Notifications;
using ClientManagerService.Services;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace ClientManagerService.NotificationHandlers
{
    /// <summary>
    /// Force auto actualization notification handler. Invokes auto actualization iteration.
    /// </summary>
    public class ForceAutoActualizationNotificationHandler : INotificationHandler<ForceAutoActualizationNotification>
    {
        private readonly ClientAutoUpgradeService clientAutoUpgradeService;

        /// <summary>
        /// Initializes <see cref="ForceAutoActualizationNotificationHandler"/>.
        /// </summary>
        /// <param name="clientAutoUpgradeService">Client auto upgrade service.</param>
        public ForceAutoActualizationNotificationHandler(ClientAutoUpgradeService clientAutoUpgradeService)
        {
            this.clientAutoUpgradeService = clientAutoUpgradeService;
        }

        /// <summary>
        /// Handles notification.
        /// </summary>
        /// <param name="notification">Notification object.</param>
        /// <param name="cancellationToken">Cancelation token.</param>
        /// <returns>Completed task.</returns>
        public Task Handle(ForceAutoActualizationNotification notification, CancellationToken cancellationToken)
        {
            clientAutoUpgradeService.QueueAutoUpgradeIteration();
            return Task.CompletedTask;
        }
    }
}
