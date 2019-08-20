export interface CurrentProcess {
    processId: string;
    processType: CurrentProcessType;
    progress: number;
    log: CurrentProcessLogItem[];
    result: CurrentProcessResult;
}

export interface CurrentProcessLogItem {
    logItemId: string;
    content: string;
}

export enum CurrentProcessType {
    installation = 'installation',
    upgrade = 'upgrade',
    delete = 'delete'
}

export enum CurrentProcessResult {
    running = 'process is running',
    success = 'success',
    error = 'process failed'
}
