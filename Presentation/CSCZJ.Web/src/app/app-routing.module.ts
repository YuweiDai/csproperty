import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';



import { LoginComponent } from './component/passport/login/login.component';


export const appRoutes: Routes = [
  { path: 'login', component: LoginComponent },
  { path: '**', redirectTo: '/login', pathMatch: 'full' },
]

@NgModule({
  imports: [
    RouterModule.forRoot(appRoutes, { useHash: false }),
  ],
  exports: [RouterModule],
})
export class AppRoutingModule { }
