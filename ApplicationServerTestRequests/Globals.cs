using ICSharpCode.SharpZipLib.Zip;
using System.Data;
using System.IO;

namespace NorisWin32Update
{
    public class Globals
    {
        public static DataSet UserConfig = null;
        public static bool UseBASHX = false;
        public static string Language;
        public static string InstallDir;
        public static string AppServer;

        public static void Init(string appServer, string language, string installDir)
        {
            Language = language;
            InstallDir = installDir;
            AppServer = appServer;

            LoadConfig();
        }

        private static string GetMyCachePath()
        {
            var path = Path.Combine(InstallDir, $"Cache");
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            var tempPath = Path.Combine(path, "Temp");
            if (!Directory.Exists(tempPath))
                Directory.CreateDirectory(tempPath);

            return path;
        }


        public static void LoadConfig()
        {
            UserConfig = new DataSet();

            string path = Path.Combine(GetMyCachePath(), "Client.Config");

            if (!File.Exists(path))
                CreateClientConfig(AppServer, Language);

            XmlReadMode rm = XmlReadMode.IgnoreSchema;

            if (File.Exists(path + ".xsd"))
                UserConfig.ReadXmlSchema(path + ".xsd");
            else
                rm = XmlReadMode.Auto;

            using (FileStream Stream = new FileStream(path, FileMode.Open, FileAccess.Read))
            {
                using (StreamReader Xreader = new StreamReader(Stream))
                {
                    UserConfig.ReadXml(Xreader, rm);
                }
            }
        }

        public static void CreateClientConfig(string server, string language)
        {
                if (language == null || language == "") language = "CZ";
                if (server == null || server == "") server = "http://localhost/noris";
                string file = Path.Combine(GetMyCachePath(), "Client.Config");
                System.IO.StreamWriter sr = System.IO.File.CreateText(file);
                sr.WriteLine("<?xml version=\"1.0\" encoding=\"utf-8\" ?>");
                sr.WriteLine("<configuration xmlns=\"http://tempuri.org/Client.Config.xsd\">");
                sr.WriteLine("<userSettings section=\"LogIn\">");
                sr.WriteLine("  <key name=\"Server\" value=\"" + server + "\" />");
                sr.WriteLine("  <key name=\"Language\" value=\"" + language + "\" />");
                sr.WriteLine("</userSettings>");
                sr.WriteLine("</configuration>");
                sr.Close();
        }


        public static void UnzipFromStreamToStream(Stream inputStream, Stream outputStream)
        {
            // Refer to #ziplib documentation for more info on this
            ZipInputStream zipIn = new ZipInputStream(inputStream);
            ZipEntry theEntry = zipIn.GetNextEntry();
            byte[] buffer = new byte[2048];
            int size = 2048;
            while (true)
            {
                size = zipIn.Read(buffer, 0, buffer.Length);
                if (size > 0)
                {
                    outputStream.Write(buffer, 0, size);
                }
                else
                {
                    break;
                }
            }
        }
    }
}
