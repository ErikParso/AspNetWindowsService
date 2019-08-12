using AspWinService.Model;
using MediatR;
using System.ComponentModel.DataAnnotations;

namespace AspWinService.Requests
{
    public class NewClientInstallationRequest : IRequest<ClientInfo>
    {
        [Required]
        public string ClientName { get; set; }

        [Required]
        public string InstallDir { get; set; }
    }
}
