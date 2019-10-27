using ClientManagerService.Model;
using MediatR;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using ValidationResult = ClientManagerService.Model.ValidationResult;

namespace ClientManagerService.Requests
{
    /// <summary>
    /// Version Manager address validation request.
    /// </summary>
    public class VersionManagerAddressValidationRequest : IRequest<ValidationResult>
    {
        /// <summary>
        /// Version Manager address.
        /// </summary>
        [Required]
        public string VersionManagerAddress { get; set; }

        /// <summary>
        /// Configuration items. May contain proxy and credentials option needed to connect Version Manager server.
        /// </summary>
        public IEnumerable<ClientConfigItem> ConfigItems { get; set; }
    }
}
