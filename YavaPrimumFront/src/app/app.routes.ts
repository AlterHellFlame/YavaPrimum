import { Routes } from '@angular/router';
import { AuthorizationComponent } from './components/authorization/authorization.component';
import { HeaderComponent } from './components/header/header.component';
import { NotFoundComponent } from './components/not-found/not-found.component';
import { HrComponent } from './components/HR-page/hr/hr.component';
import { UserDataComponent } from './components/user-data/user-data.component';
import { NotificationsComponent } from './components/notifications-page/notifications/notifications.component';
import { AdminComponent } from './components/admin-page/admin/admin.component';
import { ChartsPageComponent } from './components/admin-page/charts-page/charts-page.component';
import { CandidatePageComponent } from './components/candidate-page/candidate-page.component';

export const routes: Routes = [
    { path: '', redirectTo: 'log-in', pathMatch: 'full' },
    { path: 'log-in', component: AuthorizationComponent },
    {
      path: 'account', component: HeaderComponent,
      children: [
        { path: 'hr', component: HrComponent },
        { path: 'po', component: HrComponent },
        { path: 'userData', component: UserDataComponent },
        { path: 'notifications', component: NotificationsComponent },
        { path: 'admin', component: AdminComponent},
        { path: 'charts', component: ChartsPageComponent},
        { path: 'candidates', component: CandidatePageComponent}
      ]
    },
    { path: '**', component: NotFoundComponent },
  ];
  