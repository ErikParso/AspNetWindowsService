using System.IO;
using System.IO.Compression;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using AspWinService.Model;
using AspWinService.Requests;
using AspWinService.Services;
using MediatR;
using Newtonsoft.Json;

namespace AspWinService.RequestHandlers
{
    public class ClientUpdateRequestHandler : IRequestHandler<ClientUpdateRequest, ClientInfo>
    {
        private readonly ClientInfoService clientInfoService;

        public ClientUpdateRequestHandler(ClientInfoService clientInfoService)
        {
            this.clientInfoService = clientInfoService;
        }

        public async Task<ClientInfo> Handle(ClientUpdateRequest request, CancellationToken cancellationToken)
        {
            ClientInfo ret = null;
            var installDir = request.InstallDir;

            if (Directory.Exists(installDir))
                Directory.Delete(installDir, true);

            Directory.CreateDirectory(installDir);

            var zipPath = Path.Combine(installDir, "HeliosGreen.zip");

            // Download zipped client.
            using (var httpClient = new HttpClient())
                using (var httpRequest = new HttpRequestMessage(HttpMethod.Get, $"http://localhost:5100/api/client/download"))
                    using (
                        Stream contentStream = await(await httpClient.SendAsync(httpRequest)).Content.ReadAsStreamAsync(),
                        stream = new FileStream(zipPath, FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite))
                    {
                        await contentStream.CopyToAsync(stream);
                    }

            // Unzip client and delete zip.
            ZipFile.ExtractToDirectory(zipPath, installDir);
            File.Delete(zipPath);

            // Save client installation information.
            clientInfoService.ProcessClientInfo(
                clientInfo => clientInfo.InstallDir == installDir,
                clientInfo =>
                {
                    clientInfo.Version = JsonConvert.DeserializeObject<dynamic>(File.ReadAllText(Path.Combine(installDir, "VersionInfo.json"))).version;
                    ret = clientInfo;
                });

            return ret;
        }
    }
}
