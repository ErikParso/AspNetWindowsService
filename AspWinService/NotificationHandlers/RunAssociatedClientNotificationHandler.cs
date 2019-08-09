using AspWinService.Notifications;
using AspWinService.Services;
using MediatR;
using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace AspWinService.NotificationHandlers
{
    public class RunAssociatedClientNotificationHandler : INotificationHandler<RunAssociatedClientNotification>
    {
        private readonly ClientInfoService clientInfoService;

        public RunAssociatedClientNotificationHandler(ClientInfoService clientInfoService)
        {
            this.clientInfoService = clientInfoService;
        }

        public async Task Handle(RunAssociatedClientNotification notification, CancellationToken cancellationToken)
        {
            if (!string.IsNullOrWhiteSpace(notification.FilePath))
            {
                var extension = Path.GetExtension(notification.FilePath);
                clientInfoService.ProcessClientInfo(
                    c => c.Extensions?.Contains(extension) ?? false,
                    c =>
                    {
                        ProcessExtensions.StartProcessAsCurrentUser(
                            Path.Combine(c.InstallDir, Constants.ClientFileName), notification.FilePath, c.InstallDir);
                    });
            }
            else if (!string.IsNullOrWhiteSpace(notification.Uri))
            {
                var uri = new Uri(notification.Uri);
                var clientName = uri.LocalPath;
                clientInfoService.ProcessClientInfo(
                    c => c.ClientName == clientName,
                    c =>
                    {
                        ProcessExtensions.StartProcessAsCurrentUser(
                            Path.Combine(c.InstallDir, Constants.ClientFileName), notification.Uri, c.InstallDir);
                    });
            }
            else
            {
                throw new Exception($"Missing FilePath or Uri in {nameof(RunAssociatedClientNotification)}.");
            }

            await Task.FromResult("Dummy");
        }
    }
}
