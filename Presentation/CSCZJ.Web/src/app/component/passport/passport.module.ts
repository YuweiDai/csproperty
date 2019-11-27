import { NgModule } from '@angular/core';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router'
import { HttpClientModule } from '@angular/common/http';

import { NgZorroAntdModule } from 'ng-zorro-antd';

import { LoginComponent } from "./login/login.component";

import { PassportRoutingModule } from './passport-rouding.module';


@NgModule({
  imports: [
    CommonModule,
    FormsModule,
    RouterModule,
    ReactiveFormsModule,
    HttpClientModule,
    NgZorroAntdModule,
    PassportRoutingModule    
  ],
  declarations: [
    LoginComponent
  ],
  providers: [    
  ]
})
export class PassportModule { }