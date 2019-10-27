import { Injectable } from '@angular/core';
import { ClientConfigItem } from '../models/client-config-item';
import { Subject, BehaviorSubject } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class InstallConfigService {

  private defaultConfigValues: ClientConfigItem[] = [];

  private defaultConfigValuesSubject = new BehaviorSubject<ClientConfigItem[]>(this.defaultConfigValues);

  public defaultConfigValues$ = this.defaultConfigValuesSubject.asObservable();

  constructor() { }

  public addConfigItem(item: ClientConfigItem) {
    this.defaultConfigValues = this.defaultConfigValues.filter(c => c.section !== item.section || c.key !== item.key);
    this.defaultConfigValues = this.defaultConfigValues.concat(item);
    this.defaultConfigValuesSubject.next(this.defaultConfigValues);
  }

  public removeConfigItem(section: string, key: string) {
    this.defaultConfigValues = this.defaultConfigValues.filter(c => c.section !== section || c.key !== key);
    this.defaultConfigValuesSubject.next(this.defaultConfigValues);
  }

  public clearConfigItems() {
    this.defaultConfigValues = [];
    this.defaultConfigValuesSubject.next(this.defaultConfigValues);
  }

  setConfigItems(items: ClientConfigItem[]) {
    this.defaultConfigValues = items;
    this.defaultConfigValuesSubject.next(this.defaultConfigValues);
  }

  getConfigValue(section: string, key: string): string {
    const item = this.defaultConfigValues.find(c => c.key === key && c.section === section);
    return item ? item.value : '';
  }
}
