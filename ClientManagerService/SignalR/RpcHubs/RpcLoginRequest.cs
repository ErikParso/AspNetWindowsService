namespace ClientManagerService.SignalR.RpcHubs
{
    /// <summary>
    /// Request object for <see cref="Rpc.RpcHub{RpcLoginRequest, RpcLoginResponse}"/>.
    /// </summary>
    public class RpcLoginRequest
    {
        public string Server { get; set; }

        public string UserName { get; set; }
    }
}
