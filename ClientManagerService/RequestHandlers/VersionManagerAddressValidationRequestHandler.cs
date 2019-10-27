using ClientManagerService.Model;
using ClientManagerService.Requests;
using ClientManagerService.Services;
using MediatR;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace ClientManagerService.RequestHandlers
{
    /// <summary>
    /// Version Manager address validation request handler.
    /// </summary>
    public class VersionManagerAddressValidationRequestHandler : IRequestHandler<VersionManagerAddressValidationRequest, ValidationResult>
    {
        private readonly RedirectService redirectService;

        /// <summary>
        /// Initializes VersionManagerAddressValidationRequestHandler
        /// </summary>
        /// <param name="redirectService">Cleint redisrect service</param>
        public VersionManagerAddressValidationRequestHandler(RedirectService redirectService)
        {
            this.redirectService = redirectService;
        }

        /// <summary>
        /// Validates Client Manager address. Tries to connect Client Manager and get client redirect uri (Application Server address).
        /// Then tries to querry languages from application server.
        /// </summary>
        /// <param name="request">Request object.</param>
        /// <param name="cancellationToken">Cancelation token.</param>
        /// <returns>
        /// Validatuon result with message. 
        /// Message contains languages list in case validation is successfull.
        /// </returns>
        public async Task<ValidationResult> Handle(VersionManagerAddressValidationRequest request, CancellationToken cancellationToken)
        {
            var result = new ValidationResult();

            var config = new ClientConfig()
            {
                ApplicationServer = request.VersionManagerAddress,
                Items = request.ConfigItems
            };

            if (request.ConfigItems != null && request.ConfigItems.Any(c => c.Section == "test" && c.Key == "invalid_application_server" && c.Value == "1"))
            {
                result.IsValid = false;
                result.Message = "invalid_application_server is specified in configuration step 2 section test.";
                return result;
            }

            try
            {
                var applicationserverAddress = await redirectService.GetClientRedirectAddress(config);
                if (!string.IsNullOrWhiteSpace(applicationserverAddress))
                {
                    var languagesXml = await redirectService.GetAvailableLanguages(config);
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
