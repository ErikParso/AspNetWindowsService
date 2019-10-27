using ClientManagerService.SignalR.Rpc;
using ClientManagerService.SignalR.RpcHubs;
using ClientManagerService.Support;
using Microsoft.IdentityModel.Tokens;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IdentityModel.Selectors;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.ServiceModel.Description;
using System.ServiceModel.Security;
using System.Threading;

namespace ClientManagerService.Services
{
    /// <summary>
    /// Service to set custom server certificate validator. <seealso cref="CertificateValidator"/>.
    /// </summary>
    public class CertificateValidationService
    {
        private readonly CertificateValidator certificateValidator;

        /// <summary>
        /// Initializes <see cref="CertificateValidationService"/>.
        /// </summary>
        /// <param name="certificateValidator">Custom certificate validator.</param>
        public CertificateValidationService(CertificateValidator certificateValidator)
        {
            this.certificateValidator = certificateValidator;
            this.CanCustomValidateCertificate = true;
        }

        /// <summary>
        /// If set true. Custom server certificate validator wont be set to client.
        /// </summary>
        public bool CanCustomValidateCertificate { get; set; }

        /// <summary>
        /// Sets custom server certificate validator to client credentials.
        /// </summary>
        /// <param name="clientCredentials">Client credentials reference.</param>
        public void SetCertificateValidation(ClientCredentials clientCredentials)
        {
            if (CanCustomValidateCertificate)
            {
                clientCredentials.ServiceCertificate.SslCertificateAuthentication = new X509ServiceCertificateAuthentication()
                {
                    CertificateValidationMode = X509CertificateValidationMode.Custom,
                    CustomCertificateValidator = certificateValidator
                };
            }
        }
    }

    /// <summary>
    /// Custom server certificate validatior.
    /// Uses <see cref="RpcHub{Q, S}"/> to notify server certificate problems to client.
    /// Waits for client to accept or reject certificate.
    /// Users choice will be cached for request scope or application lifetime.
    /// </summary>
    public class CertificateValidator : X509CertificateValidator
    {
        private readonly RpcHub<AcceptCertificateRpcRequest, AcceptCertificateRpcResponse> rpcHub;
        private readonly ConcurrentDictionary<string, bool> requestScopeCertProblemsIgnoreList;

        /// <summary>
        /// ToDo: make it persistently.
        /// </summary>
        private static HashSet<string> appScopeCertProblemsIgnoreList = new HashSet<string>();

        /// <summary>
        /// Initializes <see cref="CertificateValidator"/>
        /// </summary>
        /// <param name="rpcHub">Rpc hub used to notify client to accept ivalid cert.</param>
        public CertificateValidator(RpcHub<AcceptCertificateRpcRequest, AcceptCertificateRpcResponse> rpcHub)
        {
            this.rpcHub = rpcHub;
            requestScopeCertProblemsIgnoreList = new ConcurrentDictionary<string, bool>();
        }

        /// <summary>
        /// certificate validation process.
        /// </summary>
        /// <param name="certificate"></param>
        public override void Validate(X509Certificate2 certificate)
        {
            lock (requestScopeCertProblemsIgnoreList)
            {
                var certHashString = certificate.GetCertHashString();

                if (requestScopeCertProblemsIgnoreList.TryGetValue(certHashString, out bool acceptCert))
                {
                    if (acceptCert)
                        return;
                    else
                        throw new SecurityTokenValidationException("Service certification is not valid.");
                }

                if (appScopeCertProblemsIgnoreList.Contains(certHashString))
                    return;

                X509Chain chain = new X509Chain();
                if (!chain.Build(certificate))
                {
                    var acceptCertRequest = new AcceptCertificateRpcRequest()
                    {
                        Problems = chain.ChainStatus.Select(s => s.StatusInformation),
                        Issuer = certificate.GetNameInfo(X509NameType.SimpleName, true),
                        Subject = certificate.GetNameInfo(X509NameType.SimpleName, false),
                        ValidFrom = certificate.NotBefore,
                        ValidTill = certificate.NotAfter
                    };

                    var acceptCertResponse = AsyncHelpers.RunSync(() =>
                        rpcHub.RequestClient(acceptCertRequest));

                    if (acceptCertResponse == null || !acceptCertResponse.AcceptCertificate)
                    {
                        requestScopeCertProblemsIgnoreList.TryAdd(certHashString, false);
                        throw new SecurityTokenValidationException("Service certification is not valid.");
                    }
                    else
                    {
                        requestScopeCertProblemsIgnoreList.TryAdd(certHashString, true);
                        if (acceptCertResponse.DontAskAgain)
                            appScopeCertProblemsIgnoreList.Add(certHashString);
                    }


                }
            }
        }
    }
}
