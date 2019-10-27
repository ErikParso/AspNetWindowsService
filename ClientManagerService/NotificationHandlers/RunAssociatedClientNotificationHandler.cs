using ClientManagerService.Notifications;
using ClientManagerService.Services;
using MediatR;
using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ClientManagerService.NotificationHandlers
{
    /// <summary>
    /// RunAssociatedClientNotification Handler.
    /// </summary>
    public class RunAssociatedClientNotificationHandler : INotificationHandler<RunAssociatedClientNotification>
    {
        private readonly ClientInfoService clientInfoService;
        private readonly ClientNameService clientNameService;

        /// <summary>
        /// Initializes RunAssociatedClientNotificationHandler.
        /// </summary>
        /// <param name="clientInfoService">Client info service.</param>
        /// <param name="clientNameService">Client name service.</param>
        public RunAssociatedClientNotificationHandler(
            ClientInfoService clientInfoService,
            ClientNameService clientNameService)
        {
            this.clientInfoService = clientInfoService;
            this.clientNameService = clientNameService;
        }

        /// <summary>
        /// Handles RunAssociatedClientNotification.
        /// Finds client info with specified extension or parses client name in uri.
        /// Starts instance of found client under current windows user.
        /// </summary>
        /// <param name="notification">Notification object.</param>
        /// <param name="cancellationToken">cancelation token.</param>
        /// <returns></returns>
        public async Task Handle(RunAssociatedClientNotification notification, CancellationToken cancellationToken)
        {
            if (!string.IsNullOrWhiteSpace(notification.FilePath))
            {
                var extension = Path.GetExtension(notification.FilePath);
                clientInfoService.ProcessClientInfo(
                    c => c.Extensions?.Contains(extension) ?? false,
                    c =>
                    {
                        var clientExecutableName = clientNameService.GetClientExecutableName(c.Config);
                        ProcessExtensions.StartProcessAsCurrentUser(
                            Path.Combine(c.InstallDir, clientExecutableName), notification.FilePath, c.InstallDir);
                    });
            }
            else if (!string.IsNullOrWhiteSpace(notification.Uri))
            {
                var uri = new Uri(notification.Uri);
                var clientId = uri.LocalPath.Split('/').First();
                clientInfoService.ProcessClientInfo(clientId, c =>
                {
                    var clientExecutableName = clientNameService.GetClientExecutableName(c.Config);
                    ProcessExtensions.StartProcessAsCurrentUser(
                        Path.Combine(c.InstallDir, clientExecutableName), notification.Uri, c.InstallDir);
                });
            }
            else
            {
                throw new Exception($"Missing FilePath or Uri in {nameof(RunAssociatedClientNotification)}.");
            }

            await Task.FromResult("dummy");
        }
    }
}
