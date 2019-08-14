using AspWinService.Requests;
using MediatR;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace AspWinService.RequestHandlers
{
    public class LatestClientVersionRequestHandler : IRequestHandler<LatestClientVersionRequest, string>
    {
        public Task<string> Handle(LatestClientVersionRequest request, CancellationToken cancellationToken)
        {
            return Task.FromResult("undefined");
        }
    }
}
