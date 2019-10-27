using MediatR;

namespace ClientManagerService.Notifications
{
    /// <summary>
    /// Run Helios Green client notification.
    /// </summary>
    public class RunClientNotification : INotification
    {
        /// <summary>
        /// Helios Green client Name.
        /// </summary>
        public string ClientId { get; set; }
    }
}
