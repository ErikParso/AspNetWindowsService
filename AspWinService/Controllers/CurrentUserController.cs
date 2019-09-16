using AspWinService.Requests;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace AspWinService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CurrentUserController : ControllerBase
    {
        private readonly IMediator mediator;

        public CurrentUserController(IMediator mediator)
        {
            this.mediator = mediator;
        }

        [HttpGet]
        public async Task<IActionResult> GetCurrentUserInfo()
        {
            var request = new GetUserInfoRequest();
            var result = await mediator.Send(request);
            return Ok(result);
        }
    }
}
