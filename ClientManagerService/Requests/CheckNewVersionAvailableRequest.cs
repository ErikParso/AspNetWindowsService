using ClientManagerService.Model;
using MediatR;

namespace ClientManagerService.Requests
{
    /// <summary>
    /// Check Helios Green client upgrade availability request.
    /// </summary>
    public class CheckNewVersionAvailableRequest : IRequest<UpgradeInfo>
    {
        /// <summary>
        /// Helios Green client id.
        /// </summary>
        public string ClientId { get; set; }
    }
}
