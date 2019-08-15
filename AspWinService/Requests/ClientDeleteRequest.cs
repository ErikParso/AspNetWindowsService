﻿using AspWinService.Model;
using MediatR;

namespace AspWinService.Requests
{
    public class ClientDeleteRequest : IRequest<ClientInfo>
    {
        public string ClientName { get; set; }
    }
}