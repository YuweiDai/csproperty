import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';

import { NgZorroAntdModule } from 'ng-zorro-antd';
import { NgxEchartsModule } from 'ngx-echarts';
import { PerfectScrollbarModule } from 'ngx-perfect-scrollbar';
import { PERFECT_SCROLLBAR_CONFIG } from 'ngx-perfect-scrollbar';
import { PerfectScrollbarConfigInterface } from 'ngx-perfect-scrollbar';

import { AdminRoutingModule } from './admin-routing.module';
import { AdminComponent } from './admin.component';
import { DashboardComponent } from './dashboard/dashboard.component';
import { MapComponent } from './map/map.component';

import { HeaderComponent } from './layout/header/header.component';
import { FooterComponent } from './layout/footer/footer.component';

import { PropertyCenterComponent } from './properties/property-center/property-center.component';
import { LeftmenuComponent } from '../common/leftmenu/leftmenu.component';
import { UiTableComponent } from '../common/ui-table/ui-table.component';
import { PropertyListComponent } from './properties/property-list/property-list.component';
import { PropertyCreateComponent } from './properties/property-create/property-create.component';
import { PropertyDetailComponent } from './properties/property-detail/property-detail.component';
import { PropertyRentComponent } from './properties/property-rent/property-rent.component';
import { PropertyRentlistComponent } from './properties/property-rentlist/property-rentlist.component';
import { PropertyOffComponent } from './properties/property-off/property-off.component';
import { PropertyExportComponent } from './properties/property-export/property-export.component';
import { PropertyPatrollistComponent } from './properties/property-patrollist/property-patrollist.component';


const DEFAULT_PERFECT_SCROLLBAR_CONFIG: PerfectScrollbarConfigInterface = {
  suppressScrollX: true
};


@NgModule({
  declarations: [AdminComponent,
    DashboardComponent, MapComponent, PropertyCenterComponent, PropertyListComponent, PropertyCreateComponent,
    HeaderComponent, FooterComponent,
    UiTableComponent,
    LeftmenuComponent,
    PropertyDetailComponent,
    PropertyRentComponent,
    PropertyRentlistComponent,
    PropertyOffComponent,
    PropertyExportComponent,
    PropertyPatrollistComponent
  ],
  imports: [
    CommonModule, FormsModule, ReactiveFormsModule,
    AdminRoutingModule,
    NgZorroAntdModule,
    NgxEchartsModule,
    PerfectScrollbarModule
  ],
  providers: [
    {
      provide: PERFECT_SCROLLBAR_CONFIG,
      useValue: DEFAULT_PERFECT_SCROLLBAR_CONFIG
    }
  ]
})
export class AdminModule { }
