export class AcceptCertificateRpcRequest {
    problems: string[];
    issuer: string;
    subject: string;
    validFrom: Date;
    validTill: Date;
}
