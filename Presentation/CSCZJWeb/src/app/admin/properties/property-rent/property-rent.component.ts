import { Component, OnInit } from '@angular/core';
import { PropertyRentModel, PropertyPictureModel, PropertyFileModel } from 'src/app/viewModels/properties/property';
import { FormGroup, FormBuilder, FormControl, Validators } from '@angular/forms';
import { NzModalService, NzMessageService, NzNotificationService, UploadFile } from 'ng-zorro-antd';
import { Router, ActivatedRoute } from '@angular/router';
import { ConfigService } from 'src/app/services/config.service';
import { PropertyService } from 'src/app/services/property.service';
import { format, differenceInCalendarYears, differenceInCalendarMonths } from 'date-fns'


@Component({
  selector: 'app-property-rent',
  templateUrl: './property-rent.component.html',
  styleUrls: ['./property-rent.component.less']
})
export class PropertyRentComponent implements OnInit {
  private rentId: number;
  private pid: number;
  propertyRent = new PropertyRentModel();
  basicInfoForm: FormGroup;
  title="新增出租";

  private isPropertyLoading = false;
  private optionList = [];
  private selectedProperties = [];

  timeRange=[] ;
  startTime="起始时间";
  endTime="结束时间";
  private priceControls = [];

  private pictureUploadUrl: string;
  private picureUploading = false;
  private fileUploadUrl: string;
  private fileUploading = false;
  private previewImage = '';
  private previewVisible = false;
  private pictureList = [];
  private fileList = [];

  private isSubmit = false;
  private loading = false;
  dateFormat = 'yyyy/MM/dd';
  constructor(private modalService: NzModalService, private msg: NzMessageService, private notification: NzNotificationService,
    private router: Router, private route: ActivatedRoute, private fb: FormBuilder,
    private configService: ConfigService, private propertyService: PropertyService) {
  }


  ngOnInit() {
    console.log(111);
    var that = this;
    that.pictureUploadUrl = that.configService.getApiUrl() + "Media/Pictures/Upload";
    that.fileUploadUrl = that.configService.getApiUrl() + "Media/Files/Upload";

    let routeConfig = that.route.routeConfig;
    if (routeConfig.path.indexOf("rentedit") > -1) {
      //说明是新增
      that.title = "出租变更";
      that.loading = true;
      let id = parseInt(that.route.snapshot.paramMap.get('id'));

      that.propertyService.getRentById(id).subscribe((response:any)=>{
        that.loading = false;
         console.log(response);
         that.pid = response.property_Id;
         that.propertyService.getPropertyById(that.pid, true).subscribe((repsonse: any) => {

          if (repsonse != null && repsonse != undefined && !repsonse.Code) {
              that.optionList.push({
                name: repsonse.name,
                id: that.pid
              });
  
              that.selectedProperties.push(that.pid);

          }
          that.loading = false;
        });
         that.propertyRent.id=response.id;
         that.propertyRent.name=response.name;
         that.propertyRent.rentArea = response.rentArea;
         that.propertyRent.reamrk= response.remark;
         that.startTime = response.rentTime;
         that.endTime = response.backTime;
         that.timeRange.push(new Date(response.rentTime));
         that.timeRange.push(new Date(response.backTime));
         var prices=response.priceString.split(";");
         that.priceControls.forEach(control => {
          that.basicInfoForm.removeControl("pRentYear" + control.index);
          that.basicInfoForm.removeControl("checked" + control.index);
        });
  
        that.priceControls = [];
         for(var i=1;i<=prices.length;i++){
           var data=prices[i-1].split("_");
           var ch=false;
           if(data[1]=="true") ch=true;
          var d = {
            index: i,
            price: data[0],
            checked:ch
          }
          that.priceControls.push(d);
  
          //动态增加表单
          that.basicInfoForm.addControl("pRentYear" + i, new FormControl(data[0], Validators.required));
          that.basicInfoForm.addControl("checked" + i, new FormControl(ch, Validators.required));
         }
         that.pictureList=[];
         response.rentPictures.forEach(element=>{
          var picture={
              uid: element.pictureId,    //Uid不能重复
              message: "成功",
              name: element.title,
              status: 'done',
              url: element.href,
              thumbUrl: element.href,
              id: element.pictureId
    
          }
          that.pictureList.push(picture);
         })

         that.fileList=[];
         response.rentFiles.forEach(element=>{
          var file={
            uid: element.fileId,
              name: element.title,
              status: 'done',
              url: element.src,
              id: element.fileId
    
          }
          that.fileList.push(file);

         })
      })

    }
    else{
      that.title = "新增出租";
      that.pid = parseInt(that.route.snapshot.queryParamMap.get('pid'));
    }


    
    if (that.pid > -1) {

      that.loading = true;
      //传入待处理的
      that.propertyService.getPropertyById(that.pid, true).subscribe((repsonse: any) => {

        if (repsonse != null && repsonse != undefined && !repsonse.Code) {

          if (!repsonse.locked) {
            that.optionList.push({
              name: repsonse.name,
              id: that.pid
            });

            that.selectedProperties.push(that.pid);
          }
          else that.pid = 0;
        }
        that.loading = false;
      });
    }

    that.basicInfoForm = this.fb.group({
      pIds: [that.selectedProperties, [Validators.required]],
      pName: [that.propertyRent.name, [Validators.required]],
      pTimeRange: [that.timeRange, [Validators.required]],
      // pPriceString: [that.propertyRent.priceString, [Validators.required]],
      pRentArea: [that.propertyRent.rentArea, [Validators.required]],
      pRemark: [that.propertyRent.reamrk,]
    });
  }

  //government 搜索实现
  onSearch(value: string): void {
    if (value == "" || value == undefined || value == null) return;
    var that = this;
    this.isPropertyLoading = true;
    this.propertyService.getProcessPropertyByName(value).subscribe(response => {
      that.optionList = response;
      that.isPropertyLoading = false;
    });
  }

  timeRangeChange(): void {
    if (this.timeRange != undefined && this.timeRange != null&& this.timeRange.length > 1) {
     
        var startDate = this.timeRange[0];
        var endDate = this.timeRange[1];
  
        var months = differenceInCalendarMonths(endDate, startDate);
        if (months == 0) months = 1;
  
        //删除原有价格表单
        this.priceControls.forEach(control => {
          this.basicInfoForm.removeControl("pRentYear" + control.index);
          this.basicInfoForm.removeControl("checked" + control.index);
        });
  
        this.priceControls = [];
  
        for (var i = 1; i <= Math.ceil(months / 12); i++) {
          var d = {
            index: i,
            price: 1,
            checked:false
          }
          this.priceControls.push(d);
  
          //动态增加表单
          this.basicInfoForm.addControl("pRentYear" + i, new FormControl(1, Validators.required));
          this.basicInfoForm.addControl("checked" + i, new FormControl(false, Validators.required));
        }
      


    } else this.priceControls = [];
  }


  save(submit: boolean): void {
    var that = this;
    var title = "数据错误", content = "";
    var validation = this.basicInfoForm.valid;
    if(this.title=="出租变更")validation=true;

    if (!validation) {
      for (const key in this.basicInfoForm.controls) {
        this.basicInfoForm.controls[key].markAsDirty();
        this.basicInfoForm.controls[key].updateValueAndValidity();
        console.log(key);
        console.log(this.basicInfoForm.controls[key].status);
        console.log("___________");
      }

      content = "信息填写不正确";

      this.createNotification("error", title, content, 2000);
    }
    else {
      this.propertyRent.submit = submit;

      //资产Id处理      
      this.propertyRent.ids = "";
      this.selectedProperties.forEach(id => {
        this.propertyRent.ids += id + ";";
      });

      //起止日期处理
      var startDate = this.timeRange[0];
      var endDate = this.timeRange[1];

      this.propertyRent.rentTime = format(startDate, 'YYYY/MM/DD');
      this.propertyRent.backTime = format(endDate, 'YYYY/MM/DD');

      //租金处理
      this.propertyRent.priceString = "";
      this.priceControls.forEach(priceControl => {
        this.propertyRent.priceString += priceControl.price+ "_" +priceControl.checked + ";";
      })


      //同步照片信息
      this.propertyRent.rentPictures = [];
      this.pictureList.forEach(element => {
        var ppm = new PropertyPictureModel();
        if (element.uid == element.id) ppm.pictureId = element.id;
        else ppm.pictureId = element.response[0].id;
        this.propertyRent.rentPictures.push(ppm);
      });
      //同步文件信息
      this.propertyRent.rentFiles = [];
      console.log(this.fileList);
      this.fileList.forEach(element => {
        var pfm = new PropertyFileModel();

        if (element.uid == element.id) pfm.fileId = element.id;
        else pfm.fileId = element.response[0].id;
        this.propertyRent.rentFiles.push(pfm);
      });

      if (that.rentId > 0) {

      }
      else {

      if(this.title=="新增出租"){

        this.propertyService.createPropertyRentRecord(this.propertyRent).subscribe((response: any) => {
          if (response!=undefined && response.Code) {
            that.createNotification("error", "数据出租申请失败", "错误原因：" + response.Message, 0);
            that.isSubmit = false;
          }
          else {
            var url='/admin/properties';
            if (this.selectedProperties.length==1) {
              url+="/"+this.selectedProperties[0];
            }

            this.modalService.confirm({
              nzTitle: '提示',
              nzContent: '数据出租申请成功',
              nzOkText: '确定',
              nzCancelText:null,
              nzOnOk: function () {
                that.router.navigate([url]);
              }
            });
          }
        });
        console.log(this.propertyRent);

      }
      else{
        this.propertyService.updatedRent(this.propertyRent).subscribe((response:any)=>{
          if (response!=undefined && response.Code) {
            that.createNotification("error", "数据出租变更失败", "错误原因：" + response.Message, 0);
            that.isSubmit = false;
          }
          else {
            var url='/admin/properties';
            if (this.selectedProperties.length==1) {
              url+="/"+this.selectedProperties[0];
            }

            this.modalService.confirm({
              nzTitle: '提示',
              nzContent: '数据出租变更成功',
              nzOkText: '确定',
              nzCancelText:null,
              nzOnOk: function () {
                that.router.navigate([url]);
              }
            });
          }
        })
      }




      }


    }
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
  handleAvatarPreview = (file: UploadFile) => {
    this.previewImage = file.url || file.thumbUrl;
    this.previewVisible = true;
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
