using System;
using System.Collections.Generic;
using System.Text;

namespace Noris.Config
{
    /// <summary>
    /// Seznam konstant používaných během konfigurace
		/// c:\signtool.exe sign /f "c:\asseco.pfx" /p "patrik.73" /v $(TargetPath)
		/// c:\signtool.exe sign /f "c:\asseco.pfx" /p "patrik.73" /v $(TargetDir)Interop.IWshRuntimeLibrary.dll
    /// </summary>
    public class Const
    {

        /// <summary>
        /// Technical name, eg. for directory naming
        /// </summary>
        public const string ProductName = "Helios";
        /// <summary>
        /// Business name for captions. This is default bussines name. It may by changed, e.g., by ServerSetup.Config to "Helios Fenix".
        /// </summary>
        public const string BusinessName = "Helios Green";
        /// <summary>
        /// CommonApplicationData folder for NorisWin32Clients. 
        /// (c:\ProgramData\[CompanyName]\NorisWin32Clients)
        /// </summary>
        [Obsolete("use Noris.SetupUtilities.ClientRepozitory.ApplicationPath(applicationName) instead.", true)]
        public static string CommonApplicationDataFolderForWin32Clients
        {
            get { return System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), Noris.Config.Const.CompanyName + @"\NorisWin32Clients"); }
        }

        /// <summary> Oddělovač adresářů</summary>
        public static string DS = System.IO.Path.DirectorySeparatorChar.ToString();
        /// <summary> Název firmy </summary>        
        public const string CompanyName = "Asseco Solutions";// "LCS International"; //Asseco Solutions //STR0033689 - 2012.03.09
        /// <summary> Přípona lnk souboru </summary>
        public const string LinkExtName = ".lnk";
		/// <summary>Přípona konfiguračního souboru pro manažer klientů</summary>
		public const string ClientManagerConfigExtName = ".hegi";
		/// <summary>Seznam znaků které nesmí obsahovat název souboru</summary>
		public static char[] FileInvalidChars = new char[] { '\\', '/', ':', '*', '?', '<', '>', '|' };


        /// <summary>
        /// Konfigurační konstanty Fenix
        /// </summary>
        /// <remarks>
        /// Konstanty jsou použity v DbInstall.exe (DBInstall.Presents.FenixSetting).
        /// </remarks>
        public class Fenix
        {
            //NOL: Doplnil jsem poté, co jsem se pokusil přeložit DbInstall.exe a zjistil jsem, že v projektu NorisConfig 
            //konstanty chybí. Pravděpodobně nějaký necommitnutý kód z doby odchodu MEJ. Přesný význam však neznám.

            public const string LcsPassword = "";
            public const string UkPassword = "";
            public const string NetUserPassword = "";
            public const string NetAdminPassword = "";
        }
    }
}
