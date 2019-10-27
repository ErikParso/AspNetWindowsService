using System;
using System.Threading.Tasks;

namespace CredentialsListener
{
    static class Program
    {
        [STAThread]
        static async Task Main()
        {
            var connectionService = new ConnectionService();
            await connectionService.StartConnection();
        }
    }
}
