using System.Data;
using System.IO;

namespace AspWinService.Services
{
    public class ManifestService
    {
        public void LoadConfig(string installDir, string applicationServer, string language)
        {
            var userConfig = new DataSet();

            string path = Path.Combine(GetMyCachePath(installDir), "Client.Config");

            if (!File.Exists(path))
                CreateClientConfig(applicationServer, language, installDir);

            XmlReadMode rm = XmlReadMode.IgnoreSchema;

            if (File.Exists(path + ".xsd"))
                userConfig.ReadXmlSchema(path + ".xsd");
            else
                rm = XmlReadMode.Auto;

            using (FileStream Stream = new FileStream(path, FileMode.Open, FileAccess.Read))
            {
                using (StreamReader Xreader = new StreamReader(Stream))
                {
                    userConfig.ReadXml(Xreader, rm);
                }
            }
        }

        private string GetMyCachePath(string installDir)
        {
            var path = Path.Combine(installDir, $"Cache");
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            var tempPath = Path.Combine(path, "Temp");
            if (!Directory.Exists(tempPath))
                Directory.CreateDirectory(tempPath);

            return path;
        }

        private void CreateClientConfig(string server, string language, string installDir)
        {
            if (language == null || language == "") language = "CZ";
            if (server == null || server == "") server = "http://localhost/noris";
            string file = Path.Combine(GetMyCachePath(installDir), "Client.Config");
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
    }
}
