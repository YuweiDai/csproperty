import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { Location } from '@angular/common';

import { Property, PropertyRentModel,SameIdPropertyModel } from "../../../../viewModels/Properties/property";

import { MapService } from '../../../../services/map/mapService';
import { PropertyService } from '../../../../services/propertyService';
declare var L: any;
declare var Wkt: any;
@Component({
  selector: 'app-property-detail',
  templateUrl: './property-detail.component.html',
  styleUrls: ['./property-detail.component.less']
})
export class PropertyDetailComponent implements OnInit {

  public loading: boolean;
  public property: Property;
  public wkt: any;
  public map: any;
  public subProperties:SameIdPropertyModel[];

  constructor(
    public propertyService: PropertyService, public mapService: MapService,
    public route: ActivatedRoute, public router: Router) {
    this.property = new Property();

    this.subProperties=[];
  }

  ngOnInit() {
    this.getProperty();
  }

  getProperty(): void {
    this.loading = true;
    const id = +this.route.snapshot.paramMap.get('id');

    this.propertyService.getPropertyById(id,false).subscribe(property => {
      if (property == undefined) this.router.navigate(['../properties']);
      else {
        this.property = property;

        this.property.rents.forEach(rent=>{
          rent.priceList=rent.priceString.split(';');
        });
        
        if(this.property.parentPropertyId==0)
        {
        var typeId = this.property.estateId  ? "0" : "1";
        var number = this.property.estateId ? this.property.estateId : 
        (this.property.constructId?this.property.constructId:this.property.landId);

        this.propertyService.getPropertiesBySameNumberId(number, typeId,this.property.id)
        .subscribe(response => {
          var that = this; 
          console.log(123);
          that.subProperties=response;

          that.subProperties.forEach(element => {
            if(element.isMain)
            {
              that.property.parentPropertyId=element.id;
              return false;
            }
          });
        });        
      }

        console.log(this.property);
        this.loading = false;
      }
    });


  }

  mapStepInitial(): void {
    var that = this;
    that.wkt = new Wkt.Wkt();
    setTimeout(() => {
      var normal = this.mapService.getLayer("vector");
      var satellite = this.mapService.getLayer("img");
      this.map = L.map('map', {
        crs: L.CRS.EPSG4326,
        center: [28.905527517199516, 118.50629210472107],
        zoom: 17
      });

      satellite.addTo(this.map);
      var baseLayers = {
        "矢量": normal,
        "卫星": satellite
      };
      //L.control.layers(baseLayers).addTo(that.map);
      var zoomControl = this.map.zoomControl;

      zoomControl.setPosition("topright");

      if (that.property != null && that.property != undefined) {

        that.wkt.read(that.property.location);
        L.marker([that.wkt.components[0].y, that.wkt.components[0].x], { title: that.property.name })
          .bindTooltip(that.property.name, { permanent: true, direction: "top", offset: [0, -15] })
          .addTo(this.map);


        that.map.panTo([that.wkt.components[0].y, that.wkt.components[0].x]);
      }
    }, 800);
  }

}
