// Supervisor:DAVID.CHARVAT
// Part of HELIOS Green, proprietary software, (c) Asseco Solution, a. s.
// Redistribution and use in source and binary forms, with or without modification,
// is not permitted without valid contract with Asseco Solution, a. s.

using System;
using WSUpdate;
using System.Text;
using System.Threading.Tasks;

namespace NorisWin32Update.ManifestProcessing
{
    public sealed class ClientManifestFileAccessor
    {
        /// <summary> Filename of the current manifest file on client side. </summary>
        public const string CurrentManifestFileName = "CurrentManifest.xml";

        /// <summary> Filename of the update manifest file from aplication server. </summary>
        public const string UpdateManifestFileName = "UpdateManifest.xml";

        private readonly ClientUpdateSoapClient updateClient;
        public string TargetDirectory { get; private set; }


        public ClientManifestFileAccessor(string targetDirectory, ClientUpdateSoapClient updateClient)
        {
            TargetDirectory = targetDirectory;
            this.updateClient = updateClient;
        }

        public async Task<string> ReadUpdateManifestFileContent()
        {
            var cmd = await updateClient.GetClientManifestInfoAsync(string.Empty, "Win32");
            string messageFromServer = Encoding.UTF8.GetString(cmd.Data);
            if (cmd.WasError)
            {
                throw new Exception(messageFromServer);
            }
            return messageFromServer;
        }

        public string ReadCurrentManifestFileContent()
        {
            var currentFileName = CurrentManifestFullFileName;
            if (System.IO.File.Exists(currentFileName))
            {
                return System.IO.File.ReadAllText(currentFileName, Encoding.UTF8);
            }
            throw new Exception("Current manifest «" + currentFileName + "» not found!");
        }

        public bool ExistsCurrentManifestFile
        {
            get
            {
                return System.IO.File.Exists(CurrentManifestFullFileName);
            }
        }

        public string CurrentManifestFullFileName
        {
            get { return System.IO.Path.Combine(TargetDirectory, CurrentManifestFileName); }
        }

        internal void WriteCurrentManifest(string content)
        {
            System.IO.File.WriteAllText(CurrentManifestFullFileName, content, Encoding.UTF8);
        }
    }
}
