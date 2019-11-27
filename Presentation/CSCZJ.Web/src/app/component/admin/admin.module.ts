import { NgModule } from '@angular/core';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { CommonModule} from '@angular/common';
import { RouterModule } from '@angular/router'
import { HttpClientModule } from '@angular/common/http';

import { NgZorroAntdModule } from 'ng-zorro-antd';
import { NgxEchartsModule } from 'ngx-echarts';

import { AdminCenterComponent } from "./admin-center/admin-center.component";
import { HeaderComponent } from './layout/header/header.component';
import { FooterComponent } from './layout/footer/footer.component';
import { OverviewComponent } from './statistics/overview/overview.component';
import { MapHomeComponent } from './map/map-home/map-home.component';
import { AccountListComponent } from './systemmanager/account-list/account-list.component';
import { PropertyCenterComponent } from './properties/property-center/property-center.component';
import { PropertyListComponent } from './properties/property-list/property-list.component';
import { PropertyCreateComponent } from './properties/property-create/property-create.component';
import { PropertyDetailComponent } from './properties/property-detail/property-detail.component';
import { PropertyRentComponent } from './properties/property-rent/property-rent.component';
import { PropertyOffComponent } from './properties/property-off/property-off.component';

import { LeftmenuComponent } from "./common/leftmenu/leftmenu.component";
import { UiTableComponent } from './common/ui-table/ui-table.component';

import { AdminRoutingModule } from './admin-routing.module';

import { PerfectScrollbarModule, PERFECT_SCROLLBAR_CONFIG, PerfectScrollbarConfigInterface } from 'ngx-perfect-scrollbar';

import { PropertyService } from "../../services/propertyService";
import { GovernmentService } from "../../services/governmentService";
import { MapService } from "../../services/map/mapService";
import { PropertyExportComponent } from './properties/property-export/property-export.component';
import { PropertyRentlistComponent } from './properties/property-rentlist/property-rentlist.component';
import { PropertyPatrollistComponent } from './properties/property-patrollist/property-patrollist.component';

const DEFAULT_PERFECT_SCROLLBAR_CONFIG: PerfectScrollbarConfigInterface = {
  suppressScrollX: true
};

@NgModule({
  imports: [
    CommonModule,
    FormsModule,
    RouterModule,
    ReactiveFormsModule,
    HttpClientModule,
    NgZorroAntdModule,
    NgxEchartsModule,
    PerfectScrollbarModule,
    AdminRoutingModule
  ],
  declarations: [
    HeaderComponent,
    FooterComponent,
    OverviewComponent,
    MapHomeComponent,
    AccountListComponent,
    PropertyCenterComponent,
    PropertyListComponent,
    PropertyCreateComponent,
    PropertyDetailComponent,
    PropertyExportComponent,
    PropertyRentlistComponent,
    UiTableComponent,
    LeftmenuComponent,
    PropertyRentComponent,
    PropertyOffComponent,
    AdminCenterComponent,
    PropertyExportComponent,
    PropertyRentlistComponent,
    PropertyPatrollistComponent
  ],
  providers: [
    {
      provide: PERFECT_SCROLLBAR_CONFIG,
      useValue: DEFAULT_PERFECT_SCROLLBAR_CONFIG

    },
    PropertyService, GovernmentService, MapService,
  ]
})
export class AdminModule { }