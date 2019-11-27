import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormControl, FormGroup, ValidationErrors, Validators, ValidatorFn } from '@angular/forms';
import { Observable } from 'rxjs/Observable';
import { Observer } from 'rxjs/Observer';
import { Router, ActivatedRoute, ParamMap } from '@angular/router';

import { NzMessageService, UploadFile, NzNotificationService, NzModalService } from 'ng-zorro-antd';

import { format, differenceInCalendarYears, differenceInCalendarMonths } from 'date-fns'

import { PropertyOffModel, PropertyPictureModel, PropertyFileModel } from '../../../../viewModels/Properties/property';

import { PropertyService } from '../../../../services/propertyService';
import { ConfigService } from '../../../../services/configService';

@Component({
  selector: 'app-property-off',
  templateUrl: './property-off.component.html',
  styleUrls: ['./property-off.component.less']
})
export class PropertyOffComponent implements OnInit {
  private offId: number;
  private pid: number;
  propertyOff = new PropertyOffModel();
  basicInfoForm: FormGroup;

  private isPropertyLoading = false;
  private optionList = [];
  private selectedProperties = [];

  private timeRange = "";
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
    var that = this;
    that.pictureUploadUrl = that.configService.getApiUrl() + "Media/Pictures/Upload";
    that.fileUploadUrl = that.configService.getApiUrl() + "Media/Files/Upload";

    that.pid = parseInt(that.route.snapshot.queryParamMap.get('pid'));
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
      pReason: [that.propertyOff.reason, [Validators.required]],
      pOffTime: [that.propertyOff.offTime, [Validators.required]],
      pOffTypeId: [that.propertyOff.offTypeId, [Validators.required]],
      pRemark: [that.propertyOff.reamrk,]
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

  save(submit: boolean): void {
    var that = this;
    var title = "数据错误", content = "";
    var validation = this.basicInfoForm.valid;

    if (!validation) {
      for (const key in this.basicInfoForm.controls) {
        this.basicInfoForm.controls[key].markAsDirty();
        this.basicInfoForm.controls[key].updateValueAndValidity();
      }

      content = "信息填写不正确";

      this.createNotification("error", title, content, 2000);
    }
    else {
      this.propertyOff.submit = submit;

      //资产Id处理      
      this.propertyOff.ids = "";
      this.selectedProperties.forEach(id => {
        this.propertyOff.ids += id + ";";
      });
      this.propertyOff.offTime = format(this.propertyOff.offTime, 'YYYY/MM/DD');
      //同步照片信息
      this.propertyOff.offPictures = [];
      this.pictureList.forEach(element => {
        var ppm = new PropertyPictureModel();
        if (element.uid == element.id) ppm.pictureId = element.id;
        else ppm.pictureId = element.response[0].id;
        this.propertyOff.offPictures.push(ppm);
      });
      //同步文件信息
      this.propertyOff.offFiles = [];
      console.log(this.fileList);
      this.fileList.forEach(element => {
        var pfm = new PropertyFileModel();

        if (element.uid == element.id) pfm.fileId = element.id;
        else pfm.fileId = element.response[0].id;
        this.propertyOff.offFiles.push(pfm);
      });

      if (that.offId > 0) {

      }
      else {
        this.propertyService.createPropertyOffRecord(this.propertyOff).subscribe((response: any) => {
          if (response!=undefined && response.Code) {
            that.createNotification("error", "数据核销申请失败", "错误原因：" + response.Message, 0);
            that.isSubmit = false;
          }
          else {
            var url='../properties';
            if (this.selectedProperties.length==1) {
              url+="/"+this.selectedProperties[0];
            }

            this.modalService.confirm({
              nzTitle: '提示',
              nzContent: '数据核销申请成功',
              nzOkText: '确定',
              nzCancelText:null,
              nzOnOk: function () {
                that.router.navigate([url]);
              }
            });
          }
        });
        console.log(this.propertyOff);
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
