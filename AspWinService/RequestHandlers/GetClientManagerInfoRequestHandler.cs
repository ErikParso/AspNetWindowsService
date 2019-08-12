using AspWinService.Model;
using AspWinService.Requests;
using MediatR;
using Newtonsoft.Json;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace AspWinService.RequestHandlers
{
    public class GetClientManagerInfoRequestHandler : IRequestHandler<GetClientManagerInfoRequest, ClientManagerInfo>
    {
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
