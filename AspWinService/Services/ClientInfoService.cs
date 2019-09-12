using AspWinService.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace AspWinService.Services
{
    public class ClientInfoService
    {
        private const string clientsInfoLock = "Clients Info Lock";

        public IEnumerable<ClientInfo> GetClientsInfo()
        {
            IEnumerable<ClientInfo> result = null;
            ProcessClientsInfo((clientsInfo) => { result = clientsInfo; });
            return result;
        }

        public ClientInfo GetClientInfo(string clientId)
        {
            ClientInfo result = null;
            ProcessClientInfo(
                c => c.ClientId == clientId, 
                clientInfo => { result = clientInfo; });
            return result;
        }

        public ClientInfo GetClientInfo(Func<ClientInfo, bool> selector)
        {
            ClientInfo result = null;
            ProcessClientInfo(selector, clientInfo => { result = clientInfo; });
            return result;
        }

        public void AddClientInfo(ClientInfo clientInfo)
        {
            ProcessClientsInfo(clients => clients.Add(clientInfo));
        }

        public IEnumerable<ClientInfo> DeleteClientInfo(Func<ClientInfo, bool> selector)
        {
            IEnumerable<ClientInfo> clientsInfo = null;
            ProcessClientsInfo(clieentsInfo =>
            {
                clientsInfo = clieentsInfo.Where(selector).ToList();
                foreach (var clientInfo in clientsInfo)
                {
                    clieentsInfo.Remove(clientInfo);
                }
            });
            return clientsInfo;
        }

        public void ProcessClientInfo(Func<ClientInfo, bool> selector, Action<ClientInfo> process)
        {
            ProcessClientsInfo(clientsInfoList =>
            {
                var clientInfo = clientsInfoList.FirstOrDefault(selector);
                process(clientInfo);
            });
        }

        public void ProcessClientsInfo(Action<List<ClientInfo>> processClientsInfo)
        {
            lock (clientsInfoLock)
            {
                var clientsInfoString = File.ReadAllText(Constants.InstalledClientsFileName);
                var clientsInfo = JsonConvert.DeserializeObject<IEnumerable<ClientInfo>>(clientsInfoString).ToList();
                processClientsInfo(clientsInfo);
                File.WriteAllText(Constants.InstalledClientsFileName, JsonConvert.SerializeObject(clientsInfo));
            }
        }
    }
}
