using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Xml;
using NorisWin32Update.ManifestProcessing;
using System.Threading.Tasks;
using WSUpdate;
using Microsoft.Win32;
using System.ServiceModel;

namespace NorisWin32Update
{
    /// <summary> A client/plugin files updater. </summary>
	public class Downloader
    {
        private const string PluginSubDir = "Plugins";
        private const string _ManifestUpdateXsdSchemaUrl = "http://helios.eu/ClientUpdateManifest.xsd";

        internal const string LogItemSubKey_FileName = "FileName";
        internal const string LogItemSubKey_Downloaded = "Downloaded";
        internal const string LogItemSubKey_Total = "Total";
        internal const string LogItemSubKey_Status = "Status";

        private readonly ClientUpdateSoapClient updateClient;
        private readonly string tempDir;
        private readonly string installDir;
        private readonly string applicationServer;

        public Downloader(string tempDir, string installDir, string applicationServer)
        {
            this.tempDir = tempDir;
            this.installDir = installDir;
            this.applicationServer = applicationServer;

            updateClient = new ClientUpdateSoapClient(ClientUpdateSoapClient.EndpointConfiguration.ClientUpdateSoap);
            updateClient.Endpoint.Address = new EndpointAddress(applicationServer + "/ClientUpdate.asmx");
        }

        /// <summary>
        /// Zápis do GUI pro uživatele
        /// </summary>
        /// <param name="imageIndex">Index ikonky na řádku</param>
        /// <param name="fileName">Jméno souboru ve sloupci (Soubor)</param>
        /// <param name="downloaded">Velikost stažení</param>
        /// <param name="total">Celková velikost</param>
        /// <param name="status">Stav stahování</param>
        /// <returns>Vytvořený vizuální item reprezentující právě vložený řádek do GUI</returns>
        internal static string _WriteLog(int imageIndex, string fileName, int downloaded, int total, string status)
        {
            Console.WriteLine($"{fileName} {downloaded}/{total} {status}");
            return Guid.NewGuid().ToString();
        }

        internal static void _updateLogItemImageIndex(string logItemKey, int imageIndex)
        {

        }

        internal static void _updateLogItemSubItem(string logItemKey, string subItemKey, string newValue)
        {

        }

        /// <summary>
        /// Samotný proces aktualizace klienta a jeho pluginů
        /// </summary>
        public async Task DownloadNowAsync()
        {
            Globals.Init(applicationServer, "SK", installDir);

            var manifestFileAccessor = new ClientManifestFileAccessor(installDir, updateClient);
            var updateManifestContent = await manifestFileAccessor.ReadUpdateManifestFileContent();

            if (!_CheckEnvironmentVersion(updateManifestContent))
                return;

            var currentManifestParser = new UpdateManifestParser(tempDir, installDir);
            ClientUpdateManifest currentManifest = null;

            if (manifestFileAccessor.ExistsCurrentManifestFile)
            {
                string currentManifestContent = manifestFileAccessor.ReadCurrentManifestFileContent();
                currentManifest = currentManifestParser.ParseManifest(currentManifestContent);
            }

            var updateManifestParser = new UpdateManifestParser(tempDir, installDir);
            var updateManifest = updateManifestParser.ParseManifest(updateManifestContent);

            var processor = new UpdateProcessorService(tempDir, installDir, currentManifest, updateManifest);
            var executePlan = processor.CreateExecutePlan(updateClient);
            await _ProcessingUpdateManifest(updateManifest, executePlan);

            manifestFileAccessor.WriteCurrentManifest(updateManifestContent);
        }

        private bool _CheckEnvironmentVersion(string updateManifestContent)
        {
            StringReader sr = new StringReader(updateManifestContent);
            System.Xml.XPath.XPathDocument xDoc = new System.Xml.XPath.XPathDocument(sr);
            System.Xml.XPath.XPathNavigator xNav = xDoc.CreateNavigator();
            XmlNamespaceManager nsmgr = new XmlNamespaceManager(xNav.NameTable);
            nsmgr.AddNamespace("mu", _ManifestUpdateXsdSchemaUrl);
            bool result = true;
            System.Xml.XPath.XPathNavigator node = xNav.SelectSingleNode("/mu:UpdateManifest/mu:EnvironmentVersion", nsmgr);
            if (node != null)
            {// v update manifestu je informace o podporovanem rozhraní
                Version supportedVersion = new Version(node.GetAttribute("supportedNETRuntime", string.Empty));
                string url = node.GetAttribute("supportedNETRuntimeUrl", string.Empty);
                RuntimeVersionDetector detector = new RuntimeVersionDetector();
                if (!detector.CheckRuntimeVersion(supportedVersion))
                {
                    result = false;
                }
            }
            return result;
        }

        private async System.Threading.Tasks.Task<bool> _ProcessingUpdateManifest(ClientUpdateManifest updateManifest, IEnumerable<UpdateProcessorService.ExecutePlanItem> executePlan)
        {
            var execPlan = new List<UpdateProcessorService.ExecutePlanItem>(executePlan);
            var planItemsCount = execPlan.Count;
            for (int i = 0; i < planItemsCount; i++)
            {
                var item = execPlan[i];
                switch (item.Action)
                {
                    case UpdateProcessorService.ExecutePlanItemAction.StartClient:
                        _WriteLog(-1, "Start Client", i, planItemsCount, string.Empty);
                        break;
                    case UpdateProcessorService.ExecutePlanItemAction.EndClient:
                        _WriteLog(-1, "End Client", i, planItemsCount, string.Empty);
                        break;
                    case UpdateProcessorService.ExecutePlanItemAction.StartPlugin:
                        _WriteLog(-1, "Start Plugin: (" + item.PluginIdentity.Author + ", " + item.PluginIdentity.Name + ")", i, planItemsCount, string.Empty);
                        break;
                    case UpdateProcessorService.ExecutePlanItemAction.EndPlugin:
                        _WriteLog(-1, "End Plugin: (" + item.PluginIdentity.Author + ", " + item.PluginIdentity.Name + ")", i, planItemsCount, string.Empty);
                        break;
                    case UpdateProcessorService.ExecutePlanItemAction.UpdateFile:
                        var logItemKey = _WriteLog(-1, Path.GetFileName(item.TargetFileName), i, planItemsCount, string.Empty);
                        if (await _LoadFileAsync(item, logItemKey))
                            _updateLogItemImageIndex(logItemKey, 1);
                        else
                        {
                            _updateLogItemImageIndex(logItemKey, 2);
                            return false;
                        }
                        break;
                    case UpdateProcessorService.ExecutePlanItemAction.DeleteFile:
                        _WriteLog(-1, $"Delete File: {item.TargetFileName}", i, planItemsCount, string.Empty);
                        File.Delete(item.TargetFileName);
                        break;
                    case UpdateProcessorService.ExecutePlanItemAction.DeletePlugin:
                        _WriteLog(-1, "Delete Plugin: (" + item.PluginIdentity.Author + ", " + item.PluginIdentity.Name + ")", i, planItemsCount, string.Empty);
                        _deletePlugin(item.ClientIdentity, item.PluginIdentity);
                        break;
                    case UpdateProcessorService.ExecutePlanItemAction.RunOnBeforeInstall:
                        _runPluginAction("BeforeInstall", item.ClientIdentity, item.PluginIdentity, item.TargetFileName);
                        break;
                    case UpdateProcessorService.ExecutePlanItemAction.RunOnAfterInstall:
                        _runPluginAction("AfterInstall", item.ClientIdentity, item.PluginIdentity, item.TargetFileName);
                        break;
                    case UpdateProcessorService.ExecutePlanItemAction.RunOnBeforeUpdate:
                        _runPluginAction("BeforeUpdate", item.ClientIdentity, item.PluginIdentity, item.TargetFileName);
                        break;
                    case UpdateProcessorService.ExecutePlanItemAction.RunOnAfterUpdate:
                        _runPluginAction("AfterUpdate", item.ClientIdentity, item.PluginIdentity, item.TargetFileName);
                        break;
                    case UpdateProcessorService.ExecutePlanItemAction.RunOnBeforeUninstall:
                        _runPluginAction("BeforeUninstall", item.ClientIdentity, item.PluginIdentity, item.TargetFileName);
                        break;
                    case UpdateProcessorService.ExecutePlanItemAction.RunOnAfterUninstall:
                        _runPluginAction("AfterUninstall", item.ClientIdentity, item.PluginIdentity, item.TargetFileName);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            _DeleteUnusedPluginsDirectory(updateManifest);
            return true;
        }

        private void _runPluginAction(string commandType, ClientIdentityNode clientIdentity, PluginIdentityNode pluginIdentity, string command)
        {
            if (string.IsNullOrEmpty(command)) return; // nemam zadny prikaz pro vykonani

            var oldCurrentDir = Environment.CurrentDirectory;
            var logItemKey = _WriteLog(-1, "Plugin: (" + pluginIdentity.Author + ", " + pluginIdentity.Name + ") run action «" + commandType + "»", 0, 0, string.Empty);
            try
            {
                var pluginDir = pluginIdentity.GetPluginDirectory(pluginIdentity.PluginsTargetDirectory);
                if (System.IO.Directory.Exists(pluginDir))
                {
                    Environment.CurrentDirectory = pluginDir;
                }
                if (!command.Contains(Path.DirectorySeparatorChar.ToString()))
                {
                    command = Path.Combine(pluginDir, command);
                }
                if (command.Contains(" "))
                {
                    command = "\"" + command + "\"";
                }
                var p = Process.Start(command, string.Format("/Action:{0} /PluginDirectory:\"{1}\"", commandType, pluginDir));
                if (p != null)
                {
                    p.WaitForExit();
                }
                _updateLogItemImageIndex(logItemKey, 1);
            }
            catch (Exception ex)
            {
                _updateLogItemImageIndex(logItemKey, 2);
                var sEx = new Exception("Plugin: (" + pluginIdentity.Author + ", " + pluginIdentity.Name + ") fail on «" + commandType + "» Error: " + ex.Message, ex);
                _WriteLog(2, sEx.Message, 0, 0, string.Empty);
                throw sEx;
            }
            finally
            {
                Environment.CurrentDirectory = oldCurrentDir;
            }
        }

        private void _deletePlugin(ClientIdentityNode clientIdentity, PluginIdentityNode pluginIdentity)
        {
            var directory = pluginIdentity.GetPluginDirectory(pluginIdentity.PluginsTargetDirectory);
            Directory.Delete(directory, true); // smazani cele struktury adresare pluginu

            // smazani nadrizeneho adresare pokud je prazdny
            var parentDirectory = new DirectoryInfo(directory).Parent;
            if (parentDirectory != null && parentDirectory.Exists && parentDirectory.GetFileSystemInfos().Length == 0)
            {
                parentDirectory.Delete(); // smazani adresare autora pluginu, zadny dalsi plugin od tohoto autora uz neni k dispozici
            }
        }

        //DAVID.CHARVAT 0044384 02.04.2013 Odinstalace klientských pluginů
        private void _DeleteUnusedPluginsDirectory(ClientUpdateManifest updateManifest)
        {
            string rootDir = installDir;
            string pluginsRootDir = System.IO.Path.Combine(rootDir, PluginSubDir);
            if (Directory.Exists(pluginsRootDir) == false) return;

            var usedDirectories = new Dictionary<string, object>();
            foreach (var plugin in updateManifest.Plugins)
            {
                var dir = plugin.Identity.GetPluginDirectory(plugin.Identity.PluginsTargetDirectory);
                usedDirectories.Add(dir, true);
            }

            foreach (string pluginAuthorDir in System.IO.Directory.GetDirectories(pluginsRootDir))
            {
                string pluginDir = System.IO.Path.Combine(pluginsRootDir, pluginAuthorDir);

                foreach (string pluginNameDir in System.IO.Directory.GetDirectories(pluginDir))
                {
                    if (!usedDirectories.ContainsKey(pluginNameDir))
                    {
                        Directory.Delete(pluginNameDir, true); // nepouzivany adresar pluginu klienta
                    }
                }
            }

            if (Directory.Exists(pluginsRootDir) && System.IO.Directory.GetDirectories(pluginsRootDir).Length == 0)
            {
                Directory.Delete(pluginsRootDir, true); // nemam zadne pluginy - nepotrebuji adresar plugins, vymazu ho cely
            }
        }

        private bool _ignoreBadRequestDownload(string targetFileName)
        {
            var updateFileExe = typeof(Downloader).Assembly.Location;
            if (targetFileName == updateFileExe) return true;

            var updateFile = System.IO.Path.GetFileNameWithoutExtension(updateFileExe);
            var updateDirectory = System.IO.Path.GetDirectoryName(updateFileExe);
            var updateFilePdb = System.IO.Path.Combine(updateDirectory, updateFile + ".pdb");
            if (targetFileName == updateFilePdb) return true;

            var updateFileConfig = updateFileExe + ".config";
            if (targetFileName == updateFileConfig) return true;
            return false;
        }

        private async Task<bool> _LoadFileAsync(UpdateProcessorService.ExecutePlanItem execItem, string logItemKey)
        {
            string targetName = execItem.TargetFileName;

            if (execItem.File.NodeType != NodeType.DirectUpdate) // shadow soubory generuji jen pokud nebezim z temp adresare
            {// jedna se o shadowFile
                //targetName += ShadowFileExtension; // generovani .shadow souboru uz nepodporujeme, klient nema pravo s takovym souborem neco udelat
                if (_ignoreBadRequestDownload(targetName))
                {// narazil jsem na pozadavek aktualizovat sama sebe, tedy aktualizacni program. To musim ignorovat, protoze z nejakeho duvodu jedu bez temp adresare, tak nemohu sam sebe prespat
                    return true;
                }
            }

            //DCH 0059422 16.07.2018 preprogramoveno na providery pro jednotlive tipy sluzeb. Nove

            FileDownloadProvider downloadProvider;
            if (Globals.UseBASHX)
            {
                throw new Exception("UseBASHX not supported yet");
                //downloadProvider = new FileDownloadProviderBASHX(targetName, execItem, logItemKey);
            }
            else
            {
                downloadProvider = new FileDownloadProviderWS(targetName, execItem, logItemKey, updateClient);
            }
            return await downloadProvider.DownloadAsync();
        }
    }

    /// <summary>
    /// Download one file from server side to client side  via WebService. Support zip and download view chunks.
    /// </summary>
    public abstract class FileDownloadProvider
    {
        public string TargetFileName { get; }

        public string DownloadTargetFileName { get; }
        public UpdateProcessorService.ExecutePlanItem ExecutePlanItem { get; }
        public string LogItemKey { get; }

        public FileDownloadProvider(string targetFileName, UpdateProcessorService.ExecutePlanItem executePlanItem, string logItemKey)
        {
            TargetFileName = targetFileName;
            ExecutePlanItem = executePlanItem;
            LogItemKey = logItemKey;
            DownloadTargetFileName = TargetFileName + ".part";
        }

        protected const string BaseFlags = "zip=2"; // vzdy chci dostat zip verze 2
        protected const string ChunkFlag = "Chunk=1";
        protected const string ChunkIndexFlagKey = "ChunkIndex=";
        protected const string ChunkSizeFlagKey = "ChunkSize=";
        protected const string ChunkStartKey = "ChunkStart=";
        protected const string ChunkContinueKey = "ChunkContinue=";
        protected const string ChunkEndKey = "ChunkEnd=";
        protected const int DownloadBufferSize = 3 * 1024 * 1024; // 3MB

        internal async Task<bool> DownloadAsync()
        {
            System.IO.FileInfo fi = new System.IO.FileInfo(ExecutePlanItem.TargetFileName);
            Directory.CreateDirectory(Path.GetDirectoryName(TargetFileName));
            var indetermineStatusText = string.Empty;
            bool indetermineStatusEnabled = true;
            var _indetermineIndicatorAnimationTask = System.Threading.Tasks.Task.Run(() =>
            {
                var pointChar = (Char)0x25CF;
                do
                {
                    if (indetermineStatusText.Length > 10)
                        indetermineStatusText = string.Empty;
                    indetermineStatusText += pointChar;
                    if (!indetermineStatusEnabled) break;
                    Downloader._updateLogItemSubItem(LogItemKey, Downloader.LogItemSubKey_Status, indetermineStatusText);
                    Task.Delay(500).Wait();
                }
                while (indetermineStatusEnabled);
                Downloader._updateLogItemSubItem(LogItemKey, Downloader.LogItemSubKey_Status, string.Empty);
            });

            var currentChunkIndex = 0;
            var success = true;
            long downloaded = 0;
            long total = 0;
            bool eof = false;
            bool zipUsed = false;
            System.IO.Stream downloadStream = null;
            string timeCheck = null;
            try
            {
                do
                {
                    var flags = $"{BaseFlags} {ChunkFlag} {ChunkIndexFlagKey}{currentChunkIndex} {ChunkSizeFlagKey}{DownloadBufferSize}";

                    var fd = await CallDownloadRequest(flags);


                    if (fd.ErrorMessage != null && fd.ErrorMessage != "")
                    {
                        success = false;
                        break;
                    }

                    if (fd == null || fd.Name == string.Empty)
                    {
                        success = false;
                        break;
                    }

                    if (fd == null)
                    {
                        success = false;
                        break;
                    }


                    bool chunkResponse = false;
                    if (fd.Flags.Contains($" {ChunkStartKey}")) // this is first part of requested file
                    {// new application server with support chunks download
                        timeCheck = _readTimeCheck(fd.Flags, ChunkStartKey);
                        chunkResponse = true;
                        var innerStream = System.IO.File.Open(DownloadTargetFileName, FileMode.Create, FileAccess.Write, FileShare.Read);
                        downloadStream = new System.IO.BufferedStream(innerStream, DownloadBufferSize);
                        currentChunkIndex++;
                        zipUsed = fd.ZipUsed;
                        downloadStream.Write(fd.Data, 0, fd.Data.Length);
                    }
                    else if (fd.Flags.Contains($" {ChunkContinueKey}")) // this is second but not last part of requested file
                    {// new application server with support chunks download
                        chunkResponse = true;
                        var nCheck = _readTimeCheck(fd.Flags, ChunkContinueKey);
                        if (nCheck == timeCheck)
                        {
                            currentChunkIndex++;
                            zipUsed = fd.ZipUsed;
                            downloadStream.Write(fd.Data, 0, fd.Data.Length);
                        }
                        else
                        {//server file was modified, restart download
                            currentChunkIndex = 0;
                            downloadStream.Dispose();
                            downloadStream = null;
                            System.IO.File.Delete(DownloadTargetFileName);
                        }
                    }

                    // END muze prijit rovnou se START pokud je soubor mensi nez buffer
                    if (fd.Flags.Contains($" {ChunkEndKey}")) // this is last part of requested file
                    {// new application server with support chunks download
                        chunkResponse = true;
                        var nCheck = _readTimeCheck(fd.Flags, ChunkEndKey);
                        if (nCheck == timeCheck)
                        {
                            if (!fd.Flags.Contains($" {ChunkStartKey}")) // pokud to nebyl stav kdy start byl zaroven i end, pri kombinaci start-end v jedne odpovedi provadi zapis uz if pro start segment
                                downloadStream.Write(fd.Data, 0, fd.Data.Length);
                            else
                            {
                                // write already in if (fd.Flags.Contains($" {ChunkStartKey}")... branche in this method
                                // read first part of file is already EOF, no need call server again for next part
                            }

                            downloadStream.Flush();
                            downloadStream.Dispose();
                            downloadStream = null;
                            eof = true;
                            zipUsed = fd.ZipUsed;
                        }
                        else
                        {//server file was modified, restart download
                            currentChunkIndex = 0;
                            downloadStream.Dispose();
                            downloadStream = null;
                            System.IO.File.Delete(DownloadTargetFileName);
                        }
                    }
                    else if (!chunkResponse)
                    {// old application server without support chunks download, fd.Data contains all content
                        var innerStream = System.IO.File.Open(DownloadTargetFileName, FileMode.Create, FileAccess.Write, FileShare.Read);
                        downloadStream = new System.IO.BufferedStream(innerStream, DownloadBufferSize);
                        downloadStream.Write(fd.Data, 0, fd.Data.Length);
                        eof = true;
                        zipUsed = fd.ZipUsed;
                        total = downloaded;
                    }

                    downloaded += fd.Data.Length;

                    //DCH 0059422 16.07.2018 Pokud bude v budoucnu server posilat jednotlive casti jako samostatne ZIP soubory a ne jako casti velkeho ZIP souboru jako ted tak zde by se mel udelat Unzip a patricne upravit Total
                    total = downloaded;

                    //downloaded size
                    Downloader._updateLogItemSubItem(LogItemKey, Downloader.LogItemSubKey_Total, _humanizeFileSize(total));
                    Downloader._updateLogItemSubItem(LogItemKey, Downloader.LogItemSubKey_Downloaded, _humanizeFileSize(downloaded));
                }
                while (!eof);
            }
            catch (Exception ex)
            {
                throw;
            }
            finally
            {
                indetermineStatusEnabled = false;
                downloadStream?.Dispose();
                downloadStream = null;
            }

            // when fail we only delete downloaded file
            if (!success && System.IO.File.Exists(DownloadTargetFileName))
            {
                System.IO.File.Delete(DownloadTargetFileName);
                return success;
            }

            // delete original target file before move/rename downloaded file to target file
            if (success)
            {
                //downloaded size
                Downloader._updateLogItemSubItem(LogItemKey, Downloader.LogItemSubKey_Downloaded, _humanizeFileSize(new System.IO.FileInfo(DownloadTargetFileName).Length));

                if (fi.Exists)
                {
                    try
                    {
                        System.IO.FileAttributes fa = fi.Attributes & ~System.IO.FileAttributes.ReadOnly;
                        System.IO.File.SetAttributes(fi.FullName, fa); //reset readonly attribute
                        fi.Delete();
                    }
                    catch
                    {   //error? wait a while and try again
                        System.Threading.Thread.Sleep(3000);
                        try
                        {
                            System.IO.FileAttributes fa = fi.Attributes & ~System.IO.FileAttributes.ReadOnly;
                            System.IO.File.SetAttributes(fi.FullName, fa); //reset readonly attribute
                            fi.Delete();
                        }
                        catch (System.UnauthorizedAccessException)
                        {
                            success = false;
                        }
                    }
                }
            }

            // if all ok then move/rename downloaded file to targetfile
            if (success)
            {
                if (zipUsed)
                {
                    using (var sourceFs = File.Open(DownloadTargetFileName, FileMode.Open, FileAccess.Read))
                    using (var sourceBs = new System.IO.BufferedStream(sourceFs, 1024 * 1024))
                    using (var targetFs = System.IO.File.Open(TargetFileName, FileMode.Create, FileAccess.Write))
                    using (var targetBs = new System.IO.BufferedStream(targetFs, 1024 * 1024))
                    {
                        Globals.UnzipFromStreamToStream(sourceBs, targetBs);
                    }
                    System.IO.File.Delete(DownloadTargetFileName);
                }
                else
                {
                    File.Move(DownloadTargetFileName, TargetFileName);
                }

                //total size
                Downloader._updateLogItemSubItem(LogItemKey, Downloader.LogItemSubKey_Total, _humanizeFileSize(new System.IO.FileInfo(TargetFileName).Length));
            }
            return success;
        }

        private string _readTimeCheck(string flags, string chunkTypeKey)
        {
            var keyLength = chunkTypeKey.Length;
            var startKey = flags.IndexOf(chunkTypeKey);
            var startValue = startKey + keyLength - 1;
            var endValue = flags.IndexOf(' ', startValue);
            if (endValue == -1)
                return flags.Substring(startValue);
            else
                return flags.Substring(startValue, endValue - startValue);
        }

        private string _humanizeFileSize(long fileSizeBytes)
        {
            var units = new[] { "kB", "MB", "GB" };
            var nextValue = fileSizeBytes;
            if (fileSizeBytes < 1000) return string.Format("{0} {1}", fileSizeBytes, "B");
            var unitIndex = 0;
            for (var i = 0; i < units.Length; i++)
            {
                nextValue = nextValue / 1024;
                if (nextValue <= 1000)
                {
                    unitIndex = i;
                    break;
                }
            }
            var result = string.Format("{0} {1}", nextValue, units[unitIndex]);
            return result;
        }

        protected abstract Task<WSUpdate.LoadClientFileDescriptor> CallDownloadRequest(string flags);
    }

    /// <summary>
    /// Download one file from server side to client side  via WebService. Support zip and download view chunks.
    /// </summary>
    public sealed class FileDownloadProviderWS : FileDownloadProvider
    {
        private readonly ClientUpdateSoapClient updateClient;

        public FileDownloadProviderWS(string targetFileName, UpdateProcessorService.ExecutePlanItem executePlanItem, string logItemKey, ClientUpdateSoapClient updateClient) 
            : base(targetFileName, executePlanItem, logItemKey)
        {
            this.updateClient = updateClient;
        }

        protected override async Task<LoadClientFileDescriptor> CallDownloadRequest(string flags)
        {
            if (ExecutePlanItem.PluginIdentity == null)
            {
                return await updateClient.LoadClientFileAsync(
                    ExecutePlanItem.ClientIdentity.Author,
                    ExecutePlanItem.ClientIdentity.Name,
                    ExecutePlanItem.File.FileName, flags);
            }
            else
            {
                return await updateClient.LoadPluginFileAsync(
                    ExecutePlanItem.ClientIdentity.Author, ExecutePlanItem.ClientIdentity.Name,
                    ExecutePlanItem.PluginIdentity.Author, ExecutePlanItem.PluginIdentity.Name,
                    ExecutePlanItem.File.FileName, flags);
            }
        }
    }


    public sealed class RuntimeVersionDetector
    {
        // zdroj informací: http://support.microsoft.com/kb/318785

        // Dodatek co microsoft neříká!
        // FW 2.0 SP 1 na WindowsXP a má jako verzi .net 2.2.30729
        // FW 2.0 SP 1 na Windows Vista a Windows 7 má jako verzi .net 2.0.50727.1433
        // FW 2.0 je na Windows 2003 nainstalován bez příznaku Install=1  !

        public sealed class RuntimeVersion
        {
            /// <summary>
            /// CZ: Zjištěná verze .net runtime
            /// </summary>
            public readonly Version Version;
            /// <summary>
            /// CZ: Aplikovaný ServicePack
            /// </summary>
            public readonly int? ServicePack;
            /// <summary>
            /// CZ: Instalovaný klientský profile?
            /// </summary>
            public readonly bool IsClientProfile;
            /// <summary>
            /// CZ: Instalovaný plný profil?
            /// </summary>
            public readonly bool IsFullProfile;

            public RuntimeVersion(Version version, int? servicePack, bool isClientProfile, bool isFullProfile)
            {
                Version = version;
                ServicePack = servicePack;
                IsClientProfile = isClientProfile;
                IsFullProfile = isFullProfile;
            }
        }

        /// <summary>
        /// CZ: Ověření zda je nainstalována podporovaná verze .NET Framework
        /// </summary>
        /// <param name="version">Podporovaná verze rozhraní</param>
        /// <returns>True pokud je platforma nainstalována, nebo je nainstalována některá z kompatibilních verzí.</returns>
        public bool CheckRuntimeVersion(Version version)
        {
            RuntimeVersion rv = new RuntimeVersion(version, null, true, true); //kontroluji jen verzi
            return _CheckRuntimeVersion(rv);
        }

        /// <summary>
        /// CZ: Ověření zda je nainstalována podporovaná verze .NET Framework s aplikovaným, nebo vyšším service packem
        /// </summary>
        /// <param name="version">Podporovaná verze rozhraní</param>
        /// <param name="servicePack">Požadována určitá verze instalovaného ServicePack</param>
        /// <returns>True pokud je platforma nainstalována, nebo je nainstalována některá z kompatibilních verzí. Zároveň je stejný a nebo vyšší ServicePack.</returns>
        public bool CheckRuntimeVersion(Version version, int servicePack)
        {
            RuntimeVersion rv = new RuntimeVersion(version, servicePack, true, true); //kontroluji verzi a instalovaný SP
            return _CheckRuntimeVersion(rv);
        }

        /// <summary>
        /// CZ: Ověření zda je nainstalována podporovaná verze .NET Framework s aplikovaným, nebo vyšším service packem a yda je instalov8n jen klientský profil namísto plného.
        /// </summary>
        /// <param name="version">Podporovaná verze rozhraní</param>
        /// <param name="servicePack">Požadována určitá verze instalovaného ServicePack</param>
        /// <param name="needOnlyClientProfile">True pokud požaduji jen instalovaný klientský profil. False pokud stačí, aby byl nainstalován jen plný profil.</param>
        /// <returns>True pokud je platforma nainstalována, nebo je nainstalována, některá z kompatibilních verzí. Zároveň je stejný a nebo vyšší ServicePack a je instalován požadovaný profil.</returns>
        public bool CheckRuntimeVersion(Version version, int servicePack, bool needOnlyClientProfile)
        {
            RuntimeVersion rv = new RuntimeVersion(version, servicePack, needOnlyClientProfile, !needOnlyClientProfile); //kontroluji verzi a instalovaný SP a vyzaduji pouze klientský profil
            return _CheckRuntimeVersion(rv);
        }

        private bool _CheckRuntimeVersion(RuntimeVersion rv)
        {
            bool versionFound = false;
            List<RuntimeVersion> installedVersions = GetRuntimeInstalledVersions();
            foreach (RuntimeVersion v in installedVersions)
            {
                versionFound =
                    (v.Version.Major == rv.Version.Major && v.Version.Minor >= rv.Version.Minor) && // shodnost Major a Minor stejný a větší
                    (rv.Version.Build == -1 || v.Version.Minor > rv.Version.Minor || v.Version.Build >= rv.Version.Build) && // pokud je zadaný build a nebo je vetsi minor nebo je build vetsi nez pozadovany
                    (rv.Version.Revision == -1 || v.Version.Minor > rv.Version.Minor || v.Version.Revision >= rv.Version.Revision) && // pokud je zadaná revize a nebo je vetsi minor a nebo je revize vetsi nez pozadovana
                    (rv.ServicePack == null || v.ServicePack >= rv.ServicePack) && // service pack je zadaný a instalovaný je stejný a nebo větší
                    (rv.IsClientProfile == false || rv.IsClientProfile == v.IsClientProfile) && // je pozadovana instalace klient profile (FW 4.0 a vyšší) a opravdu tomu tak je
                    (rv.IsFullProfile == false || rv.IsFullProfile == v.IsFullProfile);// je pozadovaná instalace full profile (FW 4.0 a vyšší) a opravdu tomu tak je
                if (versionFound) // uz jsem nasel verzi, kterou hledam
                    break;
            }
            return versionFound;
        }

        /// <summary>
        /// CZ: Zjištění instalovaných verzí runtime .NET Framework
        /// </summary>
        /// <returns></returns>
        public List<RuntimeVersion> GetRuntimeInstalledVersions()
        {
            // algoritmus neumi zpracovat FW 1.0 a FW 1.1 nicméně ty nepotřebne takže to nevadí
            // zdroj informací: http://support.microsoft.com/kb/318785/en-us

            RegistryKey regKey = Registry.LocalMachine.OpenSubKey(
                "Software\\Microsoft\\NET Framework Setup\\NDP", false); // jen pro cteni, abych nepotreboval UAC elevation
            List<RuntimeVersion> versions = new List<RuntimeVersion>();
            foreach (string key in regKey.GetSubKeyNames())
            {
                if (key.StartsWith("v") && key.Length > 1 && char.IsNumber(key[1]))
                {
                    Microsoft.Win32.RegistryKey fwRegKey = regKey.OpenSubKey(key);
                    bool installed = (int)fwRegKey.GetValue("Install", 0) == 1;
                    if (installed)
                    {// daný framework je nainstalován
                        string vString = (string)fwRegKey.GetValue("Version", string.Empty);
                        if (!string.IsNullOrEmpty(vString))
                        {
                            Version v = new Version(vString);
                            int? sp = (int)fwRegKey.GetValue("SP", -1);
                            if (sp.Value == -1)
                                sp = null;

                            RuntimeVersion version = new RuntimeVersion(v, sp, true, true);
                            versions.Add(version);
                        }
                        else
                        {// nektere instalace nemaji vyplnenu verzi, ale jen increment a nekdy ani to, proto je tu tento blok
                            int? sp = (int)fwRegKey.GetValue("SP", -1);
                            if (sp.Value == -1)
                                sp = null;

                            string incrementString = (string)fwRegKey.GetValue("increment", string.Empty);
                            Version v = new Version(key.Substring(1) + (string.IsNullOrEmpty(incrementString) ? string.Empty : "." + incrementString));
                            RuntimeVersion version = new RuntimeVersion(v, sp, true, true);
                            versions.Add(version);
                        }
                    }
                    else
                    {// pro framework 4.0 a vyšší platí, že je distribuován jako dvě podverze Client profile a Full profile. V systému může nastat situace, že bude k dispozici jen jeden profil
                        Microsoft.Win32.RegistryKey fwRegClientKey = fwRegKey.OpenSubKey("Client");
                        Microsoft.Win32.RegistryKey fwRegFullKey = fwRegKey.OpenSubKey("Full");

                        bool installedClient = fwRegClientKey != null ? (int)fwRegClientKey.GetValue("Install", 0) == 1 : false;
                        bool installedFull = fwRegFullKey != null ? (int)fwRegFullKey.GetValue("Install", 0) == 1 : false;
                        if (installedClient || installedFull)
                        {
                            fwRegKey = installedClient ? fwRegClientKey : fwRegFullKey;
                            string vString = (string)fwRegClientKey.GetValue("Version", string.Empty);
                            if (!string.IsNullOrEmpty(vString))
                            {
                                Version v = new Version(vString);
                                int? sp = (int)fwRegKey.GetValue("SP", -1);
                                if (sp.Value == -1)
                                    sp = null;

                                RuntimeVersion version = new RuntimeVersion(v, sp, installedClient, installedFull);
                                versions.Add(version);
                            }
                        }
                    }
                }
            }
            return versions;
        }
    }
}
