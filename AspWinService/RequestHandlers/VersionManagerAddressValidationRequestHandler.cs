using AspWinService.Model;
using AspWinService.Requests;
using AspWinService.Services;
using MediatR;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace AspWinService.RequestHandlers
{
    public class VersionManagerAddressValidationRequestHandler : IRequestHandler<VersionManagerAddressValidationRequest, ValidationResult>
    {
        private readonly RedirectService redirectService;

        public VersionManagerAddressValidationRequestHandler(RedirectService redirectService)
        {
            this.redirectService = redirectService;
        }

        public async Task<ValidationResult> Handle(VersionManagerAddressValidationRequest request, CancellationToken cancellationToken)
        {
            var result = new ValidationResult();

            if (request.ConfigItems != null && request.ConfigItems.Any(c => c.Section == "test" && c.Key == "invalid_application_server" && c.Value == "1"))
            {
                result.IsValid = false;
                result.Message = "invalid_application_server is specified in configuration step 2 section test.";
                return result;
            }

            try
            {
                var applicationserverAddress = await redirectService.GetApplicationServerAddress(request.VersionManagerAddress);
                if (!string.IsNullOrWhiteSpace(applicationserverAddress))
                {
                    var languagesXml = await redirectService.GetAvailableLanguages(request.VersionManagerAddress);
                    var languages = ParseLanguages(languagesXml);
                    result.IsValid = true;
                    result.Message = JsonConvert.SerializeObject(
                        new { Languages = languages, RedirectAddress = applicationserverAddress });
                }
                else
                {
                    throw new Exception("Version manager returns empty application server address. Please, set correct version manager address.");
                }
            }
            catch (Exception e)
            {
                result.IsValid = false;
                result.Message = $"Cannot redirect to application server: {e.Message}";
            }

            return result;
        }

        private IEnumerable<string> ParseLanguages(string xml) {
            var xdOc = XDocument.Parse(xml);
            return xdOc.Descendants("language").Select(el => el.Attribute("code").Value).ToList();
        }
    }
}
