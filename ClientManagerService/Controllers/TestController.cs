using ClientManagerService.Exceptions;
using ClientManagerService.Notifications;
using ClientManagerService.SignalR.Rpc;
using ClientManagerService.SignalR.RpcHubs;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace ClientManagerService.Controllers
{
    /// <summary>
    /// Test controller.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class TestController : ControllerBase
    {
        private readonly RpcHub<RpcLoginRequest, RpcLoginResponse> loginRpc;
        private readonly IMediator mediator;

        public TestController(
            RpcHub<RpcLoginRequest, RpcLoginResponse> loginRpc,
            IMediator mediator)
        {
            this.loginRpc = loginRpc;
            this.mediator = mediator;
        }

        /// <summary>
        /// Forces Auto Actualization iteration to execute now.
        /// </summary>
        /// <returns>Result without content.</returns>
        [HttpPost("forceAutoActualization")]
        [ProducesResponseType(200)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> ForceAutoActualization()
        {
            await mediator.Publish(new ForceAutoActualizationNotification());
            return Ok("auto actualization has started...");
        }

        /// <summary>
        /// Test HttpResponseException handling.
        /// </summary>
        /// <returns>Result without content.</returns>
        /// <response code="404">Client xyz not found.</response>
        [ProducesResponseType(404)]
        [HttpGet("getException")]
        public async Task<IActionResult> GetException()
        {
            throw new ClientNotFoundException("client-xyz");
            return await Task.FromResult(Ok());
        }
    }
}
