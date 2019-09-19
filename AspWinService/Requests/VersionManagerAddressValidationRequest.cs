using AspWinService.Model;
using MediatR;
using System.Collections.Generic;

namespace AspWinService.Requests
{
    public class VersionManagerAddressValidationRequest : IRequest<ValidationResult>
    {
        public string VersionManagerAddress { get; set; }

        public IEnumerable<ClientConfigItem> ConfigItems { get; set; }
    }
}
