using CredentialsListener.Model;
using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CredentialsListener
{
    internal class ConnectionService
    {
        private readonly HubConnection connection;
        TaskCompletionSource<bool> waiting = new TaskCompletionSource<bool>();

        public ConnectionService()
        {
            connection = new HubConnectionBuilder()
                .WithUrl("http://localhost:5000/loginrpc")
                .Build();

            connection.On<string, RpcLoginRequest>("request", async (methodId, methodParams) =>
            {
                try
                {
                    var result = await Credentials.PromptCredentials(methodParams);
                    await connection.InvokeAsync("methodresponse", methodId, result);
                }
                catch (Exception e)
                {
                    await connection.InvokeAsync("methodresponse", methodId, null);
                }

                waiting.SetResult(true);
            });

            connection.Closed += async (error) =>
            {
                await Task.Delay(new Random().Next(0, 5) * 1000);
                await connection.StartAsync();
            };
        }

        public async Task StartConnection()
        {
            try
            {
                await connection.StartAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            await waiting.Task;
        }
    }
}
