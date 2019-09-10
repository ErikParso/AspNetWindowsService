using AspWinService.Requests;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace AspWinService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ValidationController : ControllerBase
    {
        private readonly IMediator mediator;

        public ValidationController(IMediator mediator)
        {
            this.mediator = mediator;
        }

        [HttpPost("versionManagerAddress")]
        public async Task<IActionResult> Validate(VersionManagerAddressValidationRequest request)
        {
            var result = await mediator.Send(request);
            return Ok(result);
        }
    }
}
