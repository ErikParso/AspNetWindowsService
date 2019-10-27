using ClientManagerService.Model;
using ClientManagerService.SignalR;
using MediatR;
using System.ComponentModel.DataAnnotations;

namespace ClientManagerService.Requests
{
    /// <summary>
    /// Helios Green client upgrade request.
    /// </summary>
    public class ClientUpdateRequest : IRequest<ClientInfoExtended>
    {
        /// <summary>
        /// Process id. Needed to notify progress to Client Manager App using <see cref="ProgressHub"/>.
        /// </summary>
        [Required]
        public string UpdateProcessId { get; set; }

        /// <summary>
        /// Helios Green client id.
        /// </summary>
        [Required]
        public string ClientId { get; set; }
    }
}
