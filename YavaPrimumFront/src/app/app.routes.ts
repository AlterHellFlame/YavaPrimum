import { Routes } from '@angular/router';
import { HrMainPageComponent } from './pages/HR/hr-main-page/hr-main-page.component';
import { PoMainPageComponent } from './pages/PO/po-main-page/po-main-page.component';
import { HeaderComponent } from './pages/header/header.component';
import { AuthGuard } from './services/autorization/auth-guard';
import { AutotizationComponent } from './pages/autotization/autotization.component';

export const routes: Routes = [
    { path: '', component: AutotizationComponent },
    {
      path: 'account', component: HeaderComponent,
      children: [
        { path: 'HR', component: HrMainPageComponent },
        { path: 'PO', component: PoMainPageComponent }
      ]
    }
  ];
  