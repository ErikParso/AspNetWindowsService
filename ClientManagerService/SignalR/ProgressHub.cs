using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace ClientManagerService.SignalR
{
    /// <summary>
    /// Process prgress hub. Used to notify connected clients about process progress.
    /// </summary>
    public class ProgressHub : Hub<IProgressHub>
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
