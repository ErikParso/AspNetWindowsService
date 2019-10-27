using ClientManagerService.Model;
using ClientManagerService.SignalR;
using MediatR;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ClientManagerService.Requests
{
    /// <summary>
    /// Helios Green client delete request.
    /// </summary>
    public class ClientDeleteRequest : IRequest<IEnumerable<ClientInfoExtended>>
    {
        /// <summary>
        /// Process id. Needed to notify progress to Client Manager App using <see cref="ProgressHub"/>.
        /// </summary>
        [Required]
        public string DeleteProcessId { get; set; }

        /// <summary>
        /// Helios Green client id.
        /// </summary>
        [Required]
        public string ClientId { get; set; }
    }
}
