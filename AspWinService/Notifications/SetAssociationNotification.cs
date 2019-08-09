using MediatR;
using System.ComponentModel.DataAnnotations;

namespace AspWinService.Notifications
{
    public class SetAssociationNotification : INotification
    {
        [Required]
        public string InstallDir { get; set; }

        [Required]
        public string Extension { get; set; }
    }
}
