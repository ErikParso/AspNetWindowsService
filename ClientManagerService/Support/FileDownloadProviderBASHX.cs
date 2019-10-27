using ClientManagerService.Services;
using SimmoTech.Utils.Serialization;
using System;
using System.Collections.Generic;
using System.Net;
using System.ServiceModel;
using System.Threading.Tasks;
using WSUpdate;
using static ClientManagerService.Services.UpdateProcessorService;

namespace ClientManagerService.Support
{
    public class FileDownloadProviderBASHX : FileDownloadProvider
    {
        private readonly ClientUpdateSoapClient wsUpdate;

        public FileDownloadProviderBASHX(string targetFileName, ExecutePlanItem executePlanItem, string logItemKey, ProgressService progressService, ClientUpdateSoapClient wsUpdate) 
            : base(targetFileName, executePlanItem, logItemKey, progressService)
        {
            this.wsUpdate = wsUpdate;
        }

        protected override async Task<LoadClientFileDescriptor> CallDownloadRequest(string flags)
        {
            //BASHX approach uses bdata.ashx service for binary communication
            string url = wsUpdate.Endpoint.Address.Uri.AbsoluteUri.Replace("ClientUpdate", "BData").Replace("asmx", "ashx");
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";
            request.AllowWriteStreamBuffering = true;
            request.KeepAlive = true;
            request.Credentials = wsUpdate.ClientCredentials.Windows.ClientCredential;
            var binding = wsUpdate.Endpoint.Binding as BasicHttpBinding;
            request.Proxy = new WebProxy()
            {
                Address = binding.ProxyAddress,
                BypassProxyOnLocal = binding.BypassProxyOnLocal,
                UseDefaultCredentials = binding.UseDefaultWebProxy
            };
            request.Timeout = (int)(binding.ReceiveTimeout + binding.SendTimeout).TotalMilliseconds;
            const byte requestVersion = 2;
            const string requestAction = "GetClientFile";

            using (SerializationWriter writer = new SerializationWriter(request.GetRequestStream()))
            {
                writer.Write(requestVersion);                //first byte is always protocol version.
                writer.WriteStringDirect(requestAction);     //action string then
                Dictionary<string, string> args = new Dictionary<string, string>();
                args.Add("ClientAuthor", ExecutePlanItem.ClientIdentity.Author);
                args.Add("ClientName", ExecutePlanItem.ClientIdentity.Name);
                args.Add("FileName", ExecutePlanItem.File.FileName);
                args.Add("Flags", flags);
                if (ExecutePlanItem.PluginIdentity != null)
                {
                    args.Add("PluginAuthor", ExecutePlanItem.PluginIdentity.Author);
                    args.Add("PluginName", ExecutePlanItem.PluginIdentity.Name);
                }
                writer.Write<string, string>(args);
            }

            var response = await request.GetResponseAsync();

            //READ RESULT
            SerializationReader reader = new SimmoTech.Utils.Serialization.SerializationReader(response.GetResponseStream());
            byte responseVersion = reader.ReadByte(); //first byte represents the version of response
            if (responseVersion != requestVersion)
            {
                using (var xreader = new System.IO.StreamReader(response.GetResponseStream()))
                {
                    xreader.Read();
                    xreader.Read();
                    var responseAsString = await xreader.ReadToEndAsync();
                    throw new Exception(responseAsString);
                }
            }
            string responseAction = reader.ReadStringDirect(); //read action name
            if (responseAction != requestAction) return null;
            Dictionary<string, string> inputArgs = reader.ReadDictionary<string, string>();
            WSUpdate.LoadClientFileDescriptor fd = new WSUpdate.LoadClientFileDescriptor();
            fd.Data = reader.ReadByteArray();

            fd.Name = inputArgs["Name"];
            fd.Flags = inputArgs["Flags"];
            fd.ZipUsed = inputArgs["ZipUsed"] == "1";
            fd.ErrorMessage = inputArgs["Error"];
            fd.Modified = DateTime.FromFileTimeUtc(Convert.ToInt64(inputArgs["Modified"]));
            return fd;
        }
    }
}
