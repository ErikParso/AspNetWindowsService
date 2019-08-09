using MediatR;

namespace AspWinService.Notifications
{
    public class RunAssociatedClientNotification: INotification
    {
        public string FilePath { get; set; }

        public string Uri { get; set; }
    }
}
