import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';

import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
// import { NzLayoutModule } from 'ng-zorro-antd/layout';
// import { NzMenuModule } from 'ng-zorro-antd/menu';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { HttpClientModule } from '@angular/common/http';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { NZ_I18N } from 'ng-zorro-antd/i18n';
import { zh_CN } from 'ng-zorro-antd/i18n';
import { CommonModule, registerLocaleData } from '@angular/common';
import zh from '@angular/common/locales/zh';
import { AdminModule } from './components/admin/admin.module';
import { PassportModule } from './components/passport/passport.module';
import { NgxWebstorageModule } from 'ngx-webstorage';


import { AuthGuard } from "./services/auth-guard.service";
import { LogService } from "./services/logService";
import { ConfigService } from "./services/configService";
import { LayoutService } from "./services/layoutService";
import { AuthInterceptorService, AuthService, TokensManagerService } from "./services/passportService";


// import { NgxEchartsModule } from 'ngx-echarts';
import * as echarts from 'echarts';


@NgModule({
  declarations: [
    AppComponent
  ],
  imports: [
    BrowserModule,
    AppRoutingModule,
    FormsModule,
    HttpClientModule,
    BrowserAnimationsModule,
    PassportModule,
    AdminModule,  NgxWebstorageModule.forRoot(),
  ],
  providers: [LogService,AuthGuard,ConfigService,LayoutService,AuthInterceptorService,AuthService,TokensManagerService,
    { provide: NZ_I18N, useValue: zh_CN }],
  bootstrap: [AppComponent]
})
export class AppModule { }
