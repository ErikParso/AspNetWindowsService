import { Routes, RouterModule } from '@angular/router';
import { SettingsComponent } from './settings.component';
import { ProxyComponent } from './proxy/proxy.component';

const routes: Routes = [
  {
    path: '',
    component: SettingsComponent,
    children: [
      {
        path: 'proxy',
        component: ProxyComponent,
        pathMatch: 'full'
      },
      {
        path: '',
        redirectTo: 'proxy',
        pathMatch: 'full'
      }
    ]
  }
];

export const SettingsRoutes = RouterModule.forChild(routes);
