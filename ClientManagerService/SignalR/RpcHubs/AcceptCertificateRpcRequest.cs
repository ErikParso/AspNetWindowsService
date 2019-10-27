using System;
using System.Collections.Generic;

namespace ClientManagerService.SignalR.RpcHubs
{
    public class AcceptCertificateRpcRequest
    {
        public IEnumerable<string> Problems { get; set; }
        public string Issuer { get; set; }
        public string Subject { get; set; }
        public DateTime ValidFrom { get; set; }
        public DateTime ValidTill { get; set; }
    }
}
