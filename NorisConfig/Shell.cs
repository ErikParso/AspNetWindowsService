using IWshRuntimeLibrary;
using System.Text;

namespace Noris.Config
{
    public class Shell
	{
	    /// <summary>
	    /// Vytvoření link souboru
	    /// </summary>
	    /// <param name="pathLink">Cesta a jméno link souboru</param>
	    /// <param name="targetPath">Cesta a jméno souboru na který se link odkazuje</param>
	    /// <param name="workingDirectory">Výchozí adresář "Spustit v"</param>
	    /// <param name="windowStyle">Styl spuštění okna 0-Normal;1-Minimize;2-Maximize</param>
	    public static void CreateLinkFile(string pathLink, string targetPath, string workingDirectory, string arguments, int windowStyle)
	    {
	        CreateLinkFile(pathLink, targetPath, workingDirectory, arguments, windowStyle, false);
	    }
        /// <param name="runAsAdministrator">Vytvoří odkaz s elevací práv: RunAsAdministrator</param>
	    public static void CreateLinkFile(string pathLink, string targetPath, string workingDirectory, string arguments, int windowStyle, bool runAsAdministrator)//STR0055565 - 2017.05.03 - RunAsAdministrator
        {
			WshShell shell = new WshShell();
			IWshShortcut link = (IWshShortcut)shell.CreateShortcut(pathLink);
			link.TargetPath = targetPath;
			link.WindowStyle = 2;
			link.WorkingDirectory = workingDirectory;
			if (arguments != null)
				link.Arguments = arguments;
			link.Save();

            //STR0055565 - 2017.05.03 - RunAsAdministrator
            try
            {
                // HACKHACK: update the link's byte to indicate that this is a admin shortcut
                if (runAsAdministrator)
                {
                    using (var fs = new System.IO.FileStream(pathLink, System.IO.FileMode.Open, System.IO.FileAccess.ReadWrite))
                    {
                        fs.Seek(21, System.IO.SeekOrigin.Begin);
                        fs.WriteByte(0x22);
                    }
                }
            }
            catch { /*sunken exception: odkaz se vytvořil ale nepodařilo se nastavit RunAsAdministrator. Není to nijak kritické.*/ }
        }

		public static void CreateLinkFile(string pathLink, string targetPath, string workingDirectory, int windowStyle, string[] arguments)
		{
			StringBuilder args = new StringBuilder(20 * arguments.Length);
			foreach (string anArg in arguments)
			{
				args.Append((anArg.Contains(" ") ? " \"" : " ") + anArg + (anArg.Contains(" ") ? "\"" : ""));
			}
			CreateLinkFile(pathLink, targetPath, workingDirectory, args.ToString().Trim(), windowStyle);
		}

		public static void CreateLinkFile(string pathLink, string targetPath, string workingDirectory, int windowStyle)
		{
			CreateLinkFile(pathLink, targetPath, workingDirectory, null, windowStyle);
		}

	}
}
