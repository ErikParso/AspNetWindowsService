using ClientManagerService.Model;
using ClientManagerService.SignalR;
using MediatR;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ClientManagerService.Requests
{
    /// <summary>
    /// New Helios Green client installation request.
    /// </summary>
    public class NewClientInstallationRequest : IRequest<ClientInfoExtended>
    {
        /// <summary>
        /// Process id. Needed to notify progress to Client Manager App using <see cref="ProgressHub"/>.
        /// </summary>
        [Required]
        public string InstallationProcessId { get; set; }

        /// <summary>
        /// Helios Green client id.
        /// </summary>
        [Required]
        public string ClientId { get; set; }

        /// <summary>
        /// Helios Green client name.
        /// </summary>
        [Required]
        public string ClientName { get; set; }

        /// <summary>
        /// Language.
        /// </summary>
        [Required]
        public string Language { get; set; }

        /// <summary>
        /// Installation directory.
        /// </summary>
        [Required]
        public string InstallDir { get; set; }

        /// <summary>
        /// Client Redirect server address.
        /// </summary>
        [Required]
        public string ApplicationServer { get; set; }

        /// <summary>
        /// Config file name.
        /// </summary>
        [Required]
        public string ConfigName { get; set; }

        /// <summary>
        /// Whether to create links in start menu for all users.
        /// </summary>
        [Required]
        public bool LnkForAllUser { get; set; }

        /// <summary>
        /// Whether to create desktop icon.
        /// </summary>
        [Required]
        public bool DesktopIcon { get; set; }

        /// <summary>
        /// Installation scope.
        /// true = per machine, false = per user.
        /// </summary>
        [Required]
        public bool InstallForAllUsers { get; set; }

        /// <summary>
        /// Configuration items collection.
        /// </summary>
        public IEnumerable<ClientConfigItem> ConfigItems { get; set; }
    }

}
