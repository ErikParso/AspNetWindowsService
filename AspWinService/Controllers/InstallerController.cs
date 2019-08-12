using AspWinService.Requests;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace AspWinService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InstallerController : ControllerBase
    {
        private readonly IMediator mediator;

        public InstallerController(IMediator mediator)
        {
            this.mediator = mediator;
        }

        [HttpGet]
        public async Task<IActionResult> GetInfo()
        {
            var result = await mediator.Send(new GetClientManagerInfoRequest());
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> DownloadInstaller()
        {
            var result = await mediator.Send(new DownloadClientManagerRequest());
            return Ok(result);
        }
    }
}
