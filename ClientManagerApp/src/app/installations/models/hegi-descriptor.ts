import { ClientConfigItem } from './client-config-item';

export class HegiDescriptor {
    typeExec: TypeExec;
    hideWizard: boolean;
    clientName: string;
    language: string;
    installDir: string;
    clientExists: ClientExistsAction;
    applicationServer: string;
    configName: string;
    lnkForAllUser: boolean;
    desktopIcon: boolean;
    installScope: InstallationScope;
    configItems: ClientConfigItem[];
}

export enum TypeExec {
    addInstall,
    updateInstall,
    deleteInstall
}

export enum ClientExistsAction {
    dialog,
    end,
    delete
}

export enum InstallationScope {
    perMachine,
    perUser
}
