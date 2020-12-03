import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormControl, FormGroup, ValidationErrors, Validators, ValidatorFn } from '@angular/forms';
import { Observable } from 'rxjs';
import { Observer } from 'rxjs';
import { Router, ActivatedRoute, ParamMap } from '@angular/router';

import { NzModalService } from 'ng-zorro-antd/modal';
import { NzMessageService } from 'ng-zorro-antd/message';
import { NzNotificationService } from 'ng-zorro-antd/notification';
import { NzUploadFile } from 'ng-zorro-antd/upload';

import { format, compareAsc } from 'date-fns'

import { PropertyCreateModel, PropertyPictureModel, PropertyFileModel,SameIdPropertyModel } from '../../../../viewModels/Properties/property';

import { MapService } from 'src/app/services/map/mapService';
import { PropertyService } from 'src/app/services/propertyService';
import { GovernmentService } from 'src/app/services/governmentService';
import { ConfigService } from 'src/app/services/configService';

declare var L: any;
declare var Wkt: any;

@Component({
  selector: 'app-property-create',
  templateUrl: './property-create.component.html',
  styleUrls: ['./property-create.component.less'],
  providers: [],
})

export class PropertyCreateComponent implements OnInit {
  public id: number;
  public title: string;
  public current: number;  
  public stepStatus: string;
  public property = new PropertyCreateModel();
  public orginalPropertyName: string;
  public isSameCardIdLoading:boolean;
  public sameCardIdChecked:boolean;
  public sameCardProperties:SameIdPropertyModel[];
 
  public wkt: any;
  public map: any;
  public marker = null;
  public extent = null;
  public editableLayers = new L.FeatureGroup();
  public mapOverlayOption = {
    edit: true,
    //icon: new L.Icon.Default(),
    //color: '#AA0000',
    //weight: 3,
    //opacity: 1.0,
    //fillColor: '#AA0000',
    //fillOpacity: 0.2
  };

  public basicFormValidateConfig = {
    constructAreaRequired: false,
    estateIdRequired: true,
    estateTimeRequired: true,
    constructIdRequired: false,
    constructTimeRequired: false,
    landIdRequired: false,
    landTimeRequired: false,
  };
  public isGovernmentLoading = false;
  public optionList = [];

  basicInfoForm: FormGroup;
  geoInfoForm: FormGroup;
  fileInfoForm: FormGroup;

  public pictureUploadUrl: string;
  public picureUploading = false;
  public fileUploadUrl: string;
  public fileUploading = false;
  public previewImage = '';
  public previewVisible = false;
  public avatarList = [];
  public pictureList = [];
  public fileList = [];

  public isSubmit = false;
  public loading = false;


  constructor(private modalService: NzModalService, private msg: NzMessageService, private notification: NzNotificationService,
    private router: Router, private route: ActivatedRoute, private fb: FormBuilder,
    private mapService: MapService, private configService: ConfigService, private propertyService: PropertyService, private governmentService: GovernmentService) {


     this.sameCardProperties=[];
     this.sameCardIdChecked=false;

    this.basicInfoForm = this.fb.group({
      pName: ['', [Validators.required]],
      pType: ['', [Validators.required]],
      pAddress: ['', [Validators.required]],
      pFloor: [''],
      pFourToStation: [''],
      pGetedDate: ['', [Validators.required]],
      pGetModeId: ['', [Validators.required]],
      pIsAdmission: ['', [Validators.required]],

      //产权信息
      pRegisterEstate: ['', [Validators.required]],
      pEstateId: ['', [Validators.required]],
      pEstateTime: ['', [Validators.required]],
      pConstructId: [''],
      pConstructArea: [''],
      pConstructTime: [''],
      pLandId: [''],
      pLandArea: ['', [Validators.required]],
      pLandTime: [''],

      pGovernmentId: ['', [Validators.required]],
      pUsedPeolple: ['', [Validators.required]],
      pUseTypeId: ['', [Validators.required]],
      pCurrentTypeId: ['', [Validators.required]],
      pIsMortgage: ['', [Validators.required]],

      pDescription: ['',]
    });

    this.geoInfoForm = this.fb.group({
      pLocation: ['', [Validators.required]],
      pExtent: ['',]
    });

    this.fileInfoForm = this.fb.group({
      pLogo: ['', [Validators.required]]
    })
  }

  //#region 验证相关

  //名称验证
  propertyNameAsyncValidator = (control: FormControl) => {
    var that = this;
    return Observable.create((observer: Observer<ValidationErrors>) => {

      that.propertyService.nameValidate(control.value).subscribe(response => {

        if (this.id > 0 && control.value == this.orginalPropertyName) observer.next(null);
        else {
          if (response) {
            observer.next({ error: true, duplicated: true });
          } else {
            observer.next(null);
          }
        }

        observer.complete();
      });
    })
  }

  //资产类别变化引起的表单验证切换
  propertyTypeValidateSwicher(): void {
    if (this.property.propertyTypeId == "0"||this.property.propertyTypeId == "2") {

      this.basicInfoForm.get('pConstructArea').setValidators(Validators.required);
      this.basicInfoForm.get('pConstructArea').markAsDirty();
      this.basicFormValidateConfig.constructAreaRequired = true;

      if (this.property.registerEstate == 'false') {
        this.basicInfoForm.get('pConstructId').setValidators(Validators.required);
        this.basicInfoForm.get('pConstructId').markAsDirty();
        this.basicFormValidateConfig.constructIdRequired = true;

        this.basicInfoForm.get('pConstructTime').setValidators(Validators.required);
        this.basicInfoForm.get('pConstructTime').markAsDirty();
        this.basicFormValidateConfig.constructTimeRequired = true;
      }

    } else {

      this.basicInfoForm.get('pConstructArea').clearValidators();
      this.basicInfoForm.get('pConstructArea').markAsPristine();
      this.basicFormValidateConfig.constructAreaRequired = false;

      if (this.property.registerEstate == 'false') {
        this.basicInfoForm.get('pConstructId').clearValidators();
        this.basicInfoForm.get('pConstructId').markAsPristine();
        this.basicFormValidateConfig.constructIdRequired = false;

        this.basicInfoForm.get('pConstructTime').clearValidators();
        this.basicInfoForm.get('pConstructTime').markAsPristine();
        this.basicFormValidateConfig.constructTimeRequired = false;
      }
    }

    this.basicInfoForm.get('pFloor').updateValueAndValidity();
    this.basicInfoForm.get('pConstructArea').updateValueAndValidity();
    this.basicInfoForm.get('pConstructId').updateValueAndValidity();
    this.basicInfoForm.get('pConstructTime').updateValueAndValidity();
  }

  //登记类型变化引起的表单验证切换
  registerTypeValidateSwicher(): void {
    if (this.property.registerEstate == 'false') {
      if (this.property.propertyTypeId == "0") {
        this.basicInfoForm.get('pConstructId').setValidators(Validators.required);
        this.basicInfoForm.get('pConstructId').markAsDirty();
        this.basicFormValidateConfig.constructIdRequired = true;

        this.basicInfoForm.get('pConstructTime').setValidators(Validators.required);
        this.basicInfoForm.get('pConstructTime').markAsDirty();
        this.basicFormValidateConfig.constructTimeRequired = true;
      }
      else {
        this.basicInfoForm.get('pConstructId').clearValidators();
        this.basicInfoForm.get('pConstructId').markAsPristine();
        this.basicFormValidateConfig.constructIdRequired = false;

        this.basicInfoForm.get('pConstructTime').clearValidators();
        this.basicInfoForm.get('pConstructTime').markAsPristine();
        this.basicFormValidateConfig.constructTimeRequired = false;
      }

      this.basicInfoForm.get('pLandId').setValidators(Validators.required);
      this.basicInfoForm.get('pLandId').markAsDirty();
      this.basicFormValidateConfig.landIdRequired = true;

      this.basicInfoForm.get('pLandTime').setValidators(Validators.required);
      this.basicInfoForm.get('pLandTime').markAsDirty();
      this.basicFormValidateConfig.landTimeRequired = true;

      this.basicInfoForm.get('pEstateId').clearValidators();
      this.basicInfoForm.get('pEstateId').markAsPristine();
      this.basicFormValidateConfig.estateIdRequired = false;

      this.basicInfoForm.get('pEstateTime').clearValidators();
      this.basicInfoForm.get('pEstateTime').markAsPristine();
      this.basicFormValidateConfig.estateTimeRequired = false;
    }
    else {
      this.basicInfoForm.get('pConstructId').clearValidators();
      this.basicInfoForm.get('pConstructId').markAsPristine();
      this.basicFormValidateConfig.constructIdRequired = false;

      this.basicInfoForm.get('pConstructTime').clearValidators();
      this.basicInfoForm.get('pConstructTime').markAsPristine();
      this.basicFormValidateConfig.constructTimeRequired = false;

      this.basicInfoForm.get('pLandId').clearValidators();
      this.basicInfoForm.get('pLandId').markAsPristine();
      this.basicFormValidateConfig.landIdRequired = false;

      this.basicInfoForm.get('pLandTime').clearValidators();
      this.basicInfoForm.get('pLandTime').markAsPristine();
      this.basicFormValidateConfig.landTimeRequired = false;

      this.basicInfoForm.get('pEstateId').setValidators(Validators.required);
      this.basicInfoForm.get('pEstateId').markAsDirty();
      this.basicFormValidateConfig.estateIdRequired = true;

      this.basicInfoForm.get('pEstateTime').setValidators(Validators.required);
      this.basicInfoForm.get('pEstateTime').markAsDirty();
      this.basicFormValidateConfig.estateTimeRequired = true;
    }

    this.basicInfoForm.get('pConstructId').updateValueAndValidity();
    this.basicInfoForm.get('pConstructTime').updateValueAndValidity();
    this.basicInfoForm.get('pLandId').updateValueAndValidity();
    this.basicInfoForm.get('pLandTime').updateValueAndValidity();
  }


  //#endregion

  //government 搜索实现
  onSearch(value: string): void {
    if (value == "" || value == undefined || value == null) return;
    var that = this;
    this.isGovernmentLoading = true;
    this.governmentService.autocompleteByName(value).subscribe(response => {
      that.optionList = response.data;
      that.isGovernmentLoading = false;
    });
  }

  ngOnInit() {
    var that = this;
    that.current = 0;
    that.stepStatus = "process";//waitprocessfinisherror
    that.pictureUploadUrl = that.configService.getApiUrl() + "Media/Pictures/Upload";
    that.fileUploadUrl = that.configService.getApiUrl() + "Media/Files/Upload";

    let routeConfig = that.route.routeConfig;
    if (routeConfig.path.indexOf("create") > -1) {
      //说明是新增
      that.title = "新增资产";
    }
    else {
      that.loading = true;
      that.title = "资产变更";

      let id = parseInt(that.route.snapshot.paramMap.get('id'));
      if (id != undefined && id != null) {
        //说明是编辑页面
        that.propertyService.getUpdatedPropertyById(id).subscribe((response: any) => {
          if (response == undefined || response == null || response.Code) that.router.navigate(['/properties']);
          that.property = response;

          that.id = that.property.id;
          that.orginalPropertyName = that.property.name;

          that.property.isAdmission = that.property.isAdmission.toString();
          that.property.isMortgage = that.property.isMortgage.toString();
          that.property.registerEstate = that.property.registerEstate.toString();
          that.property.propertyTypeId = that.property.propertyTypeId.toString();
          that.property.getModeId = that.property.getModeId.toString();
          that.property.useTypeId = that.property.useTypeId.toString();
          that.property.currentTypeId = that.property.currentTypeId.toString();
          if (parseInt(that.property.governmentId) > 0 && that.property.governmentName != undefined
            && that.property.governmentId != "" && that.property.governmentId != "")
            that.property.governmentId = that.property.governmentId.toString();
          else that.property.governmentId = "";

          that.optionList.push({
            name: that.property.governmentName,
            id: that.property.governmentId
          });

          var pics = [];
          var files = [];

          that.property.pictures.forEach(element => {
            pics.push({
              uid: element.pictureId,    //Uid不能重复
              message: "成功",
              name: element.title,
              status: 'done',
              url: element.href,
              thumbUrl: element.href,
              id: element.pictureId
            });
          });


          that.property.files.forEach(element => {
            files.push({
              uid: element.fileId,
              name: element.title,
              status: 'done',
              url: element.src,
              id: element.fileId
            })
          });
          that.avatarList = [{ uid: -1, name: "logo", status: 'done', url: this.property.logoUrl, thumbUrl: this.property.logoUrl }];
          that.pictureList = [...pics];
          that.fileList = [...files];

          that.loading = false;
        });
      }
    }
  }

  ngAfterViewInit() {
  }

  pre(): void { 
    this.current -= 1;
    this.changeContent();

    console.log("pre");
     
  }

  next(): void {
console.log("next");

    var validation = false;
    var title = "数据错误", content = "";
    switch (this.current) {
      case 0:

        validation = this.basicInfoForm.valid;
        var errors = this.basicInfoForm.errors;

        console.log(this.basicInfoForm.getError('pName'));
        //console.log(this.basicInfoForm.controls['pName'].g;

        if (!validation) {
          for (const key in this.basicInfoForm.controls) {
            this.basicInfoForm.controls[key].markAsDirty();
            this.basicInfoForm.controls[key].updateValueAndValidity();
            console.log(key);
            console.log(this.basicInfoForm.controls[key].status);
            console.log("___________");
          }

          content = "基本信息填写不正确";
        }
        else {
          //预处理
          this.property.getedDate = format(new Date(this.property.getedDate), 'yyyy-MM-dd');

          if (this.property.registerEstate == "true") {
            this.property.constructId = "";
            this.property.constructTime = "";
            this.property.landId = "";
            this.property.landTime = "";
            if (this.property.estateTime != undefined) this.property.estateTime = format(new Date(this.property.estateTime), 'yyyy-MM-dd');
          }
          else {
            this.property.estateId = "";
            this.property.estateTime = "";

            if (this.property.constructTime != undefined) this.property.constructTime = format(new Date(this.property.constructTime) , 'yyyy-MM-dd');
            if (this.property.landTime != undefined) this.property.landTime = format(new Date(this.property.landTime) , 'yyyy-MM-dd');
          }
        }

        break;
      case 1:
        validation = this.geoInfoForm.valid;
        if (!validation) {
          for (const key in this.geoInfoForm.controls) {
            this.geoInfoForm.controls[key].markAsDirty();
            this.geoInfoForm.controls[key].updateValueAndValidity();
          }
          content = "空间信息填写不正确";
        }
        break;
      case 2:
        validation = this.fileInfoForm.valid;
        if (!validation) {
          for (const key in this.fileInfoForm.controls) {
            this.fileInfoForm.controls[key].markAsDirty();
            this.fileInfoForm.controls[key].updateValueAndValidity();
          }
          content = "请上传制定的资产现场照片！";
        }
        else {
          //同步照片信息
          this.property.pictures = [];
          this.pictureList.forEach(element => {
            var ppm = new PropertyPictureModel();
            if (element.uid == element.id) ppm.pictureId = element.id;
            else ppm.pictureId = element.response[0].id;
            this.property.pictures.push(ppm);
          });
          //同步文件信息
          this.property.files = [];
          console.log(this.fileList);
          this.fileList.forEach(element => {
            var pfm = new PropertyFileModel();

            if (element.uid == element.id) pfm.fileId = element.id;
            else pfm.fileId = element.response[0].id;
            this.property.files.push(pfm);
          });

          console.log(this.property);
          //获取同号资产
           
          var typeId = this.property.registerEstate == "true" ? "0" : "1";
          var number = this.property.registerEstate == "true" ? this.property.estateId : 
          (this.property.propertyTypeId=="0"?this.property.constructId:this.property.landId);


          this.propertyService.getPropertiesBySameNumberId(number, typeId,0)
            .subscribe(response => {
              var that = this;
              that.sameCardIdChecked=true;
              that.sameCardProperties=response;

              that.sameCardProperties.forEach(element => {
                if(element.isMain)
                {
                  that.property.parentPropertyId=element.id;
                  return false;
                }
              });
            });
        }
        break;
      case 3:



        break;
    }

    if (validation) {
      this.stepStatus = "process";
      this.current += 1;
      this.changeContent();
    }
    else {
      this.createNotification("error", title, content, 2000);
      this.stepStatus = "error";
    }

  }

  done(submit: boolean): void {
    var that = this;
    that.isSubmit = true;
    that.property.submit = submit;

    console.log(that.property);


    if (that.id > 0) {
      this.propertyService.updatedProperty(this.property).subscribe((response: any) => {
        if (response.Code) {
          that.createNotification("error", "数据变更申请失败", "错误原因：" + response.message, 0);
          that.isSubmit = false;
        }
        else {
          var id = response.id;
          if (id) {
            this.modalService.confirm({
              nzTitle: '提示',
              nzContent: '数据变更申请成功',
              nzOkText: '继续编辑',
              nzCancelText: '查看资产',
              nzOnOk: function () {
                window.location.reload();
                //this.router.navigate(['../properties/edit/' + id]);
              },
              nzOnCancel: function () {
                window.location.href="http://localhost:8084/#/admin/properties/"+id;
               // that.router.navigate(['../properties/' + id]);
              }
            });
          }
        }
      });
    }
    else {

      this.propertyService.createProperty(this.property).subscribe((response: any) => {
        if (response.Code) {
          that.createNotification("error", "数据入库失败", "错误原因：" + response.message, 0);
          that.isSubmit = false;
        }
        else {
          var id = response.id;
          if (id) {
            this.modalService.confirm({
              nzTitle: '提示',
              nzContent: '数据入库成功',
              nzOkText: '查看资产',
              nzCancelText: '返回列表',
              nzOnOk: function () {
                that.router.navigate(['/admin/properties/' + id]);
              },
              nzOnCancel: function () {
                that.router.navigate(['/admin/properties']);
              }
            });
          }
        }
      });
    }
  }

  //切换输入内容
  changeContent(): void {
    switch (this.current) {
      case 0:
        break;
      case 1:
        if (this.map == null || this.map == undefined) this.mapStepInitial();
        break;
      case 2:
        break;
      default:
        break;
    }
  }

  mapStepInitial(): void {
    var that = this;
    that.wkt = new Wkt.Wkt();

    setTimeout(() => {
      var normal = that.mapService.getLayer("vector");
      var satellite = that.mapService.getLayer("img");
      that.map = L.map('map', {
        crs: L.CRS.EPSG4326,
        center: [28.905527517199516, 118.50629210472107],
        zoom: 14
      });

      satellite.addTo(this.map);
      var baseLayers = {
        "矢量": normal,
        "卫星": satellite
      };
      //L.control.layers(baseLayers).addTo(that.map);

      if (that.id > 0) {
        that.wkt.read(that.property.location);

        that.marker = L.marker([that.wkt.components[0].y, that.wkt.components[0].x]);// that.wkt.toObject(mapOverlayOption);

        that.marker.addTo(that.editableLayers);
        that.map.setView(that.marker.getLatLng(), 18);

        if (that.property.extent != null && that.property.extent != "") {
          that.wkt.read(that.property.extent);
          that.extent = that.wkt.toObject(that.mapOverlayOption);
          that.extent.addTo(that.editableLayers);

          that.map.fitBounds(that.extent.getBounds());
        }
      }


      var zoomControl = that.map.zoomControl;

      zoomControl.setPosition("topright");

      that.map.addLayer(that.editableLayers);

      var options = {
        position: 'topleft',
        draw: {
          polyline: false,
          polygon: {
            allowIntersection: false, // Restricts shapes to simple polygons
            drawError: {
              color: '#e1e100', // Color the shape will turn when intersects
              message: '<strong>绘制错误！<strong> 多边形不能自重叠' // Message that will show when intersect
            },
            shapeOptions: {
              color: 'blue'
            }
          },
          circle: false,
          rectangle: false,
          marker: true,
          circlemarker: false
        },
        edit: {
          featureGroup: that.editableLayers, //REQUIRED!!
          remove: true
        }
      };

      var drawControl = new L.Control.Draw(options);
      that.map.addControl(drawControl);

      //绘制空间查询范围

      var drawPolygon = new L.Draw.Polygon(that.map, options.draw.polygon);
      var drawRectangle = new L.Draw.Rectangle(that.map, options.draw.rectangle);
      var drawCircle = new L.Draw.Circle(that.map, options.draw.circle);

      //要素绘制事件
      that.map.on(L.Draw.Event.CREATED, function (e) {
        var type = e.layerType, layer = e.layer;
        var geojson = layer.toGeoJSON();

        switch (type) {
          case "marker":
            that.editableLayers.removeLayer(that.marker);
            that.marker = layer;
            that.marker.addTo(that.editableLayers);
            that.property.location = that.wkt.fromJson(geojson).write();
            break;
          case "polygon":
            that.editableLayers.removeLayer(that.extent);
            that.extent = layer;
            that.extent.addTo(that.editableLayers);
            that.property.extent = that.wkt.fromJson(geojson).write();
            break;
        }
        console.log(that.property)
      });

      //要素编辑事件
      that.map.on(L.Draw.Event.EDITED, function (e) {
        e.layers.eachLayer(function (layer) {
          var geoJson = layer.toGeoJSON();
          if (geoJson.geometry.type == "Point") {
            var geojson = that.marker.toGeoJSON();
            that.property.location = that.wkt.fromJson(geojson).write();
          }
          else {
            var geojson = that.extent.toGeoJSON();
            that.property.extent = that.wkt.fromJson(geojson).write();
          }
        });

      });


      //要素删除事件
      that.map.on(L.Draw.Event.DELETED, function (e) {
        e.layers.eachLayer(function (layer) {
          var geoJson = layer.toGeoJSON();
          if (geoJson.geometry.type == "Point") that.property.location = "";
          else that.property.extent = "";
        });
      });

      //汉化
      // L.drawLocal.draw.toolbar.actions = {
      //   title: '取消绘制',
      //   text: '取消'
      // };
      // L.drawLocal.draw.toolbar.undo = {
      //   title: '删除最后一个已绘制的点',
      //   text: '删除最后一个点'
      // };
      // L.drawLocal.draw.toolbar.finish = {
      // 	title: '完成绘制',
      // 	text: '完成'
      // };      

      // L.drawLocal.draw.toolbar.buttons = {
      //   polyline: '绘制线',
      //   polygon: '绘制面',
      //   rectangle: '绘制矩形',
      //   circle: '绘制圆',
      //   marker: '绘制点标记'
      // };

      // // L.drawLocal.draw.handlers.circle = {
      // //   tooltip: {
      // //     start: '点击拖动绘制圆'
      // //   }
      // // };

      // L.drawLocal.draw.handlers.polygon = {
      //   tooltip: {
      //     start: '点击开始绘制',
      //     cont: '点击继续绘制',
      //     end: '点击第一个点闭合多边形'
      //   }
      // };
      // L.drawLocal.draw.handlers.marker = {
      //   tooltip: {
      //     start: '点击地图绘制'
      //   }
      // };

      // L.drawLocal.draw.handlers.polyline = {
      //   error: '<strong>错误:</strong> 图形不能交叉',
      //   tooltip: {
      //     start: '点击开始绘制',
      //     cont: '点击继续绘制',
      //     end: '点击完成绘制'
      //   }
      // };
      // L.drawLocal.draw.handlers.rectangle = {
      //   tooltip: {
      //     start: '点击拖动绘制矩形'
      //   }
      // };
      // L.drawLocal.draw.handlers.rectangle = {
      //   tooltip: {
      //     start: '点击拖动绘制矩形'
      //   }
      // };
      // L.drawLocal.draw.handlers.simpleshape = {
      //   tooltip: {
      //     end: '释放鼠标结束绘制'
      //   }
      // };
      // L.drawLocal.edit.toolbar.actions = {
      //   save: {
      //     title: '保存修改',
      //     text: '保存'
      //   },
      //   cancel: {
      //     title: '取消编辑,放弃所有修改',
      //     text: '取消'
      //   }
      // };
      // L.drawLocal.edit.toolbar.buttons = {
      //   edit: '编辑图形',
      //   editDisabled: '当前没的图形可编辑',
      //   remove: '删除图形',
      //   removeDisabled: '当前没的图形可删除'
      // };
      // L.drawLocal.edit.handlers.edit = {
      //   tooltip: {
      //     text: '点取消放弃修改',
      //     subtext: '拖动节点进行修改'
      //   }
      // };
      // L.drawLocal.edit.handlers.edit = {
      //   tooltip: {
      //     text: '右击需要删除的图形'
      //   }
      // };


      // var modifiedDraw = L.drawLocal.extend({
      //   draw: {
      //     toolbar: {
      //       actions: {
      //         title: '取消绘制',
      //         text: '取消'
      //       },
      //       undo: {
      //         title: '删除最后一个已绘制的点',
      //         text: '删除最后一个点'
      //       },
      //       buttons: {
      //         polyline: '绘制线',
      //         polygon: '绘制面',
      //         rectangle: '绘制矩形',
      //         circle: '绘制圆',
      //         marker: '绘制点标记'
      //       }
      //     },
      //     handlers: {
      //       circle: {
      //         tooltip: {
      //           start: '点击拖动绘制圆'
      //         }
      //       },
      //       marker: {
      //         tooltip: {
      //           start: '点击地图绘制'
      //         }
      //       },
      //       polygon: {
      //         tooltip: {
      //           start: '点击开始绘制',
      //           cont: '点击继续绘制',
      //           end: '双击完成绘制'
      //         }
      //       },
      //       polyline: {
      //         error: '<strong>错误:</strong> 图形不能交叉',
      //         tooltip: {
      //           start: '点击开始绘制',
      //           cont: '点击继续绘制',
      //           end: '点击完成绘制'
      //         }
      //       },
      //       rectangle: {
      //         tooltip: {
      //           start: '点击拖动绘制矩形'
      //         }
      //       },
      //       simpleshape: {
      //         tooltip: {
      //           end: '释放鼠标完成绘制'
      //         }
      //       }
      //     }
      //   },
      //   edit: {
      //     toolbar: {
      //       actions: {
      //         save: {
      //           title: '保存修改',
      //           text: '保存'
      //         },
      //         cancel: {
      //           title: '取消编辑,放弃所有修改',
      //           text: '取消'
      //         }
      //       },
      //       buttons: {
      //         edit: '编辑图形',
      //         editDisabled: '当前没的图形可编辑',
      //         remove: '删除图形',
      //         removeDisabled: '当前没的图形可删除'
      //       }
      //     },
      //     handlers: {
      //       edit: {
      //         tooltip: {
      //           text: '点取消放弃修改',
      //           subtext: '拖动节点进行修改'
      //         }
      //       },
      //       remove: {
      //         tooltip: {
      //           text: '右击需要删除的图形'
      //         }
      //       }
      //     }
      //   }
      // });

      // drawControl.drawLocal=modifiedDraw;
    }, 500);
  }

  //现场照片上传前
  beforeAvatarUpload = (file: File) => {
    console.log(file);
    const isJPG = (file.type === 'image/jpeg' || file.type === 'image/png' || file.type === 'image/bmp');
    if (!isJPG) {
      this.createNotification("error", "图片格式错误", '文件《' + file.name + '》不是图片格式数据!', 2000);
      return false;
    }
    const isLt2M = file.size / 1024 / 1024 < 2;
    if (!isLt2M) {
      this.createNotification("error", "图片大小错误", '图片《' + file.name + '》大小已经超过2MB!', 2000);
      return false;
    }
    return isJPG && isLt2M;
  }

  handleAvatarChange(info: any): void {
    if (info.file.status === 'uploading') {
      this.picureUploading = true;
      return;
    }
    if (info.file.status === 'done') {
      if (info.type == "success") {

        this.property.logoPictureId = info.file.response[0].id;
        this.property.logoUrl = info.file.response[0].url;
        this.picureUploading = false;

        // Get this url from response in real world.
        // this.getBase64(file, (img: string) => {
        //   this.picureUploading = false;
        //   this.property.logo = img;        
        // });        
      }
    }
  }

  handleAvatarPreview = (file: NzUploadFile) => {
    this.previewImage = file.url || file.thumbUrl;
    this.previewVisible = true;
  }
  handleAvatarRemove = (file: NzUploadFile) => {
    this.property.logoPictureId = 0;
    this.property.logoUrl = "";
    this.property.logo = "";

    return true;
  }
  handlePicturesChange(info: any): void {
    var that = this;

    if (info.file.status === 'uploading') {
      this.picureUploading = true;
      return;
    }
    if (info.file.status === 'done') {
      this.picureUploading = false;
    }

  }
  handleFilesChange(info: any): void {

    var that = this;
    const fileList = info.fileList;
    if (info.file.status === 'uploading') {
      that.fileUploading = true;
      return;
    }
    if (info.file.status === 'done') {
      that.fileUploading = false;
      // if (info.file.response) {
      //   info.file.url = info.file.response[0].url;
      // }

      that.fileList = fileList;
    }



  }

  createNotification(type: string, title: string, content: string, time: number): void {
    this.notification.create(type, title, content, { nzDuration: time });
  }
}
