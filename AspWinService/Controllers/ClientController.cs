﻿using AspWinService.Notifications;
using AspWinService.Requests;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace AspWinService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClientController : ControllerBase
    {
        private readonly IMediator mediator;

        public ClientController(IMediator mediator)
        {
            this.mediator = mediator;
        }

        [HttpGet]
        public async Task<IActionResult> GetClients()
        {
            var request = new GetClientsInfoRequest();
            var result = await mediator.Send(request);
            return Ok(result);
        }

        [HttpGet("latestVersion")]
        public async Task<IActionResult> GetLatestVersion()
        {
            var request = new LatestClientVersionRequest();
            var result = await mediator.Send(request);
            return Ok(result);
        }

        [HttpPost("runClient")]
        public async Task<IActionResult> RunClient(RunClientNotification notification)
        {
            await mediator.Publish(notification);
            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> InstallNewClient(NewClientInstallationRequest request)
        {
            var result = await mediator.Send(request);
            return Ok(result);
        }

        [HttpPut]
        public async Task<IActionResult> UpdateClient(ClientUpdateRequest request)
        {
            var result = await mediator.Send(request);
            return Ok(result);
        }

        [HttpDelete()]
        public async Task<IActionResult> DeleteClient(ClientDeleteRequest request)
        {
            var result = await mediator.Send(request);
            return Ok(result);
        }
    }
}
