using MediatR;

namespace ClientManagerService.Notifications
{
    /// <summary>
    /// Run associated client notification.
    /// Handler runs associated client using specified file or uri.
    /// </summary>
    public class RunAssociatedClientNotification: INotification
    {
        /// <summary>
        /// Associated file path.
        /// </summary>
        public string FilePath { get; set; }

        /// <summary>
        /// Helios Green client uri.
        /// </summary>
        public string Uri { get; set; }
    }
}
