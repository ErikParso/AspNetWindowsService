import { ClientConfig } from './client-config';

export class ClientInfo {
    clientId: string;
    clientName: string;
    installDir: string;
    extensions: string[];
    config: ClientConfig;

    needUpgrade: boolean;
    currentProcessId: string;
}
