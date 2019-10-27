using ClientManagerService.Model;

namespace ClientManagerService.Extensions
{
    /// <summary>
    /// Client info extensions.
    /// </summary>
    public static class ClientInfoExtensions
    {
        /// <summary>
        /// Creates <see cref="ClientInfoExtended"/> from <see cref="ClientInfo"/> object.
        /// </summary>
        /// <param name="clientInfo">Client info.</param>
        /// <param name="upgradeInfo">Upgrade info property.</param>
        /// <returns><see cref="ClientInfoExtended"/></returns>
        public static ClientInfoExtended ToClientInfoExtended(this ClientInfo clientInfo, UpgradeInfo upgradeInfo)
            => new ClientInfoExtended()
            {
                ClientId = clientInfo.ClientId,
                ClientName = clientInfo.ClientName,
                InstallDir = clientInfo.InstallDir,
                UserName = clientInfo.UserName,
                Extensions = clientInfo.Extensions,
                Config = clientInfo.Config,
                UpgradeInfo = upgradeInfo
            };
    }
}
