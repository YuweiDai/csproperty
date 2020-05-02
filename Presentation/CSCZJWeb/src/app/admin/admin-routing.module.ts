import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';

import { AdminComponent } from "./admin.component";
import { DashboardComponent } from "./dashboard/dashboard.component";

import { MapComponent } from "./map/map.component";
import { PropertyCenterComponent } from './properties/property-center/property-center.component';
import { PropertyListComponent } from './properties/property-list/property-list.component';
import { PropertyCreateComponent } from './properties/property-create/property-create.component';
import { PropertyDetailComponent } from './properties/property-detail/property-detail.component';
import { PropertyRentComponent } from './properties/property-rent/property-rent.component';
import { PropertyOffComponent } from './properties/property-off/property-off.component';
import { PropertyRentlistComponent } from './properties/property-rentlist/property-rentlist.component';
import { PropertyExportComponent } from './properties/property-export/property-export.component';
import { PropertyPatrollistComponent } from './properties/property-patrollist/property-patrollist.component';

const adminRoutes: Routes = [
  {
    path: 'admin',
    // canActivate: [AuthGuard],
    component: AdminComponent,
    children: [
      { path: 'dashboard', component: DashboardComponent },
      {
        path: 'properties',
        component: PropertyCenterComponent,
        children: [
          {
            path: '',
            component: PropertyListComponent,
          },
          {
            path: 'create',
            component: PropertyCreateComponent
          },
          {
            path: 'edit/:id',
            component: PropertyCreateComponent
          },
          {
            path: 'rentedit/:id',
            component: PropertyRentComponent
          },
          {
            path: 'off',
            component: PropertyOffComponent
          },
          {
            path: 'export',
            component: PropertyExportComponent
          },
          {
            path: 'rentlist',
            component: PropertyRentlistComponent
          },
          {
            path: 'patrollist',
            component: PropertyPatrollistComponent
          },
          {
            path: ':id',
            component: PropertyDetailComponent
          }
        ]
      },
      { path: 'map', component: MapComponent },
      // { path: 'manager', component: AccountListComponent },
    ]
  }
];

@NgModule({
  imports: [RouterModule.forChild(adminRoutes)],
  exports: [RouterModule]
})
export class AdminRoutingModule { }
