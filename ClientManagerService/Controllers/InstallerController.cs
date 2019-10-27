using ClientManagerService.Requests;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace ClientManagerService.Controllers
{
    /// <summary>
    /// Client Manager management controller.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class InstallerController : ControllerBase
    {
        private readonly IMediator mediator;

        /// <summary>
        /// Initializes InstallerController.
        /// </summary>
        /// <param name="mediator">Mediator.</param>
        public InstallerController(IMediator mediator)
        {
            this.mediator = mediator;
        }

        /// <summary>
        /// Gets Client Manager Local and Latest versions.
        /// </summary>
        /// <returns>Client manager local and latest version.</returns>
        /// <response code="200">Client manager local and latest version.</response>
        [HttpGet]
        [ProducesResponseType(200)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetInfo()
        {
            var result = await mediator.Send(new GetClientManagerInfoRequest());
            return Ok(result);
        }

        /// <summary>
        /// Downloads and saves latest Client manager installation pack from Production Server.
        /// </summary>
        /// <returns>Installation package location.</returns>
        /// <response code="200">Installation package location.</response>
        [HttpPost]
        [ProducesResponseType(200)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> DownloadInstaller()
        {
            var result = await mediator.Send(new DownloadClientManagerRequest());
            return Ok(result);
        }
    }
}
