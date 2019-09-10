using AspWinService.Model;
using AspWinService.Requests;
using AspWinService.Services;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace AspWinService.RequestHandlers
{
    public class VersionManagerAddressValidationRequestHandler : IRequestHandler<VersionManagerAddressValidationRequest, ValidationResult>
    {
        private readonly RedirectService redirectService;

        public VersionManagerAddressValidationRequestHandler(RedirectService redirectService)
        {
            this.redirectService = redirectService;
        }

        public async Task<ValidationResult> Handle(VersionManagerAddressValidationRequest request, CancellationToken cancellationToken)
        {
            var result = new ValidationResult();

            try
            {
                var applicationserverAddress = await redirectService.GetApplicationServerAddress(request.VersionManagerAddress);
                if (!string.IsNullOrWhiteSpace(applicationserverAddress))
                {
                    result.IsValid = true;
                    result.Message = applicationserverAddress;
                }
                else
                {
                    throw new Exception("Version manager returns empty application server address. Please, set correct version manager address.");
                }
            }
            catch (Exception e)
            {
                result.IsValid = false;
                result.Message = $"Cannot redirect to application server: {e.Message}";
            }

            return result;
        }
    }
}
