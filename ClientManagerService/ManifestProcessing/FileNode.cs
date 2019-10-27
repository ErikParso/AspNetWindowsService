// Supervisor:DAVID.CHARVAT
// Part of HELIOS Green, proprietary software, (c) Asseco Solution, a. s.
// Redistribution and use in source and binary forms, with or without modification,
// is not permitted without valid contract with Asseco Solution, a. s.

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace ClientManagerService.ManifestProcessing
{
    /// <summary> Values that represent FileNodeType. </summary>
    public enum NodeType
    {
        /// <summary> An enum constant representing the shadow update option. </summary>
        ShadowUpdate = 1,
        /// <summary> An enum constant representing the direct update option. </summary>
        DirectUpdate = 2,
        /// <summary> An enum constant representing the not update option. </summary>
        NotUpdate = 3,
    }

    public abstract class UpdateNode //DCH 0055082 06.03.2017 Změna chování pluginů
    {
        /// <summary> Gets or sets the name of the node. </summary>
        /// <value> The name of the file or directory. </value>
        public string Name { get; }

        /// <summary> Gets or sets the type of the node. </summary>
        /// <value> The type of the node. </value>
        public NodeType NodeType { get; }

        /// <summary> Gets or sets the pathname of the temporary directory. </summary>
        /// <value> The pathname of the temporary directory. </value>
        public string TemporaryDirectory { get; }

        /// <summary> Gets or sets the pathname of the target directory. </summary>
        /// <value> The pathname of the target directory. </value>
        public string TargetDirectory { get; }

        /// <summary> Gets the full pathname of the temporary full file. </summary>
        /// <value> The full pathname of the temporary full file. </value>
        public abstract string TemporaryFullPath { get; }

        /// <summary> Gets the full pathname of the target full file. </summary>
        /// <value> The full pathname of the target full file. </value>
        public abstract string TargetFullPath { get; }

        protected UpdateNode(string temporaryDirectory, string targetDirectory, NodeType nodeType, string name)
        {
            TemporaryDirectory = temporaryDirectory;
            TargetDirectory = targetDirectory;
            NodeType = nodeType;
            Name = name;
        }

        #region Equality

        public override int GetHashCode()
        {
            return Name.GetHashCode() ^ TargetDirectory.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            var o = obj as FileNode;
            if ((object)o == null) return false; //NOTE: explicit cast to object to avoid call "==" operator recursively!

            return string.Equals(TargetFullPath, o.TargetFullPath, StringComparison.OrdinalIgnoreCase);
        }

        public override string ToString()
        {
            return string.Format("Name={0},Type={1}, TargetDirectory={2}", Name,  NodeType, TargetDirectory);
        }

        public static bool operator ==(UpdateNode obj1, UpdateNode obj2)
        {
            return Object.Equals(obj1, obj2);
        }

        public static bool operator !=(UpdateNode obj1, UpdateNode obj2)
        {
            return !Object.Equals(obj1, obj2);
        }

        #endregion
    }

    public sealed class ClientDirectoryNode : UpdateNode
    {//DCH 0055082 06.03.2017 Změna chování pluginů
        public ClientDirectoryNode(string temporaryDirectory, string targetDirectory, NodeType nodeType, string name) 
            : base(temporaryDirectory, targetDirectory, nodeType, name)
        {
        }

        /// <summary> Gets the full pathname of the temporary full file. </summary>
        /// <value> The full pathname of the temporary full file. </value>
        public override string TemporaryFullPath
        {
            get { return Path.Combine(TemporaryDirectory, Name); }
        }

        /// <summary> Gets the full pathname of the target full file. </summary>
        /// <value> The full pathname of the target full file. </value>
        public override string TargetFullPath
        {
            get { return Path.Combine(TargetDirectory, Name); }
        }
    }

    public sealed class PluginDirectoryNode : UpdateNode
    {//DCH 0055082 06.03.2017 Změna chování pluginů
        /// <summary> Gets or sets the client plug-in identity. </summary>
        /// <value> The client plug-in identity. </value>
        public PluginIdentityNode PluginIdentity { get; private set; }

        public PluginDirectoryNode(string temporaryDirectory, string targetDirectory, PluginIdentityNode pluginIdentity, NodeType nodeType, string name) 
            : base(temporaryDirectory, targetDirectory, nodeType, name)
        {
            PluginIdentity = pluginIdentity;
        }

        /// <summary> Gets the full pathname of the temporary full file. </summary>
        /// <value> The full pathname of the temporary full file. </value>
        public override string TemporaryFullPath
        {
            get { return Path.Combine(PluginIdentity.GetPluginDirectory(TemporaryDirectory), Name); }
        }

        /// <summary> Gets the full pathname of the target full file. </summary>
        /// <value> The full pathname of the target full file. </value>
        public override string TargetFullPath
        {
            get { return Path.Combine(PluginIdentity.GetPluginDirectory(TargetDirectory), Name); }
        }
    }

    /// <summary> A file node. </summary>
    public abstract class FileNode : UpdateNode //DCH 0055082 06.03.2017 Změna předka pro podporu Directory elementu mezi File elementy v manifestu
    { 
        /// <summary> Gets or sets the filename of the file. </summary>
        /// <value> The name of the file. </value>
        public string FileName { get { return Name; } }

        /// <summary> Gets or sets the CRC. </summary>
        /// <value> The CRC. </value>
        public string Crc { get; private set; }

        /// <summary> Specialized constructor for use only by derived classes. </summary>
        /// <param name="temporaryDirectory"> The pathname of the temporary directory. </param>
        /// <param name="targetDirectory">    The pathname of the target directory. </param>
        /// <param name="nodeType">           The type of the node. </param>
        /// <param name="fileName">           The name of the file. </param>
        /// <param name="crc">                The CRC. </param>
        protected FileNode(string temporaryDirectory, string targetDirectory, NodeType nodeType, string fileName, string crc)
            : base(temporaryDirectory, targetDirectory, nodeType, fileName)
        {
            Crc = crc;
        }

        public override string ToString()
        {
            return string.Format("FileName={0},CRC={1}, Type={2}, TargetDirectory={3}", Name, Crc, NodeType, TargetDirectory);
        }

    }

    /// <summary> A client file node. </summary>
    public sealed class ClientFileNode : FileNode
    {
        public ClientFileNode(string temporaryDirectory, string targetDirectory, NodeType nodeType, string fileName, string crc) : base(temporaryDirectory, targetDirectory, nodeType, fileName, crc)
        {
        }

        /// <summary> Gets the full pathname of the temporary full file. </summary>
        /// <value> The full pathname of the temporary full file. </value>
        public override string TemporaryFullPath
        {
            get { return Path.Combine(TemporaryDirectory, FileName); }
        }

        /// <summary> Gets the full pathname of the target full file. </summary>
        /// <value> The full pathname of the target full file. </value>
        public override string TargetFullPath
        {
            get { return Path.Combine(TargetDirectory, FileName); }
        }
    }

    /// <summary> A client plug-in file node. </summary>
    public sealed class PluginFileNode : FileNode
    {
        /// <summary> Gets or sets the client plug-in identity. </summary>
        /// <value> The client plug-in identity. </value>
        public PluginIdentityNode PluginIdentity { get; private set; }

        /// <summary> Constructor. </summary>
        /// <param name="temporaryDirectory"> Pathname of the temporary directory. </param>
        /// <param name="targetDirectory">    Pathname of the target directory. </param>
        /// <param name="pluginIdentity">     The client plug-in identity. </param>
        /// <param name="nodeType">           Type of the node. </param>
        /// <param name="fileName">           Filename of the file. </param>
        /// <param name="crc">                The CRC. </param>
        public PluginFileNode(string temporaryDirectory, string targetDirectory, PluginIdentityNode pluginIdentity, NodeType nodeType, string fileName, string crc)
            : base(temporaryDirectory, targetDirectory, nodeType, fileName, crc)
        {
            PluginIdentity = pluginIdentity;
        }
       
        /// <summary> Gets the full pathname of the temporary full file. </summary>
        /// <value> The full pathname of the temporary full file. </value>
        public override string TemporaryFullPath
        {
            get { return Path.Combine(PluginIdentity.GetPluginDirectory(TemporaryDirectory), FileName); }
        }

        /// <summary> Gets the full pathname of the target full file. </summary>
        /// <value> The full pathname of the target full file. </value>
        public override string TargetFullPath
        {
            get { return Path.Combine(PluginIdentity.GetPluginDirectory(TargetDirectory), FileName); }
        }
    }
}
