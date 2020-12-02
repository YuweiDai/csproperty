import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { AdminCenterComponent } from './components/admin/admin-center/admin-center.component';

import { LoginComponent } from './components/passport/login/login.component';

const routes: Routes = [
  { path: 'admin', component: AdminCenterComponent },

  //{ path: 'login', component: LoginComponent },
 //{ path: '**', redirectTo: 'admin/dashboard', pathMatch: 'full' },
 //{ path: '**', redirectTo: 'admin/map', pathMatch: 'full' },
 { path: 'login', component: LoginComponent },
 { path: '',  redirectTo:"/login", pathMatch: 'full', },
 //{ path: '**', component: PageNotFoundComponent }
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
