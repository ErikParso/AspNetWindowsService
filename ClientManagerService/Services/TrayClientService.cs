using System.IO;
using System.Reflection;

namespace ClientManagerService.Services
{
    /// <summary>
    /// Provides location of Client Manager App executable.
    /// </summary>
    public class TrayClientService
    {
        /// <summary>
        /// Gets the location of Client Manager App.
        /// </summary>
        /// <returns>Client Manager executable file name.</returns>
        public string GetServiceClientPath()
            => Path.Combine(new FileInfo(Assembly.GetEntryAssembly().Location).DirectoryName, "..", "Client Manager App", Constants.TrayClientName);
    }
}
