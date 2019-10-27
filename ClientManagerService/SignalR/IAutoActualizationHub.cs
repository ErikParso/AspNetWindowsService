using ClientManagerService.Model;
using System.Threading.Tasks;

namespace ClientManagerService.SignalR
{
    public interface IAutoActualizationHub
    {
        Task clientUpgradeCheck(string clientId);

        Task clientUpgradeCheckResult(string clientId, UpgradeInfo upgradeInfo, string message);

        Task clientAutoUpgrade(string clientId, string processId);

        Task clientAutoUpgradeResult(bool reslut, ClientInfoExtended clientInfo);
    }
}
