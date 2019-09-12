using Newtonsoft.Json;
using System;
using System.IO;
using WixSharp;

namespace ServiceSetup
{
    class Program
    {
        private const string serviceFiles = @"C:\Users\eparso\source\repos\AspWinService\AspWinService\bin\Debug\netcoreapp2.2\win7-x64";
        private const string electronClientFiles = @"C:\Users\eparso\source\repos\AspWinService\AspWinServiceNgClient\asp-win-service-ng-client-win32-x64";
        private const string msiDeployPath = @"C:\Users\eparso\source\repos\AspWinService\ApplicationServer\bin\Release\netcoreapp2.2\win7-x64\publish\Installer";

        private const string serviceAppFileName = "asp-win-service-ng-client.exe";
        private const string uriScheme = "heliosGreenService";
        private const string friendlyName = "Helios Green Service Protocol";

        static void Main()
        {
            //System.Diagnostics.Debugger.Break();

            var serviceVersionInfo = Path.Combine(serviceFiles, "VersionInfo.json");
            var serviceVersion = JsonConvert.DeserializeObject<VersionInfo>(System.IO.File.ReadAllText(serviceVersionInfo)).Version;

            var project = new Project("MyProduct",
                              new Dir(@"%ProgramFiles%\My Company\My Product",                               
                                new Dir("NgClient",
                                    new Files(Path.Combine(electronClientFiles, "*.*"))),
                                new Dir("Service",
                                    new Files(Path.Combine(serviceFiles, "*.*")))),
                              new RegValue(RegistryHive.LocalMachine, $@"SOFTWARE\Classes\{uriScheme}", string.Empty, $"URL: {friendlyName}"),
                              new RegValue(RegistryHive.LocalMachine, $@"SOFTWARE\Classes\{uriScheme}", "URL Protocol", string.Empty),
                              new RegValue(RegistryHive.LocalMachine, $@"SOFTWARE\Classes\{uriScheme}\DefaultIcon", string.Empty, $@"[INSTALLDIR]NgClient\{serviceAppFileName}"),
                              new RegValue(RegistryHive.LocalMachine, $@"SOFTWARE\Classes\{uriScheme}\shell\open\command", string.Empty, $"\"[INSTALLDIR]NgClient\\{serviceAppFileName}\" \"%1\""))
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

            var client = project.FindFirstFile(serviceAppFileName);
            client.Associations = new[] { new FileAssociation("hegi") };

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