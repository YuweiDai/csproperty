import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router'
import { HttpClientModule } from '@angular/common/http';

import { PassportRoutingModule } from './passport-routing.module';
import { LoginComponent } from './login/login.component';

import { NzModalModule } from 'ng-zorro-antd/modal';
import { NzNotificationModule } from 'ng-zorro-antd/notification';
import { NzMessageModule } from 'ng-zorro-antd/message';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { NzFormModule } from 'ng-zorro-antd/form';
import { NzInputModule } from 'ng-zorro-antd/input';
import { NzUploadModule } from 'ng-zorro-antd/upload';
import { NzButtonModule } from 'ng-zorro-antd/button';
import { NzIconModule } from 'ng-zorro-antd/icon';


import { LocalStorageService, SessionStorageService } from 'ngx-webstorage';






@NgModule({
  declarations: [LoginComponent],
  imports: [
    CommonModule,
    PassportRoutingModule,
    RouterModule,HttpClientModule,
    NzModalModule,NzNotificationModule,NzMessageModule,NzUploadModule,FormsModule,ReactiveFormsModule,NzFormModule,NzIconModule,
    NzInputModule,NzButtonModule,
  ],
  providers: [    
    LocalStorageService
  ]
})
export class PassportModule { }
