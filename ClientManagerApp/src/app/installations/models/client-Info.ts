import { ClientConfig } from './client-config';
import { UpgradeInfo } from './upgrade-info.enum';

export class ClientInfo {
    clientId: string;
    clientName: string;
    installDir: string;
    extensions: string[];
    config: ClientConfig;
    userName: string;

    upgradeInfo: UpgradeInfo;
    currentProcessId: string;
}
