import { Routes, RouterModule } from '@angular/router';
import { InstallationListComponent } from './installation-list/installation-list.component';
import { InstallationToolsComponent } from './installation-tools/installation-tools.component';

const routes: Routes = [
  {
    path: '',
    component: InstallationListComponent,
    outlet: 'content',
  },
  {
    path: '',
    component: InstallationToolsComponent,
    outlet: 'footer',
  }
];

export const InstallationsRoutes = RouterModule.forChild(routes);
