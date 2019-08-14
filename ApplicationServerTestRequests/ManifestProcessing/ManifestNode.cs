// Supervisor:DAVID.CHARVAT
// Part of HELIOS Green, proprietary software, (c) Asseco Solution, a. s.
// Redistribution and use in source and binary forms, with or without modification,
// is not permitted without valid contract with Asseco Solution, a. s.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;

namespace NorisWin32Update.ManifestProcessing
{
    /// <summary> Update information. </summary>
    /// <typeparam name="TIdentity"> Type of the identity. </typeparam>
    /// <typeparam name="TFileNode"> Type of the file node. </typeparam>
    public abstract class UpdateManifest<TIdentity, TFileNode, TDirectoryNode>
        where TFileNode : FileNode
        where TDirectoryNode : UpdateNode
        where TIdentity : IdentityNode
    {
        protected UpdateManifest(TIdentity identity, IEnumerable<TFileNode> files, IEnumerable<TDirectoryNode> directories)
        {
            Identity = identity;
            Files = new List<TFileNode>(files);
            Directories = new List<TDirectoryNode>(directories);
        }
        public TIdentity Identity { get; private set; }

        public List<TFileNode> Files { get; }

        public List<TDirectoryNode> Directories { get; } 
    }

    /// <summary> Client update information. </summary>
    public sealed class ClientUpdateManifest : UpdateManifest<ClientIdentityNode, ClientFileNode, ClientDirectoryNode>
    {
        public ReadOnlyCollection<PluginUpdateManifest> Plugins { get { return new ReadOnlyCollection<PluginUpdateManifest>(_plugins); } }
        private readonly List<PluginUpdateManifest> _plugins;

        public ClientUpdateManifest(ClientIdentityNode identity, IEnumerable<ClientFileNode> files, IEnumerable<ClientDirectoryNode> directories, IEnumerable<PluginUpdateManifest> plugins) : base(identity, files, directories)
        {
            _plugins = new List<PluginUpdateManifest>(plugins);
        }

        internal bool TryGetPlugin(PluginIdentityNode pluginIdentity, out PluginUpdateManifest currentPlugin)
        {
            foreach (var plugin in Plugins)
            {
                if (plugin.Identity.Equals(pluginIdentity))
                {
                    currentPlugin = plugin;
                    return true;
                }
            }
            currentPlugin = null;
            return false;
        }
    }

    /// <summary> Client Plug-in update information. </summary>
    public sealed class PluginUpdateManifest : UpdateManifest<PluginIdentityNode, PluginFileNode, PluginDirectoryNode>
    {
        public ActionsNode Actions { get; private set; }

        public PluginUpdateManifest(PluginIdentityNode identity, ActionsNode actions, IEnumerable<PluginFileNode> files, IEnumerable<PluginDirectoryNode> directories) : base(identity, files, directories)
        {
            Actions = actions ?? new ActionsNode(null);
        }
    }
    
    /// <summary> An identity node. </summary>
    public abstract class IdentityNode
    {
        public string Author { get; private set; }
        public string Name { get; private set; }

        protected IdentityNode(string author, string name)
        {
            Author = author;
            Name = name;
        }

        #region Equality

        public override int GetHashCode()
        {
            //use the most important keys only! (xor more keys with hashcodes and take care about nullable keys)
            //Example: return Key1.GetHashCode() ^ (Key2 == null ? 0 : Key2.GetHashCode())  //nullable key (value 0 doesn't impress XOR operation result)
            return Name.GetHashCode() ^ (string.IsNullOrEmpty(Author) ? 0 : Author.GetHashCode());
        }

        public override bool Equals(object obj)
        {
            var o = obj as IdentityNode;
            if ((object)o == null) return false; //NOTE: explicit cast to object to avoid call "==" operator recursively!

            //NOTE: test the most important keys first!
            //Example: return (this.Key1 == o.Key1) && (this.Key2 == o.Key2);
            return (this.Name == o.Name) && (this.Author == o.Author);
        }

        public override string ToString()
        {
            //Example: return String.Format("Key1 = {0}, Key2 = {1}", this.Key1, this.Key2);
            return base.ToString();
        }

        public static bool operator ==(IdentityNode obj1, IdentityNode obj2)
        {
            return Object.Equals(obj1, obj2);
        }

        public static bool operator !=(IdentityNode obj1, IdentityNode obj2)
        {
            return !Object.Equals(obj1, obj2);
        }

        #endregion
    }

    /// <summary> A client identity node. </summary>
    public sealed class ClientIdentityNode : IdentityNode
    {
        public ClientIdentityNode(string author, string name) : base(author, name)
        {
        }
    }

    /// <summary> A client plug-in identity node. </summary>
    public sealed class PluginIdentityNode : IdentityNode
    {
        public string PluginsTemporaryDirectory { get; private set; }
        public string PluginsTargetDirectory { get; private set; }
        public ClientIdentityNode Client { get; private set; }

        public PluginIdentityNode(ClientIdentityNode client, string pluginsTemporaryDirectory, string pluginsTargetDirectory, string author, string name) : base(author, name)
        {
            PluginsTemporaryDirectory = pluginsTemporaryDirectory;
            PluginsTargetDirectory = pluginsTargetDirectory;
            Client = client;
        }

        public string GetPluginDirectory(string pluginsDirectory)
        {
            var s = pluginsDirectory;
            if (string.IsNullOrEmpty(Author) == false)
                s = Path.Combine(s, Author);
            if (string.IsNullOrEmpty(Name) == false)
                s = Path.Combine(s, Name);
            return s;
        }

        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns>
        /// A string that represents the current object.
        /// </returns>
        public override string ToString()
        {
            return string.Format("{0}.{1}", Author, Name);
        }


    }

    public sealed class ActionsNode
    {
        private readonly Dictionary<string, string> _actions;

        public ActionsNode(Dictionary<string, string> actions)
        {
            _actions = actions;
        }

        private string _getActionCommand(string actionName, string defaultValue)
        {
            string command;
            if (_actions == null) return defaultValue;
            return _actions.TryGetValue(actionName, out command) ? command : defaultValue;
        }

        public string OnBeforeInstall { get { return _getActionCommand("onBeforeInstall", null); } }
        public string OnAfterInstall { get { return _getActionCommand("onAfterInstall", null); } }
        public string OnBeforeUpdate { get { return _getActionCommand("onBeforeUpdate", null); } }
        public string OnAfterUpdate { get { return _getActionCommand("onAfterUpdate", null); } }
        public string OnBeforeUninstall { get { return _getActionCommand("onBeforeUninstall", null); } }
        public string OnAfterUninstall { get { return _getActionCommand("onAfterUninstall", null); } }
    }
}
