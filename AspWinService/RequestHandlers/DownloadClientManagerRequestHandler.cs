using AspWinService.Requests;
using MediatR;
using System.IO;
using System.Net.Http;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace AspWinService.RequestHandlers
{
    public class DownloadClientManagerRequestHandler : IRequestHandler<DownloadClientManagerRequest, string>
    {
        public async Task<string> Handle(DownloadClientManagerRequest request, CancellationToken cancellationToken)
        {
            using (var httpClient = new HttpClient())
            {
                using (var httpRequest = new HttpRequestMessage(HttpMethod.Get, $"{Constants.ClientManagerInfoUrl}/download"))
                {
                    if (!Directory.Exists("Installer"))
                        Directory.CreateDirectory("Installer");

                    using (
                        Stream contentStream = await(await httpClient.SendAsync(httpRequest)).Content.ReadAsStreamAsync(),
                        stream = new FileStream("Installer/MyProduct.msi", FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite))
                    {
                        await contentStream.CopyToAsync(stream);
                    }
                }
            }

            string myExeDir = new FileInfo(Assembly.GetEntryAssembly().Location).Directory.ToString();
            string msiPath = Path.Combine(myExeDir, @"Installer\MyProduct.msi");

            Thread.Sleep(5000);

            return msiPath;
        }
    }
}
