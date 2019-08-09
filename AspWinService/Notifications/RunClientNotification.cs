using MediatR;

namespace AspWinService.Notifications
{
    public class RunClientNotification : INotification
    {
        public string ClientName { get; set; }
    }
}
