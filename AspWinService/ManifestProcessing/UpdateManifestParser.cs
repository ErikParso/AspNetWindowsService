// Supervisor:DAVID.CHARVAT
// Part of HELIOS Green, proprietary software, (c) Asseco Solution, a. s.
// Redistribution and use in source and binary forms, with or without modification,
// is not permitted without valid contract with Asseco Solution, a. s.

using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace AspWinService.ManifestProcessing
{
    public sealed class UpdateManifestParser
    {
        /// <summary> URL of the client manifest update XSD schema namespace. </summary>
        public static readonly string ManifestUpdateXsdSchemaUrl = "http://helios.eu/ClientUpdateManifest.xsd";

        /// <summary> Gets or sets the pathname of the temporary directory. </summary>
        /// <value> The pathname of the temporary directory. </value>
        public string TemporaryDirectory { get; private set; }

        /// <summary> Gets or sets the pathname of the target directory. </summary>
        /// <value> The pathname of the target directory. </value>
        public string TargetDirectory { get; private set; }

        public const string PluginsDirectoryName = "Plugins";

        /// <summary> Constructor. </summary>
        /// <param name="temporaryDirectory"> Pathname of the client temporary directory. </param>
        /// <param name="targetDirectory">    Pathname of the client target directory. </param>
        public UpdateManifestParser(string temporaryDirectory, string targetDirectory)
        {
            TemporaryDirectory = temporaryDirectory;
            TargetDirectory = targetDirectory;
        }

        /// <summary> Parse manifest. </summary>
        /// <param name="content"> The content. </param>
        /// <returns> A ClientUpdateManifest. </returns>
        public ClientUpdateManifest ParseManifest(string content)
        {
            ClientIdentityNode manifestIdentity;
            IEnumerable<ClientFileNode> clientFiles;
            IEnumerable<ClientDirectoryNode> clientDirectories; //DCH 0055082 06.03.2017 
            IEnumerable<PluginUpdateManifest> plugins;
            using (var contentStream = new StringReader(content))
            {
                using (var reader = XmlReader.Create(contentStream))
                {
                    System.Xml.XPath.XPathDocument xDoc = new System.Xml.XPath.XPathDocument(reader);
                    System.Xml.XPath.XPathNavigator xNav = xDoc.CreateNavigator();
                    XmlNamespaceManager nsmgr = new XmlNamespaceManager(xNav.NameTable);
                    nsmgr.AddNamespace("mu", ManifestUpdateXsdSchemaUrl);

                    var identityElement = xNav.SelectSingleNode("/mu:UpdateManifest/mu:Identity", nsmgr);
                    manifestIdentity = _parseClientManifestIdentity(identityElement);
                    var filesElement = xNav.SelectSingleNode("/mu:UpdateManifest/mu:Files", nsmgr); ;
                    clientFiles = _parseFiles(filesElement, (nodeType, name, crc) => new ClientFileNode(TemporaryDirectory, TargetDirectory, nodeType, name, crc), nsmgr);

                    //DCH 0055082 06.03.2017
                    clientDirectories = _parseDirectories<ClientDirectoryNode>(filesElement,
                        (nodeType, name) => new ClientDirectoryNode(TemporaryDirectory, TargetDirectory, nodeType, name),
                        nsmgr);
                    plugins = _processingPlugins(manifestIdentity, xNav.SelectSingleNode("/mu:UpdateManifest/mu:Plugins", nsmgr), nsmgr);
                }
            }
            return new ClientUpdateManifest(manifestIdentity, clientFiles, clientDirectories, plugins);
        }

        private IEnumerable<PluginUpdateManifest> _processingPlugins(ClientIdentityNode clientIdentity, System.Xml.XPath.XPathNavigator pluginsElement, XmlNamespaceManager nsmgr)
        {
            foreach (System.Xml.XPath.XPathNavigator pluginElement in pluginsElement.Select("mu:Plugin", nsmgr))
            {
                var identityElement = pluginElement.SelectSingleNode("mu:Identity", nsmgr);
                var identity = _parsePluginManifestIdentity(clientIdentity, identityElement);
                var filesElement = pluginElement.SelectSingleNode("mu:Files", nsmgr);

                var files = _parseFiles(filesElement, (nodeType, name, crc) => new PluginFileNode(identity.PluginsTemporaryDirectory, identity.PluginsTargetDirectory, identity, nodeType, name, crc), nsmgr);

                //DCH 0055082 06.03.2017
                var directories = _parseDirectories<PluginDirectoryNode>(filesElement, (nodeType, name) => new PluginDirectoryNode(identity.PluginsTemporaryDirectory, identity.PluginsTargetDirectory, identity, nodeType, name), nsmgr);
                var actionsElement = pluginElement.SelectSingleNode("mu:Actions", nsmgr);
                ActionsNode actions = null;//DAVID.CHARVAT 0049336 03.02.2015 Rozšíření aktualizač. systému klienta
                var knownActions = new string[] {"onBeforeInstall", "onAfterInstall", "onBeforeUpdate", "onAfterUpdate", "onBeforeUninstall", "onAfterUninstall"};
                if (actionsElement != null)
                {
                    var a = new Dictionary<string, string>();
                    foreach (var knownAction in knownActions)
                    {
                        var k = actionsElement.GetAttribute(knownAction, string.Empty);    
                        if (string.IsNullOrEmpty(k)) continue;
                        a.Add(knownAction, k);
                    }
                    actions = new ActionsNode(a);
                }
                yield return new PluginUpdateManifest(identity, actions, files, directories);
            }
        }

        private PluginIdentityNode _parsePluginManifestIdentity(ClientIdentityNode clientIdentity, System.Xml.XPath.XPathNavigator identityElement)
        {
            var author = identityElement.GetAttribute("author", string.Empty);
            var name = identityElement.GetAttribute("name", string.Empty);

            var pluginsTempDir = Path.Combine(TemporaryDirectory, PluginsDirectoryName);
            var pluginsTargetDir = Path.Combine(TargetDirectory, PluginsDirectoryName);

            return new PluginIdentityNode(clientIdentity, pluginsTempDir, pluginsTargetDir, author, name);
        }

        internal delegate T CreateFileNode<T>(NodeType nodeType, string name, string crc) where T : FileNode;

        //DCH 0055082 06.03.2017
        internal delegate T CreateDirectoryNode<T>(NodeType nodeType, string name) where T : UpdateNode;

        //DCH 0055082 06.03.2017
        private IEnumerable<TResult> _parseDirectories<TResult>(System.Xml.XPath.XPathNavigator filesElement, CreateDirectoryNode<TResult> createResult, XmlNamespaceManager nsmgr)
            where TResult: UpdateNode
        {
            // directUpdate
            // notUpdate
            var notUpdateElement = filesElement.SelectSingleNode("mu:NotUpdate", nsmgr);
            if (notUpdateElement != null)
            {
                foreach (System.Xml.XPath.XPathNavigator fileElement in notUpdateElement.Select("mu:Directory", nsmgr))
                {
                    string name = fileElement.GetAttribute("name", string.Empty);
                    yield return createResult(NodeType.NotUpdate, name);
                }
            }
        }

        private IEnumerable<TResult> _parseFiles<TResult>(System.Xml.XPath.XPathNavigator filesElement, CreateFileNode<TResult> createResult, XmlNamespaceManager nsmgr)
            where TResult : FileNode
        {
            // directUpdate
            var directUpdateElement = filesElement.SelectSingleNode("mu:DirectUpdate", nsmgr);
            if (directUpdateElement != null)
                foreach (var result in _createFileNodes(NodeType.DirectUpdate, createResult, directUpdateElement, nsmgr)) yield return result;

            // shadowUpdate
            var shadowUpdateElement = filesElement.SelectSingleNode("mu:ShadowUpdate", nsmgr);
            if (shadowUpdateElement != null)
                foreach (var result in _createFileNodes(NodeType.ShadowUpdate, createResult, shadowUpdateElement, nsmgr)) yield return result;

            // notUpdate
            var notUpdateElement = filesElement.SelectSingleNode("mu:NotUpdate", nsmgr);
            if (notUpdateElement != null)
                foreach (var result in _createFileNodes(NodeType.NotUpdate, createResult, notUpdateElement, nsmgr)) yield return result;
        }

        private static IEnumerable<TResult> _createFileNodes<TResult>(NodeType nodeType, CreateFileNode<TResult> createResult, System.Xml.XPath.XPathNavigator filesElement, XmlNamespaceManager nsmgr) 
            where TResult : FileNode
        {
            foreach (System.Xml.XPath.XPathNavigator fileElement in filesElement.Select("mu:File", nsmgr))
            {
                string name = fileElement.GetAttribute("name", string.Empty);
                string crc = fileElement.GetAttribute("crc", string.Empty);
                yield return createResult(nodeType, name, crc);
            }
        }


        private ClientIdentityNode _parseClientManifestIdentity(System.Xml.XPath.XPathNavigator identityElement)
        {
            var author = identityElement.GetAttribute("author", string.Empty);
            var name = identityElement.GetAttribute("name", string.Empty);
            return new ClientIdentityNode(author, name);
        }
    }
}
