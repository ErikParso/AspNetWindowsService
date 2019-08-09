using AspWinService.Notifications;
using AspWinService.Services;
using MediatR;
using Microsoft.Win32;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace AspWinService.NotificationHandlers
{
    public class SetAssociationNotificationHandler : INotificationHandler<SetAssociationNotification>
    {
        private readonly ClientInfoService clientInfoService;
        private readonly TrayClientService trayClientService;

        public SetAssociationNotificationHandler(
            ClientInfoService clientInfoService,
            TrayClientService trayClientService)
        {
            this.clientInfoService = clientInfoService;
            this.trayClientService = trayClientService;
        }

        public async Task Handle(SetAssociationNotification request, CancellationToken cancellationToken)
        {
            clientInfoService.ProcessClientInfo(
                c => c.InstallDir == request.InstallDir,
                c => {
                    var trayClientPath = trayClientService.GetServiceClientPath();
                    SetAssociation(request.Extension, $"HG_{c.ClientName}", $"Helios Green {c.ClientName} file.", trayClientPath);
                    c.Extensions = (c.Extensions ?? new List<string>()).Union(new[] { request.Extension });
                });

            await Task.FromResult("Dummy");
        }

        private bool SetAssociation(string extension, string progId, string fileTypeDescription, string applicationFilePath)
        {
            bool madeChanges = false;
            madeChanges |= SetKeyDefaultValue(@"Software\Classes\" + extension, progId);
            madeChanges |= SetKeyDefaultValue(@"Software\Classes\" + progId, fileTypeDescription);
            madeChanges |= SetKeyDefaultValue($@"Software\Classes\{progId}\shell\open\command", "\"" + applicationFilePath + "\" \"%1\"");
            return madeChanges;
        }

        private bool SetKeyDefaultValue(string keyPath, string value)
        {
            using (var key = Registry.LocalMachine.CreateSubKey(keyPath))
            {
                if (key.GetValue(null) as string != value)
                {
                    key.SetValue(null, value);
                    return true;
                }
            }

            return false;
        }
    }
}
