using System.IO;
using System.Threading;
using System.Threading.Tasks;
using AspWinService.Notifications;
using AspWinService.Services;
using MediatR;

namespace AspWinService.NotificationHandlers
{
    public class RunClientNotificationHandler : INotificationHandler<RunClientNotification>
    {
        private readonly ClientInfoService clientInfoService;

        public RunClientNotificationHandler(ClientInfoService clientInfoService)
        {
            this.clientInfoService = clientInfoService;
        }

        public async Task Handle(RunClientNotification notification, CancellationToken cancellationToken)
        {
            clientInfoService.ProcessClientInfo(
                c => c.ClientName == notification.ClientName,
                c => ProcessExtensions.StartProcessAsCurrentUser(
                    Path.Combine(c.InstallDir, Constants.ClientFileName), null, c.InstallDir));
            await Task.FromResult("Dummy");
        }
    }
}
