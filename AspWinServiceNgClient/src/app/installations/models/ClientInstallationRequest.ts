import { ClientConfigItem } from './client-config-item';

export class ClientInstallationRequest {
    installationProcessId: string;
    clientId: string;
    clientName: string;
    language: string;
    installDir: string;
    applicationServer: string;
    configName: string;
    lnkForAllUser: boolean;
    desktopIcon: boolean;
    installForAllUsers: boolean;
    configItems: ClientConfigItem[];
    runAfterInstall: boolean;
}
