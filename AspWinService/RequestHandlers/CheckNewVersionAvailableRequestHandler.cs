using AspWinService.Requests;
using AspWinService.Services;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace AspWinService.RequestHandlers
{
    public class CheckNewVersionAvailableRequestHandler : IRequestHandler<CheckNewVersionAvailableRequest, bool>
    {
        private readonly CheckNewVersionService checkNewVersionService;

        public CheckNewVersionAvailableRequestHandler(CheckNewVersionService checkNewVersionService)
        {
            this.checkNewVersionService = checkNewVersionService;
        }

        public async Task<bool> Handle(CheckNewVersionAvailableRequest request, CancellationToken cancellationToken)
            => await checkNewVersionService.CheckNewVersion(request.ClientId);
    }
}
