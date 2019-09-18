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
    addInstall = 'add new installation',
    updateInstall = 'actualize existing installation',
    deleteInstall = 'delete existing installation'
}

export enum ClientExistsAction {
    dialog = 'delete existing client dialog',
    end = 'end installation',
    delete = 'delete existing client'
}

export enum InstallationScope {
    perMachine = 'installation for all users',
    perUser = 'installation for current user'
}
