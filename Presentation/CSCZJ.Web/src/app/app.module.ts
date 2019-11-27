import { NgModule } from '@angular/core'
import { FormsModule } from '@angular/forms';
import { BrowserModule } from '@angular/platform-browser'
import { RouterModule } from '@angular/router'
import {  Http, HttpModule, XHRBackend, RequestOptions } from '@angular/http'
import { AppRoutingModule } from './/app-routing.module'
import { BrowserAnimationsModule } from '@angular/platform-browser/animations'

import {HashLocationStrategy , LocationStrategy} from '@angular/common';

import { Ng2Webstorage } from 'ngx-webstorage';

import { NgZorroAntdModule } from 'ng-zorro-antd';
import { AdminModule } from "./component/admin/admin.module";
import { PassportModule } from "./component/passport/passport.module";


import { AppComponent } from './app.component';

import { AuthGuard } from "./services/auth-guard.service";
import { LogService } from "./services/logService";
import { ConfigService } from "./services/configService";
import { LayoutService } from "./services/layoutService";
import { AuthInterceptorService, AuthService, TokensManagerService } from "./services/passportService";

import { HttpInterceptorService }   from './extensions/HttpInterceptor';
 
 export function interceptorFactory(xhrBackend: XHRBackend, requestOptions: RequestOptions){
   let service = new HttpInterceptorService(xhrBackend, requestOptions);
   return service;
 }

@NgModule({
  declarations: [
    AppComponent,
  ],
  imports: [
    FormsModule,
    BrowserModule,
    BrowserAnimationsModule, Ng2Webstorage,
    RouterModule,
    NgZorroAntdModule.forRoot(),
    AdminModule,
    PassportModule,
    AppRoutingModule,
  ],
  providers: [
    LogService,
    ConfigService,
    LayoutService,
    AuthGuard,
    AuthInterceptorService, 
    AuthService, TokensManagerService,
    HttpInterceptorService,
    {
      provide: LocationStrategy,
      useClass: HashLocationStrategy

      //deps: [XHRBackend, RequestOptions]
    }  
  ],
  bootstrap: [AppComponent]
})
export class AppModule { }
