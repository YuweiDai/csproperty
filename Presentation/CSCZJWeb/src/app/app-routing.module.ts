import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';

import { PageNotFoundComponent } from './page-not-found/page-not-found.component';

import { LoginComponent } from "./auth/login/login.component";

const appRoutes: Routes = [
  { path: 'login', component: LoginComponent },
  { path: '',  redirectTo:"/login", pathMatch: 'full', },
  { path: '**', component: PageNotFoundComponent }
];

@NgModule({
  imports: [
    RouterModule.forRoot(
      appRoutes,
      {
        // enableTracing: true,// <-- debugging purposes only
        useHash: false
      }
    )
  ],
  exports: [
    RouterModule
  ]
})
export class AppRoutingModule { }