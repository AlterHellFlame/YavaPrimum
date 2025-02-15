import { Routes } from '@angular/router';
import { AuthorizationComponent } from './components/authorization/authorization.component';
import { HeaderComponent } from './components/header/header.component';
import { NotFoundComponent } from './components/not-found/not-found.component';
import { HrComponent } from './components/HR-page/hr/hr.component';

export const routes: Routes = [
    { path: '', redirectTo: 'log-in', pathMatch: 'full' },
    { path: 'log-in', component: AuthorizationComponent },
    {
      path: 'account', component: HeaderComponent,
      children: [
        { path: 'HR', component: HrComponent }
        /*{ path: 'PO', component: PoMainPageComponent },
        { path: 'userData', component: UserDataComponent },*/
      ]
    },
    { path: '**', component: NotFoundComponent },
  ];
  