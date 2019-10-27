using ClientManagerService.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ClientManagerService.Services
{
    /// <summary>
    /// Thread safe service used to provide Helios Green client installations info and context to work with client information.
    /// </summary>
    public class ClientInfoService
    {
        private const string clientsInfoLock = "Clients Info Lock";

        /// <summary>
        /// Locks installed clients info resource. Reads Helios Green clients collection.
        /// </summary>
        /// <returns>
        /// Helios Green client installations collection.
        /// </returns>
        public IEnumerable<ClientInfo> GetClientsInfo()
        {
            IEnumerable<ClientInfo> result = null;
            ProcessClientsInfo((clientsInfo) => { result = clientsInfo; });
            return result;
        }

        /// <summary>
        /// Locks installed clients info resource. Reads specific Helios Green client info.
        /// </summary>
        /// <param name="clientId">Helios Green client id.</param>
        /// <returns>
        /// Helios Green client info.
        /// </returns>
        public ClientInfo GetClientInfo(string clientId)
        {
            ClientInfo result = null;
            ProcessClientInfo(
                c => c.ClientId == clientId,
                clientInfo => { result = clientInfo; });
            return result;
        }

        /// <summary>
        /// Locks installed clients info resource. Reads info for client specified by predicate.
        /// </summary>
        /// <param name="selector">Helios Green client selector.</param>
        /// <returns>
        /// Helios Green client info.
        /// </returns>
        public ClientInfo GetClientInfo(Func<ClientInfo, bool> selector)
        {
            ClientInfo result = null;
            ProcessClientInfo(selector, clientInfo => { result = clientInfo; });
            return result;
        }

        /// <summary>
        /// Locks installed clients info resource. Adds new Helios Green client info.
        /// </summary>
        /// <param name="clientInfo">Helios Green client info.</param>
        public void AddClientInfo(ClientInfo clientInfo)
        {
            ProcessClientsInfo(clients => clients.Add(clientInfo));
        }

        /// <summary>
        /// Locks installed clients info resource.
        /// Deletes Helios Green client info specified by predicate.
        /// </summary>
        /// <param name="selector">Helios Green client selector.</param>
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

        /// <summary>
        /// Locks installed client info resource.
        /// Finds client info with specific id and process it using <paramref name="process"/> action.
        /// </summary>
        /// <param name="clientId">Client installation id.</param>
        /// <param name="process">Process client info action.</param>
        public void ProcessClientInfo(string clientId, Action<ClientInfo> process)
        {
            ProcessClientInfo(c => c.ClientId == clientId, process);
        }

        /// <summary>
        /// Locks installed client info resource.
        /// Finds client info using <paramref name="selector"/> predicate and process it using <paramref name="process"/> action.
        /// </summary>
        /// <param name="selector">Client info selector.</param>
        /// <param name="process">Process client info action.</param>
        public void ProcessClientInfo(Func<ClientInfo, bool> selector, Action<ClientInfo> process)
        {
            ProcessClientsInfo(clientsInfoList =>
            {
                var clientInfo = clientsInfoList.FirstOrDefault(selector);
                process(clientInfo);
            });
        }

        /// <summary>
        /// Locks installed client info resource.
        /// Reads all client installations info collection and process it using <paramref name="processClientsInfo"/> action.
        /// </summary>
        /// <param name="processClientsInfo">Process client list action.</param>
        public void ProcessClientsInfo(Action<List<ClientInfo>> processClientsInfo)
        {
            lock (clientsInfoLock)
            {
                var clientsInfoString = File.ReadAllText(Constants.InstalledClientsFileName);
                var clientsInfo = JsonConvert.DeserializeObject<IEnumerable<ClientInfo>>(clientsInfoString).ToList();
                processClientsInfo(clientsInfo);
                File.WriteAllText(Constants.InstalledClientsFileName, JsonConvert.SerializeObject(clientsInfo, Formatting.Indented));
            }
        }
    }
}
