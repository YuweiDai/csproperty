import { Component, OnInit,ViewEncapsulation } from '@angular/core';
import { MapService } from '../../../../services/map/mapService';
import { LayoutService } from "../../../../services/layoutService";
import { property_map } from "../../../../viewModels/Properties/property_map";
import { PropertyService } from '../../../../services/propertyService';
import {MapListResponse} from '../../../../viewModels/Response/MapListResponse';
import { Property } from "../../../../viewModels/Properties/property";
import {PropertyNameList} from "../../../../viewModels/Properties/propertyName";
import {HighSearchProperty} from '../../../../viewModels/Properties/highSearchModel';

import 'leaflet';
import 'leaflet-draw';

import 'wicket';
import * as HeatmapJS from 'heatmap.js';
import 'leaflet.markercluster';


declare var L:any;
declare var HeatmapOverlay: any;
declare var Control:any;
declare var IconLayers:any;
declare var Wkt: any;

@Component({
    selector: 'app-map-home',
    encapsulation: ViewEncapsulation.None ,
    templateUrl: './map-home.component.html',   
    styleUrls: ['./map-home.component.less']
    
})
export class MapHomeComponent implements OnInit {
    
    map: any;
    mapHeight = 500;
    properties:any;
    propertyID:number;
    private basicInfo:any[];
    private property:Property;
    private propertyNameList:any;
    search:string;
    inputValue: string;
    private options=[];
    option=[];
    allChecked = false;
    indeterminate = true;
    highSearchProperty = new HighSearchProperty;
    showCollapse=false;
    searchProperties:any[];
    perfectScrollbarConfig:{};
    containerHeight=100;
    visible: boolean;
    switchModel=true;
    switchModel1=false;
    house:any;
    land:any;
    markers:any;  
    panomarkers=new L.LayerGroup();
    private wkt: any;
    extent:any;
    choosePropert:any;
    mapOverlayOption = {
        icon:
   new L.Icon.Default(),
        editable: false,
        color: '#AA0000',
        weight: 3,
        opacity: 1.0,
        fillColor: '#AA0000',
        fillOpacity: 0.2
    };

    //热力图格式
    heatLayer = new L.LayerGroup();
      heatMapData={
          max:5700,
          data:[]
      }
      cfg = {
        // container: window.document.getElementById('container'),
        "radius": 30,
        "maxOpacity": .8,
        "scaleRadius": false,
        "useLocalExtrema": true,
        latField: 'lat',
        lngField: 'lon',
        valueField: 'count'
       };
      heatmapLayer =new HeatmapOverlay(this.cfg);
      
    
    panels = [
        {
          active    : true,
          name      : 'This is panel header 1',
          disabled  : false
        }
      ];
   


    propertyGoverment=[
        { label: '常山县财政局', value: '1', checked: true },
        { label: '常山县公路管理局', value: '2', checked: true },
        { label: '常山县教育局', value: '3', checked: true }
    ];
    propertyType = [
        { label: '房产', value: 'House', checked: false },
        { label: '土地', value: 'Land', checked: false }
       
      ];
      regionType =[
        { label: '天马镇', value: 'TMZ', checked: false },
        { label: '招贤镇', value: 'ZSZ', checked: false },
        { label: '辉埠镇', value: 'HBZ', checked: false },
        { label: '球川镇', value: 'LQZ', checked: false },
        { label: '宋畈乡', value: 'SBZ', checked: false }

      ];
      area=[
        { label: '50以下', value: 'One', checked: false },
        { label: '50-200', value: 'Two', checked: false },
        { label: '200-500', value: 'Three', checked: false },
        { label: '500-1000', value: 'Four', checked: false },
        { label: '1000以上', value: 'Five', checked: false }
      ];
      currentType=[
        { label: '自用', value: 'ZY', checked: false },
        { label: '出租', value: 'CC', checked: false },
        { label: '闲置', value: 'XZ', checked: false },
        { label: '调配使用', value: 'SYDP', checked: false }
      ];
      propertyRights=[
        { label: '两证齐全', value: 'All', checked: false },
        { label: '有房产证', value: 'isHouse', checked: false },
        { label: '有土地证', value: 'isLand', checked: false },
        { label: '两证全无', value: 'None', checked: false }
      ];


    //输入框模糊搜索
    onInput(value: string): void {
    if(this.inputValue!=""&&this.inputValue!=null){
        this.propertyService.getPropertiesBySearch(value).subscribe(response=>{  
            this.propertyNameList = response;        
            response.forEach(p=>{             
                this.options.push( {id:p.id,value:p.name +" "+p.address}); 
            })               
        });
      this.options.forEach(o=>{

        console.log(o.value);
      })
    }    
    
    else {
        this.options =[];
    }

      };

      
      

    constructor(private mapService: MapService, private layoutService: LayoutService,private propertyService:PropertyService) {
        this.containerHeight=layoutService.getActualScreenSize().height;
        this.containerHeight=layoutService.getContentHeight()-200; 
    }


    ngOnInit() {
    
        this.mapHeight = this.layoutService.getContentHeight();  //计算除了header footer的高度
        var that=this;
        setTimeout(() => {
            var normal = this.mapService.getLayer("vector");
            var satellite = this.mapService.getLayer("img");
            that.map = L.map('map', {
                crs: L.CRS.EPSG4326,
                center: [28.905527517199516, 118.50629210472107],
                zoom: 14
            });

            normal.addTo(that.map);

           
            var iconLayersControl = new L.Control.IconLayers(
                [
                    {
                        title: '矢量', // use any string
                        layer: normal, // any ILayer
                        icon: '../../assets/images/iconLayers/sl.png' // 80x80 icon
                    },
                    {
                        title: '影像',
                        layer: satellite,
                        icon: '../../assets/images/iconLayers/yx.png'
                    }
                ], {
                    position: 'bottomright'
                   // maxLayersInRow: 5
                }
            );

            var zoomControl = that.map.zoomControl;

            zoomControl.setPosition("bottomleft");

             iconLayersControl.addTo(that.map);

            // mapService.setMapAttribute(map);

            that.house = L.icon({
                iconUrl: '../../assets/js/MarkerClusterGroup/house.png',
                iconAnchor: [16, 32],
            });

           that.land = L.icon({
                iconUrl: '../../assets/js/MarkerClusterGroup/land.png',
                iconAnchor: [16, 32],
            });

            that.markers = new L.MarkerClusterGroup({
                spiderfyOnMaxZoom: false,
                showCoverageOnHover: false,
                zoomToBoundsOnClick: false                
            });  

          

            that.getMapProperties(that.markers);
            that.map.addLayer(that.markers);     
            that.wkt = new Wkt.Wkt();
     
            //that.map.addLayer(this.heatmapLayer);

            //点击获取单个资产信息
           that.markers.on('click', function (a) {
               that.showCollapse = false;
              
                that.properties.forEach(element => {
                  if(a.latlng.lat==element.x&&a.latlng.lng==element.y){
                     
                      that.propertyService.getPropertyById(element.id,false).subscribe(property=>{
                        that.property=property;
                        if(that.extent!=undefined) that.map.removeLayer(that.extent);
                       
                        that.basicInfo=[
                          {title:"资产名称",value:that.property.name},
                          {title:"类别",value:that.property.propertyType},
                          {title:"坐落位置",value:that.property.address},
                          {title:"四至情况",value:that.property.fourToStation},
                          {title:"权属单位",value:that.property.governmentName},
                          {title:"获取方式",value:that.property.getMode},
                          {title:"取得时间",value:that.property.getedDate},
                          {title:"层数",value:that.property.floor},
                          {title:"产权证号",value:that.property.propertyId},
                          {title:"建筑面积",value:that.property.constructorArea},
                          {title:"房产证",value:that.property.constructId},
                          {title:"房产证发证时间",value:that.property.constructTime},
                          {title:"土地面积",value:that.property.landArea},
                          {title:"土地证",value:that.property.landId},
                          {title:"土地证发证时间",value:that.property.landTime},
                          {title:"使用人员",value:that.property.usedPeople},
                          {title:"使用现状",value:that.property.currentType},
                          {title:"用途",value:that.property.useType},
                          {title:"入账",value:that.property.isAdmission},
                          {title:"抵押",value:that.property.isMortgage},
                        ];
                       that.wkt.read(property.extent);
                       that.extent = that.wkt.toObject(that.mapOverlayOption);
                       that.extent.addTo(that.map);

                         });
                  }
              });

          });           

        }, 500);


             
    }


//选择搜索的单个资产
findThisOne(option):void{

     this.markers.clearLayers();
     //this.map.removeLayer(this.extent);
     this.propertyService.getPropertyById(option.id,false).subscribe(property=>{
     var response = property;
   
         var points = response.location.split(' ');
         if(response.propertyType=="房屋"){
            
            var m = new L.marker(new L.LatLng (points[2].substring(0,points[2].length-1),points[1].substring(1,points[1].length-1)),{
                 icon:this.house
             }).bindTooltip(response.name,{permanent:true,direction:"top",offset:[0,-15]});                      
            this.markers.addLayer(m);
           }
           else{
            var m = new L.marker(new L.LatLng (points[2].substring(0,points[2].length-1),points[1].substring(1,points[1].length)),{
                icon:this.land
            }).bindTooltip(response.name,{permanent:true,direction:"top",offset:[0,-15]});   
          this.map.setView([points[2].substring(0,points[2].length-1),points[1].substring(1,points[1].length-1)],16);
          this.markers.addLayer(m);
     

         }

     })

};


    panTo(): any {

        this.map.panTo({ lon: 118.8656, lat: 28.9718 });

    }
    showLngLat(): any {

        console.log(this.map.getCenter());

    }
    //获取地图大数据
    getMapProperties(markers): void {
    
        var ps = [];
      
        this.propertyService.getAllPropertiesInMap()
            .subscribe(response => {
                //console.log(response[2]);
                if (response!=null) {
                    this.properties = response;
                    ps=response;
                    this.properties.forEach(element => {
                   if(element.propertyType=="房屋"){
                    var m = new L.marker(new L.LatLng (element.x,element.y),{
                         icon:this.house
                     },{propertyid:element.id}).bindTooltip(element.name,{permanent:false,direction:"top",offset:[0,-32]});                      
                     markers.addLayer(m);
                   }

                   else{
                    var m = new L.marker(new L.LatLng (element.x,element.y),{
                        icon:this.land
                    }).bindTooltip(element.name,{permanent:false,direction:"top",offset:[0,-32]});                           
                    markers.addLayer(m);

                   }
                   

                   var heatPoint = {lat:element.x,lon:element.y,count:parseInt(element.constructArea)+1};
                   this.heatMapData.data.push(heatPoint);                        
                    });    

               
                }
            });      
                       
    };


    //关闭资产详细列表
    closeDetail():void{

        this.basicInfo = null;
        if(this.extent!=null||this.extent!=undefined)  this.map.removeLayer(this.extent);
    }

    //高级搜索提交
    Submit(): void {
        this.basicInfo = null;
        if(this.extent!=null||this.extent!=undefined)  this.map.removeLayer(this.extent);

        this.highSearchProperty.House=false;
        this.highSearchProperty.Land=false;
        this.highSearchProperty.TMZ=false;
        this.highSearchProperty.ZSZ=false;
        this.highSearchProperty.HBZ=false;
        this.highSearchProperty.SBZ=false;
        this.highSearchProperty.ZY=false;
        this.highSearchProperty.CC=false;
        this.highSearchProperty.XZ=false;
        this.highSearchProperty.SYDP=false;
        this.highSearchProperty.All=false;
        this.highSearchProperty.isHouse=false;
        this.highSearchProperty.isLand=false;
        this.highSearchProperty.None=false;
        this.highSearchProperty.One=false;
        this.highSearchProperty.Two=false;
        this.highSearchProperty.Three=false;
        this.highSearchProperty.Four=false;
        this.highSearchProperty.Five=false;

        this.propertyType.forEach(p=>{     
            if(p.checked==true){                         
               
                
              for(var h in this.highSearchProperty){
                  if(h==p.value) this.highSearchProperty[h]=true;
              }

            }
        });

        this.regionType.forEach(p=>{     
            if(p.checked==true){                         
               
                
              for(var h in this.highSearchProperty){
                  if(h==p.value) this.highSearchProperty[h]=true;
              }

            }
        });

        this.area.forEach(p=>{     
            if(p.checked==true){                         
               
                
              for(var h in this.highSearchProperty){
                  if(h==p.value) this.highSearchProperty[h]=true;
              }

            }
        });
        this.currentType.forEach(p=>{     
            if(p.checked==true){                         
               
                
              for(var h in this.highSearchProperty){
                  if(h==p.value) this.highSearchProperty[h]=true;
              }

            }
        });
        this.propertyRights.forEach(p=>{     
            if(p.checked==true){                         
               
                
              for(var h in this.highSearchProperty){
                  if(h==p.value) this.highSearchProperty[h]=true;
              }

            }
        });     

        this.propertyService.getHighSearchProperties(this.highSearchProperty).subscribe(response=>{

            this.showCollapse = true;

            this.searchProperties = response;
            this.markers.clearLayers();

           this.searchProperties.forEach(element => {
           if(element.propertyType=="房屋"){
            var m = new L.marker(new L.LatLng (element.x,element.y),{
                 icon:this.house
             },{propertyid:element.id}).bindTooltip(element.name,{permanent:true,direction:"top",offset:[0,-15]});                      
            this.markers.addLayer(m);
           }

           else{
            var m = new L.marker(new L.LatLng (element.x,element.y),{
                icon:this.land
            }).bindTooltip(element.name,{permanent:true,direction:"top",offset:[0,-15]});                           
           this.markers.addLayer(m);

           }
                
            });    

            this.panels[0].name="共查询到"+this.searchProperties.length +"条记录！";
        })

        this.visible = false;
      
      }
    
    
      Close() {
        this.visible = false;
      }

      Switch(){
        this.switchModel=!this.switchModel;
        if(this.extent!=null||this.extent!=undefined)  this.map.removeLayer(this.extent);

        if(this.switchModel==false){

            this.heatLayer.addLayer(this.heatmapLayer);
            this.map.addLayer(this.heatLayer);
           // this.markers.clearLayers();
            this.map.removeLayer(this.markers);
            this.heatmapLayer.setData(this.heatMapData);
        }
        else{
           this.heatLayer.clearLayers();
           this.map.addLayer(this.markers);
        }

      }

      SwitchPanos(){
        this.switchModel1=!this.switchModel1;
        var plan=L.icon({
            iconUrl:'../../../assets/无人机.png',
            iconAnchor:[8,8]
        })

        if(this.switchModel1==true){
           // 
           console.log(123);
           var m1= L.marker([28.904104940593243,118.50616335868835],{icon:plan}).on('click',function(){
               window.open("http://220.191.237.169/cspanos?id="+"xzf");
           }).addTo(this.panomarkers); 
           var m2= L.marker([28.91212558373809,118.51464986801147],{icon:plan}).on('click',function(){
            window.open("http://220.191.237.169/cspanos?id="+"tyg");
           }).addTo(this.panomarkers); 
          this.panomarkers.addTo(this.map);
        }
        else{
            this.map.removeLayer(this.panomarkers);
        }
        
      }

      chooseProperty(choosePropert){
          this.searchProperties.forEach(e=>{
              if(choosePropert.name==e.name&&choosePropert.address==e.address){
                this.map.setView([e.x,e.y],16);
              }
          })

      }

      Reset(){
        this.propertyType.forEach(e=>{
            e.checked=false;
        });
        this.regionType.forEach(e=>{
            e.checked=false;
        });
        this.currentType.forEach(e=>{
            e.checked=false;
        });
        this.propertyRights.forEach(e=>{
            e.checked=false;
        });
        this.area.forEach(e=>{
            e.checked=false;
        });
        this.highSearchProperty.Land=false;
        this.highSearchProperty.TMZ=false;
        this.highSearchProperty.ZSZ=false;
        this.highSearchProperty.HBZ=false;
        this.highSearchProperty.SBZ=false;
        this.highSearchProperty.ZY=false;
        this.highSearchProperty.CC=false;
        this.highSearchProperty.XZ=false;
        this.highSearchProperty.SYDP=false;
        this.highSearchProperty.All=false;
        this.highSearchProperty.isHouse=false;
        this.highSearchProperty.isLand=false;
        this.highSearchProperty.None=false;
        this.highSearchProperty.One=false;
        this.highSearchProperty.Two=false;
        this.highSearchProperty.Three=false;
        this.highSearchProperty.Four=false;
        this.highSearchProperty.Five=false;

        this.searchProperties=[];
        this.showCollapse=false;

        this.map.addLayer(this.markers);

      }


}
