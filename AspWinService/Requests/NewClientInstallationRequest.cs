using AspWinService.Model;
using MediatR;
using System.Collections.Generic;
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
        public string Language { get; set; }

        [Required]
        public string InstallDir { get; set; }

        [Required]
        public string ApplicationServer { get; set; }

        [Required]
        public string ConfigName { get; set; }

        [Required]
        public bool LnkForAllUser { get; set; }

        [Required]
        public bool DesktopIcon { get; set; }

        [Required]
        public bool InstallForAllUsers { get; set; }

        public IEnumerable<ConfigItem> Config { get; set; }
    }

    public class ConfigItem
    {
        public string Section { get; set; }

        public string Key { get; set; }

        public string Value { get; set; }
    }

}
