using System.IO;
using System.Threading;
using System.Threading.Tasks;
using ClientManagerService.Notifications;
using ClientManagerService.Services;
using MediatR;

namespace ClientManagerService.NotificationHandlers
{
    /// <summary>
    /// RunClientNotification handler.
    /// </summary>
    public class RunClientNotificationHandler : INotificationHandler<RunClientNotification>
    {
        private readonly ClientInfoService clientInfoService;
        private readonly ClientNameService clientNameService;

        /// <summary>
        /// Initializes RunClientNotificationHandler.
        /// </summary>
        /// <param name="clientInfoService"></param>
        /// <param name="clientNameService"></param>
        public RunClientNotificationHandler(
            ClientInfoService clientInfoService,
            ClientNameService clientNameService)
        {
            this.clientInfoService = clientInfoService;
            this.clientNameService = clientNameService;
        }

        /// <summary>
        /// Runs client specified by notification object under current windows user.
        /// </summary>
        /// <param name="notification">Notification.</param>
        /// <param name="cancellationToken">Cancelation token.</param>
        /// <returns></returns>
        public Task Handle(RunClientNotification notification, CancellationToken cancellationToken)
        {
            clientInfoService.ProcessClientInfo(notification.ClientId,
                c => {
                    var clientExecutableName = clientNameService.GetClientExecutableName(c.Config);
                    ProcessExtensions.StartProcessAsCurrentUser(
                        Path.Combine(c.InstallDir, clientExecutableName), null, c.InstallDir);
                });

            return Task.CompletedTask;
        }
    }
}
