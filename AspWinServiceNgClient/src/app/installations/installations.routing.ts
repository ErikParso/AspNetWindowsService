import { Routes, RouterModule } from '@angular/router';
import { InstallationListComponent } from './installation-list/installation-list.component';
import { InstallationToolsComponent } from './installation-tools/installation-tools.component';
import { NewClientWizardComponent } from './new-client-wizard/new-client-wizard.component';

const routes: Routes = [
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
];

export const InstallationsRoutes = RouterModule.forChild(routes);
