using AspWinService.Model;
using MediatR;
using System.ComponentModel.DataAnnotations;

namespace AspWinService.Requests
{
    public class NewClientInstallationRequest : IRequest<ClientInfo>
    {
        [Required]
        public string InstallationProcessId { get; set; }

        [Required]
        public string ClientId { get; set; }

        [Required]
        public string ClientName { get; set; }

        [Required]
        public string InstallDir { get; set; }

        [Required]
        public string ApplicationServer { get; set; }
    }
}
