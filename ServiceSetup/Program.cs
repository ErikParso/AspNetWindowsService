using Newtonsoft.Json;
using System;
using System.IO;
using WixSharp;

namespace ServiceSetup
{
    class Program
    {
        private const string serviceFiles = @"C:\Users\eparso\source\repos\AspWinService\AspWinService\bin\Debug\netcoreapp2.2\win7-x64";
        //private const string serviceFiles = @"C:\Users\eparso\source\repos\AspWinService\AspWinService\bin\Release\netcoreapp2.2\win7-x64\publish";
        private const string clientFiles = @"C:\Users\eparso\source\repos\AspWinService\AspWinServiceClient\bin\Debug\netcoreapp3.0";
        private const string electronClientFiles = @"C:\Users\eparso\source\repos\AspWinService\AspWinServiceNgClient\asp-win-service-ng-client-win32-x64";
        private const string msiDeployPath = @"C:\Users\eparso\source\repos\AspWinService\ApplicationServer\bin\Debug\netcoreapp3.0\Installer";

        static void Main()
        {
            //System.Diagnostics.Debugger.Break();

            var serviceVersionInfo = Path.Combine(serviceFiles, "VersionInfo.json");
            var serviceVersion = JsonConvert.DeserializeObject<VersionInfo>(System.IO.File.ReadAllText(serviceVersionInfo)).Version;

            var project = new Project("MyProduct",
                              new Dir(@"%ProgramFiles%\My Company\My Product",
                                new Dir("Client", 
                                    new Files(System.IO.Path.Combine(clientFiles, "*.*"))),
                                new Dir("NgClient",
                                    new Files(System.IO.Path.Combine(electronClientFiles, "*.*"))),
                                new Dir("Service",
                                    new Files(System.IO.Path.Combine(serviceFiles, "*.*")))),
                              new RegValue(RegistryHive.LocalMachine, @"SOFTWARE\Microsoft\Windows\CurrentVersion\Run",
                                "AspWinServiceClient1", @"[INSTALLDIR]Client\AspWinServiceClient.exe")
                              {
                                  Win64 = true
                              })
            {
                Platform = Platform.x64,
                GUID = new Guid("6fe30b47-2577-43ad-9095-1861ba25889d"),
                Version = new Version(serviceVersion),                                                              
                MajorUpgradeStrategy = MajorUpgradeStrategy.Default
            };

            project.RebootSupressing = RebootSupressing.ReallySuppress;
            project.ResolveWildCards();

            var service = project.FindFirstFile("AspWinService.exe");
            service.ServiceInstaller = new ServiceInstaller
            {
                Name = "AspWinService",
                StartOn = SvcEvent.Install_Wait,
                StopOn = SvcEvent.InstallUninstall_Wait,
                RemoveOn = SvcEvent.Uninstall_Wait,
                DelayedAutoStart = true,
                ServiceSid = ServiceSid.none,
                FirstFailureActionType = FailureActionType.restart,
                SecondFailureActionType = FailureActionType.restart,
                ThirdFailureActionType = FailureActionType.restart,
                RestartServiceDelayInSeconds = 30,
                ResetPeriodInDays = 1,
                PreShutdownDelay = 1000 * 60 * 3
            };

            var client = project.FindFirstFile("AspWinServiceClient.exe");
            client.Associations = new[] { new FileAssociation("heg") };

            project.BuildMsi();

            // Move msi to deploy path (to application server)
            var sourceFile = Path.Combine(Directory.GetCurrentDirectory(), "MyProduct.msi");
            var destFile = Path.Combine(msiDeployPath, "MyProduct.msi");
            System.IO.File.Delete(destFile);
            System.IO.File.Move(sourceFile, destFile);

            // Update version info
            var versionInfo = Path.Combine(msiDeployPath, "VersionInfo.json");
            System.IO.File.Delete(versionInfo);
            System.IO.File.Copy(serviceVersionInfo, versionInfo);
        }

        private class VersionInfo
        {
            public string Version { get; set; }
        }
    }
}