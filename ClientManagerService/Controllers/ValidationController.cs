using ClientManagerService.Requests;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace ClientManagerService.Controllers
{
    /// <summary>
    /// Validation controller.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class ValidationController : ControllerBase
    {
        private readonly IMediator mediator;

        /// <summary>
        /// Initializes ValidationController.
        /// </summary>
        /// <param name="mediator">Mediator.</param>
        public ValidationController(IMediator mediator)
        {
            this.mediator = mediator;
        }

        /// <summary>
        /// Validates Version Manager Address. 
        /// Tries to connect Version Manager server and get client redirect uri.
        /// Then tries to connect to Application Server and get available languages.
        /// </summary>
        /// <param name="request">Version Manager validation request.</param>
        /// <returns>Validation result with message. Message contains quarried languages or error message.</returns>
        /// <response code="200">Validation result with message. Message contains quarried languages or error message.</response>
        [HttpPost("versionManagerAddress")]
        [ProducesResponseType(200)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> Validate(VersionManagerAddressValidationRequest request)
        {
            var result = await mediator.Send(request);
            return Ok(result);
        }
    }
}
