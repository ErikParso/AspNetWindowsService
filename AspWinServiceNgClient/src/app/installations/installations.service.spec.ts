/* tslint:disable:no-unused-variable */

import { TestBed, async, inject } from '@angular/core/testing';
import { InstallationsService } from './installations.service';

describe('Service: Installations', () => {
  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [InstallationsService]
    });
  });

  it('should ...', inject([InstallationsService], (service: InstallationsService) => {
    expect(service).toBeTruthy();
  }));
});
