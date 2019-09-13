import { Routes, RouterModule } from '@angular/router';
import { InstallationListComponent } from './installation-list/installation-list.component';
import { NewClientWizardComponent } from './new-client-wizard/new-client-wizard.component';
import { InstallationsComponent } from './installations/installations.component';

const routes: Routes = [
  {
    path: '',
    component: InstallationsComponent,
    children: [
      {
        path: 'list',
        component: InstallationListComponent,
        pathMatch: 'full'
      },
      {
        path: '',
        redirectTo: 'list',
        pathMatch: 'full'
      },
      {
        path: 'newclient',
        component: NewClientWizardComponent,
        pathMatch: 'full'
      }
    ]
  }
];

export const InstallationsRoutes = RouterModule.forChild(routes);
