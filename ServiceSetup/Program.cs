using System;
using System.Linq;
using WixSharp;

// DON'T FORGET to update NuGet package "WixSharp".
// NuGet console: Update-Package WixSharp
// NuGet Manager UI: updates tab

namespace ServiceSetup
{
    class Program
    {
        static void Main()
        {
            System.Diagnostics.Debugger.Break();

            File service;

            var project = new Project("MyProduct",
                              new Dir(@"%ProgramFiles%\My Company\My Product",
                                new Dir("Client", 
                                    new Files(@"Content\Client\*.*")),
                                new Dir("Service",
                                    new Files(@"Content\Service\*.*", f => !f.EndsWith("AspWinService.exe")),
                                    service = new File(@"Content\Service\AspWinService.exe"))))
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