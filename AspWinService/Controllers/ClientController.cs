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

        [HttpGet("needUpgrade/{clientId}")]
        public async Task<IActionResult> ClientNeedUpgrade(string clientId)
        {
            var request = new CheckNewVersionAvailableRequest() { ClientId = clientId };
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

        [HttpPost("downloadhegi")]
        public IActionResult DownloadHegiFileRequest(DownloadHegiFileRequest request)
        {
            byte[] fileBytes = System.Text.Encoding.ASCII.GetBytes("configuration json here");
            return File(fileBytes, "application/force-download", "MyProduct.msi");
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
