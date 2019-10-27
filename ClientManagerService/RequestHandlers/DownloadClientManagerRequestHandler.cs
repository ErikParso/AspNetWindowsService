using ClientManagerService.Requests;
using MediatR;
using System.IO;
using System.Net.Http;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace ClientManagerService.RequestHandlers
{
    /// <summary>
    /// Download Client Manager from Production Server request handler.
    /// </summary>
    public class DownloadClientManagerRequestHandler : IRequestHandler<DownloadClientManagerRequest, string>
    {
        /// <summary>
        /// Downloads Client Manager installer from Production Server simulation 
        /// and saves it in Cleint Manager Service Installer folder.
        /// </summary>
        /// <param name="request">Request object.</param>
        /// <param name="cancellationToken">Cancelation token.</param>
        /// <returns>Path to the downloaded installer.</returns>
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
                        stream = new FileStream("Installer/Helios Green Client Manager.msi", FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite))
                    {
                        await contentStream.CopyToAsync(stream);
                    }
                }
            }

            string myExeDir = new FileInfo(Assembly.GetEntryAssembly().Location).Directory.ToString();
            string msiPath = Path.Combine(myExeDir, @"Installer\Helios Green Client Manager.msi");

            Thread.Sleep(5000);

            return msiPath;
        }
    }
}
