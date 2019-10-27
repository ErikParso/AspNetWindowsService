using MediatR;
using System.ComponentModel.DataAnnotations;

namespace ClientManagerService.Notifications
{
    /// <summary>
    /// Set Helios Green client file association.
    /// </summary>
    public class SetAssociationNotification : INotification
    {
        /// <summary>
        /// Helios Green Client installation directory.
        /// </summary>
        [Required]
        public string ClientId { get; set; }

        /// <summary>
        /// File extension.
        /// </summary>
        [Required]
        public string Extension { get; set; }
    }
}
