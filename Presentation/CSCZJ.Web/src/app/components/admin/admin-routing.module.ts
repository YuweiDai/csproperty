import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';

import { AdminCenterComponent } from "./admin-center/admin-center.component";
import { OverviewComponent } from './statistics/overview/overview.component';
import { MapHomeComponent } from './map/map-home/map-home.component';
import { AccountListComponent } from './systemmanager/account-list/account-list.component';
import { PropertyCenterComponent } from './properties/property-center/property-center.component';
import { PropertyListComponent } from './properties/property-list/property-list.component';
import { PropertyDetailComponent } from './properties/property-detail/property-detail.component';
import { PropertyCreateComponent } from './properties/property-create/property-create.component';
import { PropertyRentComponent } from './properties/property-rent/property-rent.component';
import { PropertyOffComponent } from './properties/property-off/property-off.component';
import { PropertyExportComponent } from './properties/property-export/property-export.component';
import { PropertyRentlistComponent } from './properties/property-rentlist/property-rentlist.component';
import { PropertyPatrollistComponent } from './properties/property-patrollist/property-patrollist.component';
import { PropertyFilelistComponent } from './properties/property-filelist/property-filelist.component';
import { AuthGuard } from "../../services/auth-guard.service";

const adminRoutes: Routes = [
  {
      path: 'admin',
      //canActivate: [AuthGuard],
      component: AdminCenterComponent,
      children: [
          { path: 'dashboard', component: OverviewComponent },
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
                    path: 'filelist',
                    component: PropertyFilelistComponent
                  },     
                  {
                      path: 'rent',
                      component: PropertyRentComponent
                  },
                  {
                      path: 'off',
                      component: PropertyOffComponent
                  },
                  {
                      path: ':id',
                      component: PropertyDetailComponent
                  },
                  {
                      path: 'edit/:id',
                      component: PropertyCreateComponent
                  },
                  {
                      path: 'rentedit/:id',
                      component: PropertyRentComponent
                  }
                
              ]
          },
          { path: 'map', component: MapHomeComponent },
          { path: 'manager', component: AccountListComponent },
      ]
  }
];

@NgModule({
  imports: [RouterModule.forChild(adminRoutes)],
  exports: [RouterModule]
})
export class AdminRoutingModule { }
