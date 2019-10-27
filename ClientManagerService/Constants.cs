namespace ClientManagerService
{
    public static class Constants
    {
        public const string ClientManagerInfoUrl = "http://localhost:5100/api/installer";
        public const string TrayClientName = "ClientManagerApp.exe";

        public const string InstalledClientsFileName = "InstalledClients.json";

        public const string CurrentManifestFileName = "CurrentManifest.xml";
        public const string UpdateManifestFileName = "CurrentManifest.xml";
        public const string ManifestUpdateXsdSchemaUrl = "http://helios.eu/ClientUpdateManifest.xsd";

        public const string AssecoSolutions = "Asseco Solutions";
        public const string HeliosClients = "HELIOS Clients";
        public const string NorisWin32Clients = "NorisWin32Clients";

        public const int CSIDL_LOCAL_APPDATA = 0x001c;
        public const int CSIDL_DESKTOPDIRECTORY = 0x10;
        public const int CSIDL_PROGRAMS = 0x02;

        public const string ClientConfigUpdateXml = "ClientConfigUpdate.xml";
        public const string ClientManagerSettingsFileName = "ClientManagerSettings.json";
    }
}
