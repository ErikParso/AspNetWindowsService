﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace WSData
{
    
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Tools.ServiceModel.Svcutil", "2.0.1-preview-30514-0828")]
    [System.ServiceModel.ServiceContractAttribute(Namespace="http://lcs.cz/webservices/", ConfigurationName="WSData.DataSoap")]
    public interface DataSoap
    {
        
        [System.ServiceModel.OperationContractAttribute(Action="http://lcs.cz/webservices/GetInfo", ReplyAction="*")]
        [System.ServiceModel.XmlSerializerFormatAttribute(SupportFaults=true)]
        System.Threading.Tasks.Task<WSData.GetInfoResponse> GetInfoAsync(WSData.GetInfoRequest request);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://lcs.cz/webservices/LoadFile", ReplyAction="*")]
        [System.ServiceModel.XmlSerializerFormatAttribute(SupportFaults=true)]
        System.Threading.Tasks.Task<WSData.LoadClientFileDescriptor> LoadFileAsync(string platform, string file, string flags);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://lcs.cz/webservices/StreamService", ReplyAction="*")]
        [System.ServiceModel.XmlSerializerFormatAttribute(SupportFaults=true)]
        System.Threading.Tasks.Task<WSData.StreamServiceResponse> StreamServiceAsync(WSData.StreamServiceRequest request);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://lcs.cz/webservices/Connect", ReplyAction="*")]
        [System.ServiceModel.XmlSerializerFormatAttribute(SupportFaults=true)]
        System.Threading.Tasks.Task<string> ConnectAsync(string connectionData);
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Tools.ServiceModel.Svcutil", "2.0.1-preview-30514-0828")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://lcs.cz/webservices/")]
    public partial class NorisID
    {
        
        private string sIDField;
        
        private string qIDField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Order=0)]
        public string SID
        {
            get
            {
                return this.sIDField;
            }
            set
            {
                this.sIDField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Order=1)]
        public string QID
        {
            get
            {
                return this.qIDField;
            }
            set
            {
                this.qIDField = value;
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
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Tools.ServiceModel.Svcutil", "2.0.1-preview-30514-0828")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(WrapperName="GetInfo", WrapperNamespace="http://lcs.cz/webservices/", IsWrapped=true)]
    public partial class GetInfoRequest
    {
        
        [System.ServiceModel.MessageHeaderAttribute(Namespace="http://lcs.cz/webservices/")]
        public WSData.NorisID NorisID;
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="http://lcs.cz/webservices/", Order=0)]
        public string myCode;
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="http://lcs.cz/webservices/", Order=1)]
        public string myValue;
        
        public GetInfoRequest()
        {
        }
        
        public GetInfoRequest(WSData.NorisID NorisID, string myCode, string myValue)
        {
            this.NorisID = NorisID;
            this.myCode = myCode;
            this.myValue = myValue;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Tools.ServiceModel.Svcutil", "2.0.1-preview-30514-0828")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(WrapperName="GetInfoResponse", WrapperNamespace="http://lcs.cz/webservices/", IsWrapped=true)]
    public partial class GetInfoResponse
    {
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="http://lcs.cz/webservices/", Order=0)]
        public string GetInfoResult;
        
        public GetInfoResponse()
        {
        }
        
        public GetInfoResponse(string GetInfoResult)
        {
            this.GetInfoResult = GetInfoResult;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Tools.ServiceModel.Svcutil", "2.0.1-preview-30514-0828")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(WrapperName="StreamService", WrapperNamespace="http://lcs.cz/webservices/", IsWrapped=true)]
    public partial class StreamServiceRequest
    {
        
        [System.ServiceModel.MessageHeaderAttribute(Namespace="http://lcs.cz/webservices/")]
        public WSData.NorisID NorisID;
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="http://lcs.cz/webservices/", Order=0)]
        public string action;
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="http://lcs.cz/webservices/", Order=1)]
        [System.Xml.Serialization.XmlElementAttribute(DataType="base64Binary")]
        public byte[] fromClient;
        
        public StreamServiceRequest()
        {
        }
        
        public StreamServiceRequest(WSData.NorisID NorisID, string action, byte[] fromClient)
        {
            this.NorisID = NorisID;
            this.action = action;
            this.fromClient = fromClient;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Tools.ServiceModel.Svcutil", "2.0.1-preview-30514-0828")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(WrapperName="StreamServiceResponse", WrapperNamespace="http://lcs.cz/webservices/", IsWrapped=true)]
    public partial class StreamServiceResponse
    {
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="http://lcs.cz/webservices/", Order=0)]
        [System.Xml.Serialization.XmlElementAttribute(DataType="base64Binary")]
        public byte[] StreamServiceResult;
        
        public StreamServiceResponse()
        {
        }
        
        public StreamServiceResponse(byte[] StreamServiceResult)
        {
            this.StreamServiceResult = StreamServiceResult;
        }
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Tools.ServiceModel.Svcutil", "2.0.1-preview-30514-0828")]
    public interface DataSoapChannel : WSData.DataSoap, System.ServiceModel.IClientChannel
    {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Tools.ServiceModel.Svcutil", "2.0.1-preview-30514-0828")]
    public partial class DataSoapClient : System.ServiceModel.ClientBase<WSData.DataSoap>, WSData.DataSoap
    {
        
        /// <summary>
        /// Implement this partial method to configure the service endpoint.
        /// </summary>
        /// <param name="serviceEndpoint">The endpoint to configure</param>
        /// <param name="clientCredentials">The client credentials</param>
        static partial void ConfigureEndpoint(System.ServiceModel.Description.ServiceEndpoint serviceEndpoint, System.ServiceModel.Description.ClientCredentials clientCredentials);
        
        public DataSoapClient(EndpointConfiguration endpointConfiguration) : 
                base(DataSoapClient.GetBindingForEndpoint(endpointConfiguration), DataSoapClient.GetEndpointAddress(endpointConfiguration))
        {
            this.Endpoint.Name = endpointConfiguration.ToString();
            ConfigureEndpoint(this.Endpoint, this.ClientCredentials);
        }
        
        public DataSoapClient(EndpointConfiguration endpointConfiguration, string remoteAddress) : 
                base(DataSoapClient.GetBindingForEndpoint(endpointConfiguration), new System.ServiceModel.EndpointAddress(remoteAddress))
        {
            this.Endpoint.Name = endpointConfiguration.ToString();
            ConfigureEndpoint(this.Endpoint, this.ClientCredentials);
        }
        
        public DataSoapClient(EndpointConfiguration endpointConfiguration, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(DataSoapClient.GetBindingForEndpoint(endpointConfiguration), remoteAddress)
        {
            this.Endpoint.Name = endpointConfiguration.ToString();
            ConfigureEndpoint(this.Endpoint, this.ClientCredentials);
        }
        
        public DataSoapClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(binding, remoteAddress)
        {
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        System.Threading.Tasks.Task<WSData.GetInfoResponse> WSData.DataSoap.GetInfoAsync(WSData.GetInfoRequest request)
        {
            return base.Channel.GetInfoAsync(request);
        }
        
        public System.Threading.Tasks.Task<WSData.GetInfoResponse> GetInfoAsync(WSData.NorisID NorisID, string myCode, string myValue)
        {
            WSData.GetInfoRequest inValue = new WSData.GetInfoRequest();
            inValue.NorisID = NorisID;
            inValue.myCode = myCode;
            inValue.myValue = myValue;
            return ((WSData.DataSoap)(this)).GetInfoAsync(inValue);
        }
        
        public System.Threading.Tasks.Task<WSData.LoadClientFileDescriptor> LoadFileAsync(string platform, string file, string flags)
        {
            return base.Channel.LoadFileAsync(platform, file, flags);
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        System.Threading.Tasks.Task<WSData.StreamServiceResponse> WSData.DataSoap.StreamServiceAsync(WSData.StreamServiceRequest request)
        {
            return base.Channel.StreamServiceAsync(request);
        }
        
        public System.Threading.Tasks.Task<WSData.StreamServiceResponse> StreamServiceAsync(WSData.NorisID NorisID, string action, byte[] fromClient)
        {
            WSData.StreamServiceRequest inValue = new WSData.StreamServiceRequest();
            inValue.NorisID = NorisID;
            inValue.action = action;
            inValue.fromClient = fromClient;
            return ((WSData.DataSoap)(this)).StreamServiceAsync(inValue);
        }
        
        public System.Threading.Tasks.Task<string> ConnectAsync(string connectionData)
        {
            return base.Channel.ConnectAsync(connectionData);
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
            if ((endpointConfiguration == EndpointConfiguration.DataSoap))
            {
                System.ServiceModel.BasicHttpBinding result = new System.ServiceModel.BasicHttpBinding();
                result.MaxBufferSize = int.MaxValue;
                result.ReaderQuotas = System.Xml.XmlDictionaryReaderQuotas.Max;
                result.MaxReceivedMessageSize = int.MaxValue;
                result.AllowCookies = true;
                return result;
            }
            if ((endpointConfiguration == EndpointConfiguration.DataSoap12))
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
            if ((endpointConfiguration == EndpointConfiguration.DataSoap))
            {
                return new System.ServiceModel.EndpointAddress("http://camel/Source99-E5A1/Data.asmx");
            }
            if ((endpointConfiguration == EndpointConfiguration.DataSoap12))
            {
                return new System.ServiceModel.EndpointAddress("http://camel/Source99-E5A1/Data.asmx");
            }
            throw new System.InvalidOperationException(string.Format("Could not find endpoint with name \'{0}\'.", endpointConfiguration));
        }
        
        public enum EndpointConfiguration
        {
            
            DataSoap,
            
            DataSoap12,
        }
    }
}
