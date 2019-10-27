using System.Collections.Generic;

namespace ClientManagerService.Model
{
    /// <summary>
    /// Model used to parse .hegi file in xml format.
    /// </summary>
    public class HegiDescriptor
    {
        /// <summary>
        /// Execution Type.
        /// </summary>
        public TypeExec TypeExec { get; set; }

        /// <summary>
        /// Rin wizard or run installation without wizard.
        /// </summary>
        public bool HideWizard { get; set; }

        /// <summary>
        /// Client name.
        /// </summary>
        public string ClientName { get; set; }

        /// <summary>
        /// Language.
        /// </summary>
        public string Language { get; set; }

        /// <summary>
        /// Install dir.
        /// </summary>
        public string InstallDir { get; set; }

        /// <summary>
        /// Client already installed behaviour.
        /// </summary>
        public ClientExistsAction ClientExists { get; set; }

        /// <summary>
        /// Version Manager server address.
        /// </summary>
        public string ApplicationServer { get; set; }

        /// <summary>
        /// Config file name.
        /// </summary>
        public string ConfigName { get; set; }

        /// <summary>
        /// Create links for all users.
        /// </summary>
        public bool LnkForAllUser { get; set; }

        /// <summary>
        /// Create desktop icon.
        /// </summary>
        public bool DesktopIcon { get; set; }

        /// <summary>
        /// Installation scope.
        /// </summary>
        public InstallationScope InstallScope { get; set; }

        /// <summary>
        /// Configuration items.
        /// </summary>
        public IEnumerable<ClientConfigItem> ConfigItems { get; set; }
    }

    /// <summary>
    /// Execution type.
    /// </summary>
    public enum TypeExec {

        /// <summary>
        /// Client installation.
        /// </summary>
        AddInstall,

        /// <summary>
        /// Client actualization.
        /// </summary>
        UpdateInstall,

        /// <summary>
        /// Client uninstalation.
        /// </summary>
        DeleteInstall
    }

    /// <summary>
    /// Client exists behavior.
    /// </summary>
    public enum ClientExistsAction
    {
        /// <summary>
        /// Show client exists yes/no dialog.
        /// </summary>
        Dialog,

        /// <summary>
        /// Cancel installation.
        /// </summary>
        End,

        /// <summary>
        /// Reinstall client without dialog.
        /// </summary>
        Delete
    }

    /// <summary>
    /// Client installation scope.
    /// </summary>
    public enum InstallationScope
    {
        /// <summary>
        /// Installation for all users.
        /// </summary>
        PerMachine,

        /// <summary>
        /// Installation for current user.
        /// </summary>
        PerUser
    }

}
