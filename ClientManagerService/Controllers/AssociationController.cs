using ClientManagerService.Notifications;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace ClientManagerService.Controllers
{
    /// <summary>
    /// Allows to set file associations to instelled Helios Green clients.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class AssociationController : ControllerBase
    {
        private readonly IMediator _mediator;

        /// <summary>
        /// Initializes AssociationController.
        /// </summary>
        /// <param name="mediator">mediator.</param>
        public AssociationController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Adds file association for Helios Green client specified by its installation folder.
        /// </summary>
        /// <param name="notification">Notification object.</param>
        /// <returns>Result without content.</returns>
        [HttpPost]
        [ProducesResponseType(200)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> AddFileAssociation(SetAssociationNotification notification)
        {
            await _mediator.Publish(notification);
            return Ok();
        }

        /// <summary>
        /// Runs associated Helios Green client using associated file or Uri
        /// </summary>
        /// <param name="notification">Notification object with specified associated file path or Uri.</param>
        /// <returns>Result without content.</returns>
        [HttpPost("runClient")]
        [ProducesResponseType(200)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> RunClient(RunAssociatedClientNotification notification)
        {
            await _mediator.Publish(notification);
            return Ok();
        }

    }
}