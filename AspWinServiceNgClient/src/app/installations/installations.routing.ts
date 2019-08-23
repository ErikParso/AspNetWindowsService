import { Routes, RouterModule } from '@angular/router';
import { InstallationListComponent } from './installation-list/installation-list.component';
import { InstallationToolsComponent } from './installation-tools/installation-tools.component';
import { NewClientWizardComponent } from './new-client-wizard/new-client-wizard.component';

const routes: Routes = [
  {
    path: '',
    component: InstallationListComponent,
    pathMatch: 'full'
  },
  {
    path: 'list',
    component: InstallationListComponent,
    pathMatch: 'full'
  },
  {
    path: 'newclient',
    component: NewClientWizardComponent,
    pathMatch: 'full'
  },
  {
    path: '',
    component: InstallationToolsComponent,
    pathMatch: 'full',
    outlet: 'footer'
  },
  {
    path: 'tools',
    component: InstallationToolsComponent,
    pathMatch: 'full',
    outlet: 'footer'
  },
  {
    path: 'none',
    pathMatch: 'full',
    outlet: 'footer'
  }
];

export const InstallationsRoutes = RouterModule.forChild(routes);
