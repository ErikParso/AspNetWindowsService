namespace ClientManagerService.Model
{
    /// <summary>
    /// Client upgrade status.
    /// </summary>
    public enum UpgradeInfo
    {
        /// <summary>
        /// Upgrade was not checked or upgrade check was skipped for any reason.
        /// </summary>
        NotChecked = 0,

        /// <summary>
        /// Upgrade check was successfull. Upgrade is available.
        /// </summary>
        UpgradeAvailable = 1,

        /// <summary>
        /// Upgrade check was successfull. Client is up to date.
        /// </summary>
        IsActual = 2,

        /// <summary>
        /// Upgrade check failed for any reason.
        /// </summary>
        UpgradeCheckFailed = 3
    }
}
