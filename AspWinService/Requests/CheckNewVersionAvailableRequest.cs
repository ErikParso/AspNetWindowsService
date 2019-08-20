using MediatR;

namespace AspWinService.Requests
{
    public class CheckNewVersionAvailableRequest : IRequest<bool>
    {
        public string ClientId { get; set; }
    }
}
