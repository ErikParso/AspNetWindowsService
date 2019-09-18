using Noris.Config;
using System.IO;

namespace AspWinService.Services
{
    public class LinkService
    {
        public void CreateLinks(string pathPrograms, string clientName, string clientMainFile, string clientDirectory)
        {
            if(!Directory.Exists(pathPrograms))
                Directory.CreateDirectory(pathPrograms);

            Shell.CreateLinkFile(Path.Combine(pathPrograms, clientName + Const.LinkExtName), clientMainFile, clientDirectory, 2);
            Shell.CreateLinkFile(Path.Combine(pathPrograms, clientName + " - FormBuilder" + Const.LinkExtName), Path.Combine(clientDirectory, "FormBuilder.exe"), clientDirectory, 0);
            Shell.CreateLinkFile(Path.Combine(pathPrograms, clientName + " - Update" + Const.LinkExtName), Path.Combine(clientDirectory, "NorisWin32UpdateLauncher.exe"), clientDirectory, 0);
        }

        public void CreateDesktopLinks(string pathDesktop, string clientName, string clientMainFile, string clientDirectory)
        {
            Shell.CreateLinkFile(Path.Combine(pathDesktop, clientName + Const.LinkExtName), clientMainFile, clientDirectory, 2);
        }
    }
}
