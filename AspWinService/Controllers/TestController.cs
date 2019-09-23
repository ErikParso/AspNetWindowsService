using AspWinService.SignalR.Rpc;
using AspWinService.SignalR.RpcHubs;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace AspWinService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TestController : ControllerBase
    {
        private readonly RpcHub<RpcLoginRequest, RpcLoginResponse> loginRpc;

        public TestController(RpcHub<RpcLoginRequest, RpcLoginResponse> loginRpc)
        {
            this.loginRpc = loginRpc;
        }

        [HttpGet("{userId}")]
        public async Task<IActionResult> Get(string userId)
        {
            var result = await loginRpc.MethodCall(userId, new RpcLoginRequest());
            return Ok("hello from v 1.0.11");
        }
    }
}
