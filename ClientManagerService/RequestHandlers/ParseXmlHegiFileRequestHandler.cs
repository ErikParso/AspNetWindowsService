using ClientManagerService.Model;
using ClientManagerService.Requests;
using MediatR;
using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace ClientManagerService.RequestHandlers
{
    /// <summary>
    /// Parse Xml .hegi file request handler.
    /// </summary>
    public class ParseXmlHegiFileRequestHandler : IRequestHandler<ParseXmlHegiFileRequest, HegiDescriptor>
    {
        /// <summary>
        /// Parses xml content of .hegi file into <see cref="HegiDescriptor"/> object.
        /// </summary>
        /// <param name="request">Request object.</param>
        /// <param name="cancellationToken">Cancelation token.</param>
        /// <returns>Parsed file content.</returns>
        public Task<HegiDescriptor> Handle(ParseXmlHegiFileRequest request, CancellationToken cancellationToken)
        {
            XDocument xdoc = XDocument.Load(request.HegiFileName);
            var clientInstallNode = xdoc.Descendants("ClientInstall").First();
            var keys = clientInstallNode.Descendants("key");
            var configItems = clientInstallNode.Descendants("config");
            var installScope = GetInstallationScope(
                keys.First(k => k.Attribute("Name").Value == "InstallScope").Attribute("Value").Value);
            return Task.FromResult(new HegiDescriptor()
            {
                ApplicationServer = keys.First(k => k.Attribute("Name").Value == "AppServer").Attribute("Value").Value,
                ClientExists = GetClientExistsAction(
                    keys.First(k => k.Attribute("Name").Value == "ClientExist").Attribute("Value").Value),
                ClientName = keys.First(k => k.Attribute("Name").Value == "NameClient").Attribute("Value").Value,
                ConfigName = keys.First(k => k.Attribute("Name").Value == "ConfigName").Attribute("Value").Value,
                DesktopIcon = bool.Parse(
                    keys.First(k => k.Attribute("Name").Value == "IconInDesktop").Attribute("Value").Value),
                HideWizard = bool.Parse(
                    keys.First(k => k.Attribute("Name").Value == "HideWizard").Attribute("Value").Value),
                InstallDir = installScope == InstallationScope.PerMachine
                    ? Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), Constants.AssecoSolutions, Constants.NorisWin32Clients)
                    : string.Empty,
                InstallScope = installScope,
                Language = string.Empty,
                LnkForAllUser = bool.Parse(
                    keys.First(k => k.Attribute("Name").Value == "LnkForAllUser").Attribute("Value").Value),
                TypeExec = GetTypeExec(
                    keys.First(k => k.Attribute("Name").Value == "TypeExec").Attribute("Value").Value),
                ConfigItems = configItems.Select(c => new ClientConfigItem()
                {
                    Section = c.Attribute("Section").Value,
                    Key = c.Attribute("Name").Value,
                    Value = c.Attribute("Value").Value
                })
            });
        }

        private ClientExistsAction GetClientExistsAction(string value)
        {
            switch (value)
            {
                case "Dialog": return ClientExistsAction.Dialog;
                case "End": return ClientExistsAction.End;
                case "Delete": return ClientExistsAction.Delete;
                default: throw new System.Exception($"Invalid client exists action {value}.");
            }
        }

        private InstallationScope GetInstallationScope(string value)
        {
            switch (value)
            {
                case "PerMachine": return InstallationScope.PerMachine;
                case "PerUser": return InstallationScope.PerUser;
                default: throw new System.Exception($"Invalid install scope {value}.");
            }
        }

        private TypeExec GetTypeExec(string value)
        {
            switch (value)
            {
                case "AddInstall": return TypeExec.AddInstall;
                case "UpdateInstall": return TypeExec.UpdateInstall;
                case "Deleteinstall": return TypeExec.DeleteInstall;
                default: throw new System.Exception($"Invalid type exec option {value}.");
            }
        }
    }
}
