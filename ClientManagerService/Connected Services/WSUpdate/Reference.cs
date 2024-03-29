﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace WSUpdate
{
    
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Tools.ServiceModel.Svcutil", "2.0.1-preview-30514-0828")]
    [System.ServiceModel.ServiceContractAttribute(Namespace="http://lcs.cz/webservices/", ConfigurationName="WSUpdate.ClientUpdateSoap")]
    public interface ClientUpdateSoap
    {
        
        [System.ServiceModel.OperationContractAttribute(Action="http://lcs.cz/webservices/GetClientManifestInfo", ReplyAction="*")]
        [System.ServiceModel.XmlSerializerFormatAttribute(SupportFaults=true)]
        System.Threading.Tasks.Task<WSUpdate.ClientManifestDescriptor> GetClientManifestInfoAsync(string clientAuthor, string clientName);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://lcs.cz/webservices/GetClientUpdateInfo", ReplyAction="*")]
        [System.ServiceModel.XmlSerializerFormatAttribute(SupportFaults=true)]
        System.Threading.Tasks.Task<WSUpdate.ClientUpdateDescriptor> GetClientUpdateInfoAsync(string platform);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://lcs.cz/webservices/LoadFile", ReplyAction="*")]
        [System.ServiceModel.XmlSerializerFormatAttribute(SupportFaults=true)]
        System.Threading.Tasks.Task<WSUpdate.LoadClientFileDescriptor> LoadFileAsync(string platform, string file, string flags);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://lcs.cz/webservices/LoadPluginFile", ReplyAction="*")]
        [System.ServiceModel.XmlSerializerFormatAttribute(SupportFaults=true)]
        System.Threading.Tasks.Task<WSUpdate.LoadClientFileDescriptor> LoadPluginFileAsync(string clientAuthor, string clientName, string pluginAuthor, string pluginName, string fileName, string flags);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://lcs.cz/webservices/LoadClientFile", ReplyAction="*")]
        [System.ServiceModel.XmlSerializerFormatAttribute(SupportFaults=true)]
        System.Threading.Tasks.Task<WSUpdate.LoadClientFileDescriptor> LoadClientFileAsync(string clientAuthor, string clientName, string fileName, string flags);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://lcs.cz/webservices/GetMessages", ReplyAction="*")]
        [System.ServiceModel.XmlSerializerFormatAttribute(SupportFaults=true)]
        System.Threading.Tasks.Task<string[]> GetMessagesAsync(string[] codes, string language);
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Tools.ServiceModel.Svcutil", "2.0.1-preview-30514-0828")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://lcs.cz/webservices/")]
    public partial class ClientManifestDescriptor
    {
        
        private bool wasErrorField;
        
        private byte[] dataField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Order=0)]
        public bool WasError
        {
            get
            {
                return this.wasErrorField;
            }
            set
            {
                this.wasErrorField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(DataType="base64Binary", Order=1)]
        public byte[] Data
        {
            get
            {
                return this.dataField;
            }
            set
            {
                this.dataField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Tools.ServiceModel.Svcutil", "2.0.1-preview-30514-0828")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://lcs.cz/webservices/")]
    public partial class LoadClientFileDescriptor
    {
        
        private byte[] dataField;
        
        private string nameField;
        
        private System.DateTime modifiedField;
        
        private bool zipUsedField;
        
        private string errorMessageField;
        
        private string flagsField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(DataType="base64Binary", Order=0)]
        public byte[] Data
        {
            get
            {
                return this.dataField;
            }
            set
            {
                this.dataField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Order=1)]
        public string Name
        {
            get
            {
                return this.nameField;
            }
            set
            {
                this.nameField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Order=2)]
        public System.DateTime Modified
        {
            get
            {
                return this.modifiedField;
            }
            set
            {
                this.modifiedField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Order=3)]
        public bool ZipUsed
        {
            get
            {
                return this.zipUsedField;
            }
            set
            {
                this.zipUsedField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Order=4)]
        public string ErrorMessage
        {
            get
            {
                return this.errorMessageField;
            }
            set
            {
                this.errorMessageField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Order=5)]
        public string Flags
        {
            get
            {
                return this.flagsField;
            }
            set
            {
                this.flagsField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Tools.ServiceModel.Svcutil", "2.0.1-preview-30514-0828")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://lcs.cz/webservices/")]
    public partial class ClientUpdateDescriptor
    {
        
        private byte[] xmlField;
        
        private string[] namesField;
        
        private System.DateTime[] modifiedField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(DataType="base64Binary", Order=0)]
        public byte[] Xml
        {
            get
            {
                return this.xmlField;
            }
            set
            {
                this.xmlField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlArrayAttribute(Order=1)]
        public string[] Names
        {
            get
            {
                return this.namesField;
            }
            set
            {
                this.namesField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlArrayAttribute(Order=2)]
        public System.DateTime[] Modified
        {
            get
            {
                return this.modifiedField;
            }
            set
            {
                this.modifiedField = value;
            }
        }
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Tools.ServiceModel.Svcutil", "2.0.1-preview-30514-0828")]
    public interface ClientUpdateSoapChannel : WSUpdate.ClientUpdateSoap, System.ServiceModel.IClientChannel
    {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Tools.ServiceModel.Svcutil", "2.0.1-preview-30514-0828")]
    public partial class ClientUpdateSoapClient : System.ServiceModel.ClientBase<WSUpdate.ClientUpdateSoap>, WSUpdate.ClientUpdateSoap
    {
        
        /// <summary>
        /// Implement this partial method to configure the service endpoint.
        /// </summary>
        /// <param name="serviceEndpoint">The endpoint to configure</param>
        /// <param name="clientCredentials">The client credentials</param>
        static partial void ConfigureEndpoint(System.ServiceModel.Description.ServiceEndpoint serviceEndpoint, System.ServiceModel.Description.ClientCredentials clientCredentials);
        
        public ClientUpdateSoapClient(EndpointConfiguration endpointConfiguration) : 
                base(ClientUpdateSoapClient.GetBindingForEndpoint(endpointConfiguration), ClientUpdateSoapClient.GetEndpointAddress(endpointConfiguration))
        {
            this.Endpoint.Name = endpointConfiguration.ToString();
            ConfigureEndpoint(this.Endpoint, this.ClientCredentials);
        }
        
        public ClientUpdateSoapClient(EndpointConfiguration endpointConfiguration, string remoteAddress) : 
                base(ClientUpdateSoapClient.GetBindingForEndpoint(endpointConfiguration), new System.ServiceModel.EndpointAddress(remoteAddress))
        {
            this.Endpoint.Name = endpointConfiguration.ToString();
            ConfigureEndpoint(this.Endpoint, this.ClientCredentials);
        }
        
        public ClientUpdateSoapClient(EndpointConfiguration endpointConfiguration, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(ClientUpdateSoapClient.GetBindingForEndpoint(endpointConfiguration), remoteAddress)
        {
            this.Endpoint.Name = endpointConfiguration.ToString();
            ConfigureEndpoint(this.Endpoint, this.ClientCredentials);
        }
        
        public ClientUpdateSoapClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(binding, remoteAddress)
        {
        }
        
        public System.Threading.Tasks.Task<WSUpdate.ClientManifestDescriptor> GetClientManifestInfoAsync(string clientAuthor, string clientName)
        {
            return base.Channel.GetClientManifestInfoAsync(clientAuthor, clientName);
        }
        
        public System.Threading.Tasks.Task<WSUpdate.ClientUpdateDescriptor> GetClientUpdateInfoAsync(string platform)
        {
            return base.Channel.GetClientUpdateInfoAsync(platform);
        }
        
        public System.Threading.Tasks.Task<WSUpdate.LoadClientFileDescriptor> LoadFileAsync(string platform, string file, string flags)
        {
            return base.Channel.LoadFileAsync(platform, file, flags);
        }
        
        public System.Threading.Tasks.Task<WSUpdate.LoadClientFileDescriptor> LoadPluginFileAsync(string clientAuthor, string clientName, string pluginAuthor, string pluginName, string fileName, string flags)
        {
            return base.Channel.LoadPluginFileAsync(clientAuthor, clientName, pluginAuthor, pluginName, fileName, flags);
        }
        
        public System.Threading.Tasks.Task<WSUpdate.LoadClientFileDescriptor> LoadClientFileAsync(string clientAuthor, string clientName, string fileName, string flags)
        {
            return base.Channel.LoadClientFileAsync(clientAuthor, clientName, fileName, flags);
        }
        
        public System.Threading.Tasks.Task<string[]> GetMessagesAsync(string[] codes, string language)
        {
            return base.Channel.GetMessagesAsync(codes, language);
        }
        
        public virtual System.Threading.Tasks.Task OpenAsync()
        {
            return System.Threading.Tasks.Task.Factory.FromAsync(((System.ServiceModel.ICommunicationObject)(this)).BeginOpen(null, null), new System.Action<System.IAsyncResult>(((System.ServiceModel.ICommunicationObject)(this)).EndOpen));
        }
        
        public virtual System.Threading.Tasks.Task CloseAsync()
        {
            return System.Threading.Tasks.Task.Factory.FromAsync(((System.ServiceModel.ICommunicationObject)(this)).BeginClose(null, null), new System.Action<System.IAsyncResult>(((System.ServiceModel.ICommunicationObject)(this)).EndClose));
        }
        
        private static System.ServiceModel.Channels.Binding GetBindingForEndpoint(EndpointConfiguration endpointConfiguration)
        {
            if ((endpointConfiguration == EndpointConfiguration.ClientUpdateSoap))
            {
                System.ServiceModel.BasicHttpBinding result = new System.ServiceModel.BasicHttpBinding();
                result.MaxBufferSize = int.MaxValue;
                result.ReaderQuotas = System.Xml.XmlDictionaryReaderQuotas.Max;
                result.MaxReceivedMessageSize = int.MaxValue;
                result.AllowCookies = true;
                return result;
            }
            if ((endpointConfiguration == EndpointConfiguration.ClientUpdateSoap12))
            {
                System.ServiceModel.Channels.CustomBinding result = new System.ServiceModel.Channels.CustomBinding();
                System.ServiceModel.Channels.TextMessageEncodingBindingElement textBindingElement = new System.ServiceModel.Channels.TextMessageEncodingBindingElement();
                textBindingElement.MessageVersion = System.ServiceModel.Channels.MessageVersion.CreateVersion(System.ServiceModel.EnvelopeVersion.Soap12, System.ServiceModel.Channels.AddressingVersion.None);
                result.Elements.Add(textBindingElement);
                System.ServiceModel.Channels.HttpTransportBindingElement httpBindingElement = new System.ServiceModel.Channels.HttpTransportBindingElement();
                httpBindingElement.AllowCookies = true;
                httpBindingElement.MaxBufferSize = int.MaxValue;
                httpBindingElement.MaxReceivedMessageSize = int.MaxValue;
                result.Elements.Add(httpBindingElement);
                return result;
            }
            throw new System.InvalidOperationException(string.Format("Could not find endpoint with name \'{0}\'.", endpointConfiguration));
        }
        
        private static System.ServiceModel.EndpointAddress GetEndpointAddress(EndpointConfiguration endpointConfiguration)
        {
            if ((endpointConfiguration == EndpointConfiguration.ClientUpdateSoap))
            {
                return new System.ServiceModel.EndpointAddress("http://camel/Source99-E5A1/ClientUpdate.asmx");
            }
            if ((endpointConfiguration == EndpointConfiguration.ClientUpdateSoap12))
            {
                return new System.ServiceModel.EndpointAddress("http://camel/Source99-E5A1/ClientUpdate.asmx");
            }
            throw new System.InvalidOperationException(string.Format("Could not find endpoint with name \'{0}\'.", endpointConfiguration));
        }
        
        public enum EndpointConfiguration
        {
            
            ClientUpdateSoap,
            
            ClientUpdateSoap12,
        }
    }
}
