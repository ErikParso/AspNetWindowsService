namespace ClientManagerService.Model
{
    /// <summary>
    /// Current user information
    /// </summary>
    public class CurrentUserInfo
    {
        /// <summary>
        /// Current user name.
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// Appdata local path for current user.
        /// </summary>
        public string AppLocalPath { get; set; }

        /// <summary>
        /// Common desktop path.
        /// </summary>
        public string CommonDesktop { get; set; }

        /// <summary>
        /// Common programs path.
        /// </summary>
        public string CommonPrograms { get; set; }

        /// <summary>
        /// Current user desktop path.
        /// </summary>
        public string UserDesktop { get; set; }

        /// <summary>
        /// Current user programs path.
        /// </summary>
        public string UserPrograms { get; set; }
    }
}
