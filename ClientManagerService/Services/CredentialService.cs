using ClientManagerService.Model;
using ClientManagerService.SignalR.Rpc;
using ClientManagerService.SignalR.RpcHubs;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.ServiceModel.Security;
using System.Threading;
using System.Threading.Tasks;

namespace ClientManagerService.Services
{
    /// <summary>
    /// Service used to prompt user for windows credentials if needed.
    /// </summary>
    public class CredentialService
    {
        private readonly RpcHub<RpcLoginRequest, RpcLoginResponse> loginRpc;
        private readonly CurrentUserService currentUserService;
        static SemaphoreSlim semaphoreSlim = new SemaphoreSlim(1, 1);

        private string userName;
        private string password;
        private bool credentialsSet;

        /// <summary>
        /// Initializes CredentialService.
        /// </summary>
        /// <param name="loginRpc">Login rpc hub.</param>
        public CredentialService(
            RpcHub<RpcLoginRequest, RpcLoginResponse> loginRpc,
            CurrentUserService currentUserService)
        {
            this.loginRpc = loginRpc;
            this.currentUserService = currentUserService;
        }

        /// <summary>
        /// Prompts user for windows credentials using CredentialsListener application.
        /// Caches userName and pasword and sets it into <paramref name="credential"/>.
        /// </summary>
        /// <param name="config">Helios Green client configuration.</param>
        /// <param name="credential">Windows Credentials object reference.</param>
        public async Task SetWinCredentials(ClientConfig config, WindowsClientCredential credential)
        {
            if (config.Items.Any(c => c.Section == "LogIn" && c.Key == "IntegratedWindowsAuthentication" && c.Value == "1"))
            {
                await semaphoreSlim.WaitAsync();

                if (!credentialsSet)
                {
                    RunCredentialsListener();
                    var request = new RpcLoginRequest()
                    {
                        UserName = currentUserService.Account(),
                        Server = config.ApplicationServer
                    };
                    var result = await loginRpc.RequestAllClients(request, 10_000);
                    if (result != null)
                    {
                        userName = result.UserName;
                        password = result.Password;
                        credentialsSet = true;
                    }
                    else
                    {
                        semaphoreSlim.Release();
                        throw new Exception("Windows credentials was not set.");
                    }
                }
                semaphoreSlim.Release();

                credential.ClientCredential = new NetworkCredential(userName, password, config.ApplicationServer);
            }
        }

        private static void RunCredentialsListener()
        {
            var servicePath = new FileInfo(Assembly.GetEntryAssembly().Location).DirectoryName;
            var ngClientPath = Path.Combine(servicePath, "..", "Credentials Listener", "CredentialsListener.exe");
            ProcessExtensions.StartProcessAsCurrentUser(ngClientPath);
        }
    }
}
