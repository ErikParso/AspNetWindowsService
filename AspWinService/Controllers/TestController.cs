using AspWinService.SignalR.Rpc;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;

namespace AspWinService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TestController : ControllerBase
    {
        private readonly IRpcCaller<RpcHub> _rpcCaller;

        public TestController(IRpcCaller<RpcHub> rpcCaller)
        {
            _rpcCaller = rpcCaller;
        }

        [HttpGet("{userId}")]
        public async Task<IActionResult> Get(string userId)
        {
            var result = await _rpcCaller.MethodCall(userId, new MethodParams() { MethodCallId = Guid.NewGuid() });
            return Ok("hello from v 1.0.11");
        }
    }
}
