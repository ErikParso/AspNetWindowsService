import { Injectable } from '@angular/core';
import { map, mergeMap } from 'rxjs/operators';
import { createEffect, ofType, Actions } from '@ngrx/effects';
import { VersionService } from './version.service';
import { loadVersions, loadVersionsSuccess } from './app.reducer';

@Injectable()
export class AppEffects {

  loadVersions$ = createEffect(() => this.actions$.pipe(
    ofType(loadVersions),
    mergeMap(() => this.versionService.getServiceVersions()
      .pipe(
        map(versionInfo => loadVersionsSuccess(versionInfo))
      ))
  ));

  constructor(
    private actions$: Actions,
    private versionService: VersionService
  ) { }
}
