import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';

import { LoginComponent } from "./login/login.component";
const passportRoutes: Routes = [
    {
        path: 'login',
        component: LoginComponent,
    }
];

@NgModule({
    imports: [
        RouterModule.forChild(passportRoutes)
    ],
    exports: [
        RouterModule
    ]
})
export class PassportRoutingModule { }