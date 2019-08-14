using NorisWin32Update;
using System;
using System.ServiceModel;
using System.Text;
using WSData;
using WSUpdate;

namespace ApplicationServerTestRequests
{
    class Program
    {
        private const string clientRedirectAddress = "http://camel/Source99-E5/Data.asmx";
        private const string installDir = @"C:\Users\eparso\Desktop\installDir";
        private const string tempDir = @"C:\Users\eparso\Desktop\tempDir";
        private const string clientName = "Test Client 1";
        private const string language = "SK";

        static void Main(string[] args)
        {
            Test();
            Console.ReadLine();
        }

        static async void Test()
        {
            // get application server address
            var client = new DataSoapClient(DataSoapClient.EndpointConfiguration.DataSoap);
            client.Endpoint.Address = new EndpointAddress(clientRedirectAddress);
            var applicationServer = (await client.GetInfoAsync("GETREDIRECTINFO", string.Empty)).Body.GetInfoResult;
            Console.WriteLine($"ApplicationServer: {applicationServer}");

            var downloader = new Downloader(tempDir, installDir, applicationServer);
            await downloader.DownloadNowAsync();
        }
    }
}
