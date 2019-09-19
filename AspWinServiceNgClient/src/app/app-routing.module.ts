import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';

const routes: Routes = [
  {
    path: 'installations',
    loadChildren: () => import('./installations/installations.module').then(mod => mod.InstallationsModule)
  },
  {
    path: '',
    redirectTo: 'installations',
    pathMatch: 'full'
  }
];

@NgModule({
  imports: [RouterModule.forRoot(routes, { enableTracing: false })],
  exports: [RouterModule]
})
export class AppRoutingModule { }
