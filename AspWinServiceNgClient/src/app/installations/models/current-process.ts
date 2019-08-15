export interface CurrentProcess {
    processId: string;
    processType: CurrentProcessType;
    progress: number;
    log: CurrentProcessLogItem[];
    result: boolean;
}

export interface CurrentProcessLogItem {
    logItemId: string;
    content: string;
}

export enum CurrentProcessType {
    installation = 'installation',
    upgrade = 'upgrade'
}
