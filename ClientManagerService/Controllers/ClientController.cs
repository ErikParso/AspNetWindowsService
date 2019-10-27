using ClientManagerService.Notifications;
using ClientManagerService.Requests;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace ClientManagerService.Controllers
{
    /// <summary>
    /// Helios Green clients management controller.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class ClientController : ControllerBase
    {
        private readonly IMediator mediator;

        /// <summary>
        /// Initializes ClientController.
        /// </summary>
        /// <param name="mediator">Mediator.</param>
        public ClientController(IMediator mediator)
        {
            this.mediator = mediator;
        }

        /// <summary>
        /// Gets installed Helios Green clients.
        /// </summary>
        /// <param name="checkForUpgrades">Whether to check upgrades for each client.</param>
        /// <returns>List of Helios green clients.</returns>
        /// <response code="200">List of Helios green clients.</response>
        [HttpGet("{checkForUpgrades}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetClients(bool checkForUpgrades)
        {
            var request = new GetClientsInfoRequest()
            {
                CheckForUpgrades = checkForUpgrades
            };
            var result = await mediator.Send(request);
            return Ok(result);
        }

        /// <summary>
        /// Checks whether Helios Green client upgrade is available.
        /// </summary>
        /// <param name="clientId">Helios Green client id.</param>
        /// <returns>If client upgrade is available.</returns>
        /// <response code="200">If client upgrade is available.</response>
        [HttpGet("needUpgrade/{clientId}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> ClientNeedUpgrade(string clientId)
        {
            var request = new CheckNewVersionAvailableRequest()
            {
                ClientId = clientId
            };
            var result = await mediator.Send(request);
            return Ok(result);
        }

        /// <summary>
        /// Finds Helios Green client specified by name and runs its instance.
        /// </summary>
        /// <param name="notification">Notification object with Helios Green client name.</param>
        /// <returns>Result without content.</returns>
        [HttpPost("runClient")]
        [ProducesResponseType(200)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> RunClient(RunClientNotification notification)
        {
            await mediator.Publish(notification);
            return Ok();
        }

        /// <summary>
        /// Installs new Helios Green client.
        /// </summary>
        /// <param name="request">Installation request.</param>
        /// <returns>Installed client info.</returns>
        /// <response code="200">Installed client info.</response>
        [HttpPost]
        [ProducesResponseType(200)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> InstallNewClient(NewClientInstallationRequest request)
        {
            var result = await mediator.Send(request);
            return Ok(result);
        }

        /// <summary>
        /// Updates Helios Green client.
        /// </summary>
        /// <param name="request">Update request object.</param>
        /// <returns>Updated client info.</returns>
        /// <response code="200">Updated client info.</response>
        [HttpPut]
        [ProducesResponseType(200)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> UpdateClient(ClientUpdateRequest request)
        {
            var result = await mediator.Send(request);
            return Ok(result);
        }

        /// <summary>
        /// Deletes Helios Green client.
        /// </summary>
        /// <param name="request">Request object.</param>
        /// <returns>Deleted client info.</returns>
        /// <response code="200">Deleted client info.</response>
        [HttpDelete()]
        [ProducesResponseType(200)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> DeleteClient(ClientDeleteRequest request)
        {
            var result = await mediator.Send(request);
            return Ok(result);
        }

        /// <summary>
        /// Parses xml content of .hegi file to HegiDescriptor object.
        /// </summary>
        /// <param name="request">Request object.</param>
        /// <returns>Parsed .hegi file content.</returns>
        /// <response code="200">Parsed .hegi file content.</response>
        [HttpPost("parseXmlHegiFile")]
        [ProducesResponseType(200)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> ParseXmlHegiFile(ParseXmlHegiFileRequest request)
        {
            var result = await mediator.Send(request);
            return Ok(result);
        }
    }
}
