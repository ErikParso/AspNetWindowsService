using AspWinService.ManifestProcessing;
using AspWinService.Tools;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.ServiceModel;
using System.Threading.Tasks;
using System.Xml;
using WSUpdate;

namespace AspWinService.Services
{
    public class DownloadService
    {
        private const string PluginSubDir = "Plugins";

        public const string LogItemSubKey_FileName = "FileName";
        public const string LogItemSubKey_Downloaded = "Downloaded";
        public const string LogItemSubKey_Total = "Total";
        public const string LogItemSubKey_Status = "Status";

        private readonly RuntimeVersionDetectorService runtimeVersionDetectorService;
        private readonly UpdateProcessorService updateProcessorService;
        private readonly ProgressService progressService;
        private readonly RedirectService redirectService;

        public DownloadService(
            RuntimeVersionDetectorService runtimeVersionDetectorService,
            UpdateProcessorService updateProcessorService,
            ProgressService progressService,
            RedirectService redirectService)
        {
            this.runtimeVersionDetectorService = runtimeVersionDetectorService;
            this.updateProcessorService = updateProcessorService;
            this.progressService = progressService;
            this.redirectService = redirectService;
        }

        public async Task DownloadClient(string processId, string tempDir, string installDir, string versionManagerAddress)
        {
            versionManagerAddress = versionManagerAddress.TrimEnd('/');
            var updateClient = await redirectService.GetUpdateClient(versionManagerAddress);

            var manifestFileAccessor = new ClientManifestFileAccessor(tempDir, installDir, updateClient);
            var updateManifestContent = await manifestFileAccessor.ReadUpdateManifestFileContent();

            if (!CheckEnvironmentVersion(updateManifestContent))
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

            var executePlan = updateProcessorService.CreateExecutePlan(updateClient, tempDir, installDir, currentManifest, updateManifest);
            await ProcessingUpdateManifest(processId, updateManifest, executePlan, installDir, updateClient);

            manifestFileAccessor.WriteCurrentManifest(updateManifestContent);
        }

        private bool CheckEnvironmentVersion(string updateManifestContent)
        {
            StringReader sr = new StringReader(updateManifestContent);
            System.Xml.XPath.XPathDocument xDoc = new System.Xml.XPath.XPathDocument(sr);
            System.Xml.XPath.XPathNavigator xNav = xDoc.CreateNavigator();
            XmlNamespaceManager nsmgr = new XmlNamespaceManager(xNav.NameTable);
            nsmgr.AddNamespace("mu", Constants.ManifestUpdateXsdSchemaUrl);
            bool result = true;
            System.Xml.XPath.XPathNavigator node = xNav.SelectSingleNode("/mu:UpdateManifest/mu:EnvironmentVersion", nsmgr);
            if (node != null)
            {
                Version supportedVersion = new Version(node.GetAttribute("supportedNETRuntime", string.Empty));
                string url = node.GetAttribute("supportedNETRuntimeUrl", string.Empty);
                if (!runtimeVersionDetectorService.CheckRuntimeVersion(supportedVersion))
                {
                    result = false;
                }
            }
            return result;
        }

        private async Task<bool> ProcessingUpdateManifest(
            string processId,
            ClientUpdateManifest updateManifest, 
            IEnumerable<UpdateProcessorService.ExecutePlanItem> executePlan,
            string installDir,
            ClientUpdateSoapClient updateClient)
        {
            var execPlan = new List<UpdateProcessorService.ExecutePlanItem>(executePlan);
            var planItemsCount = execPlan.Count;
            for (int i = 0; i < planItemsCount; i++)
            {
                var item = execPlan[i];
                switch (item.Action)
                {
                    case UpdateProcessorService.ExecutePlanItemAction.StartClient:
                        await progressService.WriteLog(processId, -1, "Start Client", i, planItemsCount, string.Empty);
                        break;
                    case UpdateProcessorService.ExecutePlanItemAction.EndClient:
                        await progressService.WriteLog(processId, -1, "End Client", i, planItemsCount, string.Empty);
                        break;
                    case UpdateProcessorService.ExecutePlanItemAction.StartPlugin:
                        await progressService.WriteLog(processId, -1, "Start Plugin: (" + item.PluginIdentity.Author + ", " + item.PluginIdentity.Name + ")", i, planItemsCount, string.Empty);
                        break;
                    case UpdateProcessorService.ExecutePlanItemAction.EndPlugin:
                        await progressService.WriteLog(processId , - 1, "End Plugin: (" + item.PluginIdentity.Author + ", " + item.PluginIdentity.Name + ")", i, planItemsCount, string.Empty);
                        break;
                    case UpdateProcessorService.ExecutePlanItemAction.UpdateFile:
                        var logItemKey = await progressService.WriteLog(processId, - 1, Path.GetFileName(item.TargetFileName), i, planItemsCount, string.Empty);
                        if (await _LoadFileAsync(item, logItemKey, updateClient))
                            progressService.UpdateLogItemImageIndex(logItemKey, 1);
                        else
                        {
                            progressService.UpdateLogItemImageIndex(logItemKey, 2);
                            return false;
                        }
                        break;
                    case UpdateProcessorService.ExecutePlanItemAction.DeleteFile:
                        await progressService.WriteLog(processId, - 1, $"Delete File: {item.TargetFileName}", i, planItemsCount, string.Empty);
                        File.Delete(item.TargetFileName);
                        break;
                    case UpdateProcessorService.ExecutePlanItemAction.DeletePlugin:
                        await progressService.WriteLog(processId, - 1, "Delete Plugin: (" + item.PluginIdentity.Author + ", " + item.PluginIdentity.Name + ")", i, planItemsCount, string.Empty);
                        _deletePlugin(item.PluginIdentity);
                        break;
                    case UpdateProcessorService.ExecutePlanItemAction.RunOnBeforeInstall:
                        await RunPluginAction(processId, "BeforeInstall", item.PluginIdentity, item.TargetFileName, i, planItemsCount);
                        break;
                    case UpdateProcessorService.ExecutePlanItemAction.RunOnAfterInstall:
                        await RunPluginAction(processId, "AfterInstall", item.PluginIdentity, item.TargetFileName, i, planItemsCount);
                        break;
                    case UpdateProcessorService.ExecutePlanItemAction.RunOnBeforeUpdate:
                        await RunPluginAction(processId, "BeforeUpdate", item.PluginIdentity, item.TargetFileName, i, planItemsCount);
                        break;
                    case UpdateProcessorService.ExecutePlanItemAction.RunOnAfterUpdate:
                        await RunPluginAction(processId, "AfterUpdate", item.PluginIdentity, item.TargetFileName, i, planItemsCount);
                        break;
                    case UpdateProcessorService.ExecutePlanItemAction.RunOnBeforeUninstall:
                        await RunPluginAction(processId, "BeforeUninstall", item.PluginIdentity, item.TargetFileName, i, planItemsCount);
                        break;
                    case UpdateProcessorService.ExecutePlanItemAction.RunOnAfterUninstall:
                        await RunPluginAction(processId, "AfterUninstall", item.PluginIdentity, item.TargetFileName, i, planItemsCount);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            DeleteUnusedPluginsDirectory(updateManifest, installDir);
            return true;
        }

        private async Task<bool> _LoadFileAsync(UpdateProcessorService.ExecutePlanItem execItem, string logItemKey, ClientUpdateSoapClient updateClient)
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

            var downloadProvider = new FileDownloadProviderWS(targetName, execItem, logItemKey, updateClient, progressService);
            return await downloadProvider.DownloadAsync();
        }

        private bool _ignoreBadRequestDownload(string targetFileName)
        {
            var updateFileExe = this.GetType().Assembly.Location;
            if (targetFileName == updateFileExe) return true;

            var updateFile = System.IO.Path.GetFileNameWithoutExtension(updateFileExe);
            var updateDirectory = System.IO.Path.GetDirectoryName(updateFileExe);
            var updateFilePdb = System.IO.Path.Combine(updateDirectory, updateFile + ".pdb");
            if (targetFileName == updateFilePdb) return true;

            var updateFileConfig = updateFileExe + ".config";
            if (targetFileName == updateFileConfig) return true;
            return false;
        }

        private void DeleteUnusedPluginsDirectory(ClientUpdateManifest updateManifest, string installDir)
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

        private void _deletePlugin(PluginIdentityNode pluginIdentity)
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

        private async Task RunPluginAction(string processId, string commandType, PluginIdentityNode pluginIdentity, string command, int current, int total)
        {
            if (string.IsNullOrEmpty(command)) return; // nemam zadny prikaz pro vykonani

            var oldCurrentDir = Environment.CurrentDirectory;
            var logItemKey = await progressService.WriteLog(processId, -1, "Plugin: (" + pluginIdentity.Author + ", " + pluginIdentity.Name + ") run action «" + commandType + "»", current, total, string.Empty);
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
                progressService.UpdateLogItemImageIndex(logItemKey, 1);
            }
            catch (Exception ex)
            {
                progressService.UpdateLogItemImageIndex(logItemKey, 2);
                var sEx = new Exception("Plugin: (" + pluginIdentity.Author + ", " + pluginIdentity.Name + ") fail on «" + commandType + "» Error: " + ex.Message, ex);
                await progressService.WriteLog(processId, 2, sEx.Message, 0, 0, string.Empty);
                throw sEx;
            }
            finally
            {
                Environment.CurrentDirectory = oldCurrentDir;
            }
        }
    }
}
