using ClientManagerService.Model;
using MediatR;
using System.Collections.Generic;

namespace ClientManagerService.Requests
{
    /// <summary>
    /// Get Helios Green client installations info collection request.
    /// </summary>
    public class GetClientsInfoRequest: IRequest<IEnumerable<ClientInfoExtended>>
    {
        /// <summary>
        /// Whether to check upgrade avaibility for each client in collection.
        /// </summary>
        public bool CheckForUpgrades { get; set; }
    }
}
