using Microsoft.AspNetCore.SignalR;

namespace ClientManagerService.SignalR
{
    public class AutoActualizationHub : Hub<IAutoActualizationHub>
    {
        /// <summary>
        /// Way a client can obtain connection id.
        /// </summary>
        /// <returns>Client connection id.</returns>
        public string GetConnectionId()
        {
            return Context.ConnectionId;
        }
    }
}
