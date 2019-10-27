using ClientManagerService.Requests;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace ClientManagerService.Controllers
{
    /// <summary>
    /// Controller provides Current logged in user information.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class CurrentUserController : ControllerBase
    {
        private readonly IMediator mediator;

        /// <summary>
        /// Initializes CurrentUserController.
        /// </summary>
        /// <param name="mediator">Mediator.</param>
        public CurrentUserController(IMediator mediator)
        {
            this.mediator = mediator;
        }

        /// <summary>
        /// Gets cuurent user name and special paths.
        /// </summary>
        /// <returns>Current user name and special paths.</returns>
        /// <response code="200">Current user name and special paths.</response>
        [HttpGet]
        [ProducesResponseType(200)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetCurrentUserInfo()
        {
            var request = new GetUserInfoRequest();
            var result = await mediator.Send(request);
            return Ok(result);
        }
    }
}
