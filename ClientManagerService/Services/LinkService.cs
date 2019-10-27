using Noris.Config;
using System.IO;

namespace ClientManagerService.Services
{
    /// <summary>
    /// Service used to create links in start menu and desktop.
    /// </summary>
    public class LinkService
    {
        /// <summary>
        /// Creates links for Helios Green client, Form buider and Update executables in start menu.
        /// </summary>
        /// <param name="pathPrograms">Programs special folder path. Can be current user or machine programs path.</param>
        /// <param name="clientName">Helios Green client name.</param>
        /// <param name="clientMainFile">Helios Green client executable path.</param>
        /// <param name="clientDirectory">Helios Green client installation dir.</param>
        public void CreateLinks(string pathPrograms, string clientName, string clientMainFile, string clientDirectory)
        {
            if(!Directory.Exists(pathPrograms))
                Directory.CreateDirectory(pathPrograms);

            Shell.CreateLinkFile(Path.Combine(pathPrograms, clientName + Const.LinkExtName), clientMainFile, clientDirectory, 2);
            Shell.CreateLinkFile(Path.Combine(pathPrograms, clientName + " - FormBuilder" + Const.LinkExtName), Path.Combine(clientDirectory, "FormBuilder.exe"), clientDirectory, 0);
            Shell.CreateLinkFile(Path.Combine(pathPrograms, clientName + " - Update" + Const.LinkExtName), Path.Combine(clientDirectory, "NorisWin32UpdateLauncher.exe"), clientDirectory, 0);
        }

        /// <summary>
        /// Creates desktop shortcut for installed Helios Green client.
        /// </summary>
        /// <param name="pathDesktop">Current user or machine desktop folder path.</param>
        /// <param name="clientName">Helios Green client name.</param>
        /// <param name="clientMainFile">Helios Green client executable path.</param>
        /// <param name="clientDirectory">Helios Green client installation dir.</param>
        public void CreateDesktopLinks(string pathDesktop, string clientName, string clientMainFile, string clientDirectory)
        {
            Shell.CreateLinkFile(Path.Combine(pathDesktop, clientName + Const.LinkExtName), clientMainFile, clientDirectory, 2);
        }
    }
}
