import { Routes, RouterModule } from '@angular/router';
import { InstallationListComponent } from './installation-list/installation-list.component';
import { InstallationsComponent } from './installations.component';

const routes: Routes = [
  {
    path: '',
    component: InstallationsComponent,
    children: [
      {
        path: '',
        component: InstallationListComponent
      }
    ]
  }
];

export const InstallationsRoutes = RouterModule.forChild(routes);
