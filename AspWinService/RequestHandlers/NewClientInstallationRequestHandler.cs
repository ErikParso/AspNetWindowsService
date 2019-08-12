using AspWinService.Model;
using AspWinService.Requests;
using AspWinService.Services;
using MediatR;
using Newtonsoft.Json;
using System.IO;
using System.IO.Compression;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace AspWinService.RequestHandlers
{
    public class NewClientInstallationRequestHandler : IRequestHandler<NewClientInstallationRequest, ClientInfo>
    {
        private readonly ClientInfoService clientInfoService;

        public NewClientInstallationRequestHandler(ClientInfoService clientInfoService)
        {
            this.clientInfoService = clientInfoService;
        }

        public async Task<ClientInfo> Handle(NewClientInstallationRequest request, CancellationToken cancellationToken)
        {
            var installDir = Path.Combine(request.InstallDir, request.ClientName);

            if (!Directory.Exists(installDir))
                Directory.CreateDirectory(installDir);

            var zipPath = Path.Combine(installDir, "HeliosGreen.zip");

            // Download zipped client.
            using (var httpClient = new HttpClient())
                using (var httpRequest = new HttpRequestMessage(HttpMethod.Get, Constants.ClientDownloadUrl))
                    using (
                        Stream contentStream = await(await httpClient.SendAsync(httpRequest)).Content.ReadAsStreamAsync(),
                        stream = new FileStream(zipPath, FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite))
                    {
                        await contentStream.CopyToAsync(stream);
                    }

            // Unzip client and delete zip.
            ZipFile.ExtractToDirectory(zipPath, installDir);
            System.IO.File.Delete(zipPath);

            // Save client installation information.
            var version = JsonConvert.DeserializeObject<dynamic>(File.ReadAllText(Path.Combine(installDir, "VersionInfo.json"))).version;
            var clientInfo = new ClientInfo()
            {
                ClientName = request.ClientName,
                InstallDir = installDir,
                Version = version
            };
            clientInfoService.AddClientInfo(clientInfo);

            return clientInfo;
        }
    }
}
