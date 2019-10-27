namespace ClientManagerService.SignalR.RpcHubs
{
    /// <summary>
    /// Response object for <see cref="Rpc.RpcHub{RpcLoginRequest, RpcLoginResponse}"/>.
    /// </summary>
    public class RpcLoginResponse
    {
        /// <summary>
        /// Windows user name.
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// User password.
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// Domain.
        /// </summary>
        public string Domain { get; set; }
    }
}
