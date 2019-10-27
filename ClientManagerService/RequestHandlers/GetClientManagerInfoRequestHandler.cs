using ClientManagerService.Model;
using ClientManagerService.Requests;
using MediatR;
using Newtonsoft.Json;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace ClientManagerService.RequestHandlers
{
    /// <summary>
    /// Get Client Manager info request handler.
    /// </summary>
    public class GetClientManagerInfoRequestHandler : IRequestHandler<GetClientManagerInfoRequest, ClientManagerInfo>
    {
        /// <summary>
        /// Loads local Client Manager version and Request latest Client Manager version from Production server simulation.
        /// </summary>
        /// <param name="request">Reqeust object.</param>
        /// <param name="cancellationToken">Cancelation token.</param>
        /// <returns>Client manager versions info.</returns>
        public async Task<ClientManagerInfo> Handle(GetClientManagerInfoRequest request, CancellationToken cancellationToken)
        {
            return new ClientManagerInfo()
            {
                Latest = await GetLatestVersion(),
                Local = GetCurrentVersion()
            };
        }

        private async Task<string> GetLatestVersion()
        {
            using (var httpClient = new HttpClient())
                return await httpClient.GetStringAsync(Constants.ClientManagerInfoUrl);
        }

        private string GetCurrentVersion()
            => JsonConvert.DeserializeObject<dynamic>(System.IO.File.ReadAllText("VersionInfo.json")).version;
    }
}
