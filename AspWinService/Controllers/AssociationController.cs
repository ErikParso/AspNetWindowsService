using AspWinService.Notifications;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace AspWinService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AssociationController : ControllerBase
    {
        private readonly IMediator _mediator;

        public AssociationController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        public async Task<IActionResult> AddFileAssociation(SetAssociationNotification notification)
        {
            await _mediator.Publish(notification);
            return Ok();
        }

        [HttpPost("runClient")]
        public async Task<IActionResult> RunClient(RunAssociatedClientNotification notification)
        {
            await _mediator.Publish(notification);
            return Ok();
        }

    }
}