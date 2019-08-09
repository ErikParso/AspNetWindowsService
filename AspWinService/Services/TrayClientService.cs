using System.IO;
using System.Reflection;

namespace AspWinService.Services
{
    public class TrayClientService
    {
        public string GetServiceClientPath()
            => Path.Combine(new FileInfo(Assembly.GetEntryAssembly().Location).DirectoryName, "..", "NgClient", Constants.TrayClientName);
    }
}
