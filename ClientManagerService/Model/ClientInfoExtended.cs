namespace ClientManagerService.Model
{
    /// <summary>
    /// Helios Green client installation info enhanced by upgrade avaibility info.
    /// </summary>
    public class ClientInfoExtended : ClientInfo
    {
        /// <summary>
        /// Whether upgrade is available for Helios Green Client.
        /// </summary>
        public UpgradeInfo UpgradeInfo { get; set; }
    }
}
