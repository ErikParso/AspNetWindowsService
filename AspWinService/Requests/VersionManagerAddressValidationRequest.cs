using AspWinService.Model;
using MediatR;

namespace AspWinService.Requests
{
    public class VersionManagerAddressValidationRequest : IRequest<ValidationResult>
    {
        public string VersionManagerAddress { get; set; }
    }
}
