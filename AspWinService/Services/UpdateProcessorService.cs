// Supervisor:DAVID.CHARVAT
// Part of HELIOS Green, proprietary software, (c) Asseco Solution, a. s.
// Redistribution and use in source and binary forms, with or without modification,
// is not permitted without valid contract with Asseco Solution, a. s.

using AspWinService.ManifestProcessing;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using WSUpdate;

namespace AspWinService.Services
{
    public sealed class UpdateProcessorService
    {
        [System.Diagnostics.DebuggerDisplay("Client={ClientIdentity.Name}, Plugin={PluginIdentity}, Action={Action}, TargetFileName={TargetFileName}")]
        public class ExecutePlanItem
        {
            public ClientIdentityNode ClientIdentity { get; set; }
            public PluginIdentityNode PluginIdentity { get; set; }
            public FileNode File { get; set; }

            public string TargetFileName
            {
                get
                {
                    if (_targetFileName == null && File != null) return File.TargetFullPath;
                    return _targetFileName;
                }
                set { _targetFileName = value; }
            }

            private string _targetFileName;

            public ExecutePlanItemAction Action { get; set; }
        }

        public enum ExecutePlanItemAction
        {
            StartClient,
            EndClient,
            StartPlugin,
            EndPlugin,
            UpdateFile,
            DeleteFile,
            DeletePlugin,
            RunOnBeforeInstall,
            RunOnAfterInstall,
            RunOnBeforeUpdate,
            RunOnAfterUpdate,
            RunOnBeforeUninstall,
            RunOnAfterUninstall,
        }

        private List<ExecutePlanItem> _executePlan = new List<ExecutePlanItem>();

        public IEnumerable<ExecutePlanItem> CreateExecutePlan(
            ClientUpdateSoapClient updateClient, 
            string temporaryDirectory, 
            string targetDirectory, 
            ClientUpdateManifest currentManifest, 
            ClientUpdateManifest updateManifest)
        {
            _executePlan.Clear();

            // klientske soubory
            var clientLocalFiles = new List<string>(Directory.GetFiles(targetDirectory));

            // soubory v ignorovanych adresarich ignoruji, ze tam jsou, delam jako kdyby tam nic nebylo
            foreach (var dirNode in updateManifest.Directories.Where(r => r.NodeType == NodeType.NotUpdate))
            {
                var targetDirectoryName = dirNode.TargetFullPath;
                clientLocalFiles.RemoveAll(r => r.StartsWith(targetDirectoryName, StringComparison.OrdinalIgnoreCase));
            }

            var execPlan = new List<ExecutePlanItem>(_createExecutePlanForFilesCollection(clientLocalFiles, updateManifest.Files, null, updateManifest, currentManifest));
            // mam nejake opustene soubory?
            var currentManifestFileName = new ClientManifestFileAccessor(temporaryDirectory, targetDirectory, updateClient).CurrentManifestFullFileName;
            ExecutePlanItem item;
            foreach (var orphanedFile in clientLocalFiles)
            {
                if (orphanedFile == currentManifestFileName) continue; //current manifest file can't be deleted!

                item = new ExecutePlanItem { Action = ExecutePlanItemAction.DeleteFile, ClientIdentity = updateManifest.Identity, TargetFileName = orphanedFile };
                execPlan.Add(item);
            }

            if (execPlan.Count > 0)
            {
                item = new ExecutePlanItem { Action = ExecutePlanItemAction.StartClient, ClientIdentity = updateManifest.Identity };
                _executePlan.Add(item);
                _executePlan.AddRange(execPlan);
                item = new ExecutePlanItem { Action = ExecutePlanItemAction.EndClient, ClientIdentity = updateManifest.Identity };
                _executePlan.Add(item);
            }

            foreach (var plugin in updateManifest.Plugins)
            {
                var pluginDirectory = plugin.Identity.GetPluginDirectory(plugin.Identity.PluginsTargetDirectory);
                
                var pluginLocalFiles = Directory.Exists(pluginDirectory) ? new List<string>(_getLocalFiles(pluginDirectory)) : new List<string>();
                // soubory v ignorovanych adresarich ignoruji, ze tam jsou, delam jako kdyby tam nic nebylo
                foreach (var dirNode in plugin.Directories.Where(r => r.NodeType == NodeType.NotUpdate))
                {
                    var targetDirectoryName = dirNode.TargetFullPath;
                    pluginLocalFiles.RemoveAll(r => r.StartsWith(targetDirectoryName, StringComparison.OrdinalIgnoreCase));
                }


                var newInstall = pluginLocalFiles.Count == 0;
                execPlan = new List<ExecutePlanItem>(_createExecutePlanForFilesCollection<PluginFileNode>(pluginLocalFiles, plugin.Files, plugin.Identity, updateManifest, currentManifest));

                if (execPlan.Count == 0 && pluginLocalFiles.Count > 0)
                {// zadne kroky jen odebrani souboru povazuji to za update
                    item = new ExecutePlanItem { Action = ExecutePlanItemAction.StartPlugin, ClientIdentity = updateManifest.Identity, PluginIdentity = plugin.Identity };
                    _executePlan.Add(item);

                    var beforeItem = new ExecutePlanItem { Action = ExecutePlanItemAction.RunOnBeforeUpdate, ClientIdentity = updateManifest.Identity, PluginIdentity = plugin.Identity, TargetFileName = plugin.Actions.OnBeforeUpdate };
                    var afterItem = new ExecutePlanItem { Action = ExecutePlanItemAction.RunOnAfterUpdate, ClientIdentity = updateManifest.Identity, PluginIdentity = plugin.Identity, TargetFileName = plugin.Actions.OnAfterUpdate };
                    _executePlan.Add(beforeItem);
                    foreach (var orphanedFile in pluginLocalFiles)
                    {
                        item = new ExecutePlanItem { Action = ExecutePlanItemAction.DeleteFile, ClientIdentity = updateManifest.Identity, TargetFileName = orphanedFile, PluginIdentity = plugin.Identity };
                        _executePlan.Add(item);
                    }
                    _executePlan.Add(afterItem);

                    item = new ExecutePlanItem { Action = ExecutePlanItemAction.EndPlugin, ClientIdentity = updateManifest.Identity, PluginIdentity = plugin.Identity };
                    _executePlan.Add(item);
                }

                if (execPlan.Count <= 0) continue; // bude instalace, nebo aktualizace pluginu
                
                item = new ExecutePlanItem { Action = ExecutePlanItemAction.StartPlugin, ClientIdentity = updateManifest.Identity, PluginIdentity = plugin.Identity };
                _executePlan.Add(item);
                // instalace bude pokud zadny soubor pluginu dle manifestu neexistuje lokalne
                if (newInstall) // instalace
                {   
                    var beforeItem = new ExecutePlanItem { Action = ExecutePlanItemAction.RunOnBeforeInstall, ClientIdentity = updateManifest.Identity, PluginIdentity = plugin.Identity, TargetFileName = plugin.Actions.OnBeforeInstall};
                    var afterItem = new ExecutePlanItem { Action = ExecutePlanItemAction.RunOnAfterInstall, ClientIdentity = updateManifest.Identity, PluginIdentity = plugin.Identity, TargetFileName = plugin.Actions.OnAfterInstall };
                    _executePlan.Add(beforeItem);
                    _executePlan.AddRange(execPlan);
                    _executePlan.Add(afterItem);
                }
                else // update
                {
                    var beforeItem = new ExecutePlanItem { Action = ExecutePlanItemAction.RunOnBeforeUpdate, ClientIdentity = updateManifest.Identity, PluginIdentity = plugin.Identity, TargetFileName = plugin.Actions.OnBeforeUpdate };
                    var afterItem = new ExecutePlanItem { Action = ExecutePlanItemAction.RunOnAfterUpdate, ClientIdentity = updateManifest.Identity, PluginIdentity = plugin.Identity, TargetFileName = plugin.Actions.OnAfterUpdate };
                    _executePlan.Add(beforeItem);
                    _executePlan.AddRange(execPlan);
                    foreach (var orphanedFile in pluginLocalFiles)
                    {
                        item = new ExecutePlanItem { Action = ExecutePlanItemAction.DeleteFile, ClientIdentity = updateManifest.Identity, TargetFileName = orphanedFile, PluginIdentity = plugin.Identity };
                        _executePlan.Add(item);
                    }
                    _executePlan.Add(afterItem);
                }
                item = new ExecutePlanItem { Action = ExecutePlanItemAction.EndPlugin, ClientIdentity = updateManifest.Identity, PluginIdentity = plugin.Identity };
                _executePlan.Add(item);
            }

            // smazani adresaru pluginu a autoru pluginu, ktere nejsou v aktualizacnim manifestu
            if (currentManifest != null)
            {// tato moznost je jen pokud mam k dispozici puvodni manifest klienta
                var pluginsToDelete = new List<PluginUpdateManifest>();
                foreach (var plugin in currentManifest.Plugins)
                {
                    var found = false;
                    foreach (var updPlugin in updateManifest.Plugins)
                    {
                        if (updPlugin.Identity.Equals(plugin.Identity))
                        {
                            found = true;
                            break;
                        }
                    }
                    
                    if (found == false)
                    {
                        pluginsToDelete.Add(plugin);
                    }
                }

                foreach (var plugin in pluginsToDelete)
                {
                    var beforeItem = new ExecutePlanItem { Action = ExecutePlanItemAction.RunOnBeforeUninstall, ClientIdentity = updateManifest.Identity, PluginIdentity = plugin.Identity, TargetFileName = plugin.Actions.OnBeforeUninstall };
                    var afterItem = new ExecutePlanItem { Action = ExecutePlanItemAction.RunOnAfterUninstall, ClientIdentity = updateManifest.Identity, PluginIdentity = plugin.Identity, TargetFileName = plugin.Actions.OnAfterUninstall };
                    _executePlan.Add(beforeItem);
                    item = new ExecutePlanItem { Action = ExecutePlanItemAction.DeletePlugin, ClientIdentity = updateManifest.Identity, PluginIdentity = plugin.Identity };
                    _executePlan.Add(item);
                    _executePlan.Add(afterItem);
                }
            }
            return _executePlan;
        }

        private IEnumerable<string> _getLocalFiles(string targetDirectory)
        {
            foreach (var fileName in System.IO.Directory.GetFiles(targetDirectory))
            {
                yield return fileName;
            }

            foreach (var subDir in System.IO.Directory.GetDirectories(targetDirectory))
            {
                foreach (var fileName in _getLocalFiles(subDir))
                {
                    yield return fileName;
                }
            }
        }

        private IEnumerable<ExecutePlanItem> _createExecutePlanForFilesCollection<TFileNode>(
            List<string> localFiles, 
            IEnumerable<TFileNode> updateFiles, 
            PluginIdentityNode pluginIdentity,
            ClientUpdateManifest updateManifest,
            ClientUpdateManifest currentManifest)
            where TFileNode : FileNode
        {
            foreach (var fileNode in updateFiles)
            {
                switch (fileNode.NodeType)
                {
                    case NodeType.DirectUpdate:
                    case NodeType.ShadowUpdate:
                        var currentFileNode = GetCurrentManifestFileNode(updateManifest.Identity, pluginIdentity, fileNode, currentManifest);
                        if (currentFileNode == null //soubor neni v ramci current manifestu dohledan
                            || System.IO.File.Exists(fileNode.TargetFullPath) == false // soubor neni na disku v cilovem miste
                            || currentFileNode.Crc != fileNode.Crc) // soubor nema shodne CRC
                        {
                            yield return new ExecutePlanItem { Action = ExecutePlanItemAction.UpdateFile, ClientIdentity = updateManifest.Identity, File = fileNode, PluginIdentity = pluginIdentity };
                        }
                        break;
                    case NodeType.NotUpdate:
                        break;
                    default:
                        throw new NotSupportedException(fileNode.NodeType.ToString());
                }

                //David.Charvat 0050054 30.04.2015 Chyba aktualizace klienta
                // nelze pouzit Remove protoze je CS a ja potrebuji CI
                //localFiles.Remove(fileNode.TargetFullPath); // zname soubory si odeberu a nakonec budu mit jen seznam souboru, ktere jsou opustene a nejsou v zadnem manifestu

                //CI algoritmus dohledani a odebrani z localFiles
                var localFNames = localFiles.FindAll(s => s.Equals(fileNode.TargetFullPath, StringComparison.OrdinalIgnoreCase));
                foreach (var lf in localFNames)
                {
                    localFiles.Remove(lf);
                }
            }
        }

        private FileNode GetCurrentManifestFileNode(
            ClientIdentityNode clientIdentity, 
            PluginIdentityNode pluginIdentity, 
            FileNode updateNode,
            ClientUpdateManifest currentManifest)
        {
            if (currentManifest == null) return null;
            if (pluginIdentity == null)
            {// client soubor
                foreach (var file in currentManifest.Files)
                {
                    if (file.TargetFullPath == updateNode.TargetFullPath)
                    {
                        return file;
                    }
                }
            }
            // plugin soubor

            PluginUpdateManifest currentPlugin;
            if (currentManifest.TryGetPlugin(pluginIdentity, out currentPlugin))
            {
                foreach (var file in currentPlugin.Files)
                {
                    if (file.TargetFullPath == updateNode.TargetFullPath)
                    {
                        return file;
                    }
                }
            }
            return null;
        }
    }
}
