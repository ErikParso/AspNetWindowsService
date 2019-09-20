using AspWinService.Model;
using System;
using System.Linq;
using System.ServiceModel.Security;

namespace AspWinService.Services
{
    public class CredentialService
    {
        private readonly CurrentUserService currentUserService;

        public CredentialService(CurrentUserService currentUserService)
        {
            this.currentUserService = currentUserService;
        }

        public void SetWinCredentials(ClientConfig config, WindowsClientCredential credential)
        {
            if (config.Items.Any(c => c.Section == "LogIn" && c.Key == "IntegratedWindowsAuthentication" && c.Value == "1"))
            {
                var username = currentUserService.Account();
                var password = "*****";

                if (System.Environment.OSVersion.Platform != System.PlatformID.Win32NT ||
                    System.Environment.OSVersion.Version.Major < 5)
                {
                    username = config.Items.FirstOrDefault(c => c.Section == "LoginCache" && c.Key == "LastWindowsLogin")?.Value ?? string.Empty;
                    throw new NotImplementedException("For win older than xp show somehow... dialog to set username and password.");
                    // NorinsWin32Update.Globals.cs line 68.
                    // Tips: use SignalR or Specify Credentials before installation start.
                    // username = dialogResult
                    // password = dialogresult
                }
                else
                {

                }

                credential.ClientCredential.UserName = username;
                credential.ClientCredential.Password = password;
            }
        }
    }
}
