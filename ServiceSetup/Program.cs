using System;
using System.Linq;
using WixSharp;

namespace ServiceSetup
{
    class Program
    {
        private const string serviceFiles = @"C:\Users\eparso\source\repos\AspWinService\AspWinService\bin\Release\netcoreapp2.2\win7-x64\publish";
        private const string clientFiles = @"C:\Users\eparso\source\repos\AspWinService\AspWinServiceClient\bin\Debug\netcoreapp3.0";

        static void Main()
        {
            System.Diagnostics.Debugger.Break();

            File service;

            var project = new Project("MyProduct",
                              new Dir(@"%ProgramFiles%\My Company\My Product",
                                new Dir("Client", 
                                    new Files(System.IO.Path.Combine(clientFiles, "*.*"))),
                                new Dir("Service",
                                    new Files(System.IO.Path.Combine(serviceFiles, "*.*"), f => !f.EndsWith("AspWinService.exe")),
                                    service = new File(System.IO.Path.Combine(serviceFiles, "AspWinService.exe")))))
            {
                Platform = Platform.x64,
                GUID = new Guid("6fe30b47-2577-43ad-9095-1861ba25889b")
            };

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

            project.BuildMsi();
        }
    }
}