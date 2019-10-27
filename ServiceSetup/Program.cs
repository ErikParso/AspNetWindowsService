using Microsoft.Deployment.WindowsInstaller;
using Newtonsoft.Json;
using System;
using System.Diagnostics;
using System.IO;
using WixSharp;
using WixSharp.CommonTasks;

namespace ServiceSetup
{
    class Program
    {
        private const string serviceFiles = @"C:\Users\eparso\source\repos\ClientUpdateService\ClientManagerService\bin\Debug\netcoreapp3.0\win7-x64";
        private const string electronClientFiles = @"C:\Users\eparso\source\repos\ClientUpdateService\ClientManagerApp\ClientManagerApp-win32-x64";
        private const string credentialsListenerFiles = @"C:\Users\eparso\source\repos\ClientUpdateService\CredentialsListener\bin\Debug";
        private const string msiDeployPath = @"C:\Users\eparso\source\repos\ClientUpdateService\HgProductionServer\bin\Release\netcoreapp2.2\win7-x64\publish";

        public const string serviceAppFileName = "ClientManagerApp.exe";
        private const string uriScheme = "heliosGreenService";
        private const string friendlyName = "Helios Green Service Protocol";
        private const string projectName = "Helios Green Client Manager";

        static void Main()
        {

            var serviceVersionInfo = Path.Combine(serviceFiles, "VersionInfo.json");
            var serviceVersion = JsonConvert.DeserializeObject<VersionInfo>(System.IO.File.ReadAllText(serviceVersionInfo)).Version;

            var project = new Project(projectName,
                              new Dir(@"%ProgramFiles%\Asseco Solutions\Client Manager",                               
                                new Dir("Client Manager App",
                                    new Files(Path.Combine(electronClientFiles, "*.*"))),
                                new Dir("Client Manager Service",
                                    new Files(Path.Combine(serviceFiles, "*.*"))),
                                new Dir("Credentials Listener",
                                    new Files(Path.Combine(credentialsListenerFiles, "*.*")))),
                              new RegValue(RegistryHive.LocalMachine, $@"SOFTWARE\Classes\{uriScheme}", string.Empty, $"URL: {friendlyName}"),
                              new RegValue(RegistryHive.LocalMachine, $@"SOFTWARE\Classes\{uriScheme}", "URL Protocol", string.Empty),
                              new RegValue(RegistryHive.LocalMachine, $@"SOFTWARE\Classes\{uriScheme}\DefaultIcon", string.Empty, $@"[INSTALLDIR]Client Manager App\{serviceAppFileName}"),
                              new RegValue(RegistryHive.LocalMachine, $@"SOFTWARE\Classes\{uriScheme}\shell\open\command", string.Empty, $"\"[INSTALLDIR]Client Manager App\\{serviceAppFileName}\" \"%1\""),
                              new RegValue(RegistryHive.CurrentUser, $@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run", "ClientManagerApp", $@"[INSTALLDIR]Client Manager App\{serviceAppFileName}"))
            {
                Platform = Platform.x64,
                GUID = new Guid("6fe30b47-2577-43ad-9095-1861ba25889d"),
                Version = new Version(serviceVersion),                                                              
                MajorUpgradeStrategy = MajorUpgradeStrategy.Default
            };

            project.RebootSupressing = RebootSupressing.ReallySuppress;
            project.ResolveWildCards();

            var service = project.FindFirstFile("ClientManagerService.exe");
            service.ServiceInstaller = new ServiceInstaller
            {
                Name = "ClientManagerService",
                StartOn = SvcEvent.Install_Wait,
                StopOn = SvcEvent.InstallUninstall_Wait,
                RemoveOn = SvcEvent.Uninstall_Wait,
                DelayedAutoStart = false,
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

            project.AddAction(new ManagedAction(CustomActions.RunClientAfterInstall, Return.check, When.After, Step.InstallFinalize, Condition.NOT_Installed));

            project.BuildMsi();

            if (!Directory.Exists(Path.Combine(msiDeployPath, "Installer")))
                Directory.CreateDirectory(Path.Combine(msiDeployPath, "Installer"));

            // Move msi to deploy path (to application server)
            var sourceFile = Path.Combine(Directory.GetCurrentDirectory(), $"{projectName}.msi");
            var destFile = Path.Combine(msiDeployPath, $"Installer\\{projectName}.msi");
            System.IO.File.Delete(destFile);
            System.IO.File.Move(sourceFile, destFile);

            // Update version info
            var versionInfo = Path.Combine(msiDeployPath, "Installer\\VersionInfo.json");
            System.IO.File.Delete(versionInfo);
            System.IO.File.Copy(serviceVersionInfo, versionInfo);
        }

        private class VersionInfo
        {
            public string Version { get; set; }
        }

    }

    public class CustomActions
    {
        [CustomAction]
        public static ActionResult RunClientAfterInstall(Session session)
        {
            Debugger.Break();
            System.Diagnostics.Process process = new System.Diagnostics.Process();
            process.StartInfo.FileName = Path.Combine(session["INSTALLDIR"], "Client Manager App", Program.serviceAppFileName);
            process.StartInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Normal;
            process.Start();
            return ActionResult.Success;
        }
    }
}