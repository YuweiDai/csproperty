import { Component, OnInit } from '@angular/core';
import { PropertyService } from 'src/app/services/property.service';
import { LayoutService } from 'src/app/services/layout.service';
import { TableOption } from 'src/app/viewModels/common/TableOption';
import { ExportModel } from 'src/app/viewModels/properties/property';

@Component({
  selector: 'app-property-list',
  templateUrl: './property-list.component.html',
  styleUrls: ['./property-list.component.less']
})
export class PropertyListComponent implements OnInit {

  contentHeight: number;
  data: any[];
  tableOption: TableOption;
  loading: boolean;
  httpResponse = {
    responseType: "arraybuffer"
  }

  private exportModel = new ExportModel();
  isVisible = false;

  showModal(): void {
    this.isVisible = true;
  }

  handleOk(): void {

    for (let u of this.Attributes) {

      switch (u.value) {

        case 'isName':
          this.exportModel.isName = u.checked;
          break;
        case 'isAddress':
          this.exportModel.isAddress = u.checked;
          break;
        case 'isGovermentId':
          this.exportModel.isGoverment = u.checked;
          break;
        case 'isPropertyType':
          this.exportModel.isPropertyType = u.checked;
          break;
        case 'isRegion':
          this.exportModel.isRegion = u.checked;
          break;
        case 'isGetMode':
          this.exportModel.isGetMode = u.checked;
          break;
        case 'isPropertyID':
          this.exportModel.isPropertyID = u.checked;
          break;
        case 'isUsedPeople':
          this.exportModel.isUsedPeople = u.checked;
          break;
        case 'isFourToStation':
          this.exportModel.isFourToStation = u.checked;
          break;
        case 'isEstatedId':
          this.exportModel.isEstateId = u.checked;
          break;
        case 'isConstructArea':
          this.exportModel.isConstructArea = u.checked;
          break;
        case 'isConstructId':
          this.exportModel.isConstructId = u.checked;
          break;
        case 'isLandArea':
          this.exportModel.isLandArea = u.checked;
          break;
        case 'isCurrentType':
          this.exportModel.isCurrentType = u.checked;
          break;
        case 'isUsedType':
          this.exportModel.isUsedType = u.checked;
          break;
      }
    }

    for (let v of this.unit) {
      if (v.value != null || v.value != undefined) {
        this.exportModel.govermentids += v.value + ";";
      }

    }
    this.exportModel.propertyids = "";
    console.log("111");
    this.propertyService.exportProperty(this.exportModel).subscribe((response: any) => {


      var blob = new Blob([response], { type: "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet" });
      var objectUrl = URL.createObjectURL(blob);
      var a = document.createElement("a");
      document.body.appendChild(a);
      a.setAttribute("style", "display:none");
      a.setAttribute("href", objectUrl);
      a.setAttribute("download", "资产下载");
      a.click();
      URL.revokeObjectURL(objectUrl);

    })

    console.log('Button ok clicked!');
    this.isVisible = false;
  }

  handleCancel(): void {
    console.log('Button cancel clicked!');
    this.isVisible = false;
  }

  allChecked = true;
  Unitindeterminate = true;
  allAttributesChecked = true;
  Attributesindeterminate = true;

  unit = [
    { label: '常山县财政局', value: '1', checked: true },
    { label: '常山县公路管理局', value: '2', checked: true },
    { label: '常山县教育局', value: '3', checked: true }
  ];
  Attributes = [
    { label: '资产名称', value: 'isName', checked: true },
    { label: '资产地址', value: 'isAddress', checked: true },
    { label: '所属单位', value: 'isGovermentId', checked: true },
    { label: '资产类别', value: 'isPropertyType', checked: true },
    { label: '所属乡镇', value: 'isRegion', checked: true },
    { label: '获取方式', value: 'isGetMode', checked: true },
    { label: '产权证号', value: 'isPropertyID', checked: true },
    { label: '资产使用', value: 'isUsedPeople', checked: true },
    { label: '四至情况', value: 'isFourToStation', checked: true },
    { label: '不动产证', value: 'isEstatedId', checked: true },
    { label: '建筑面积', value: 'isConstructArea', checked: true },
    { label: '房产证号', value: 'isConstructId', checked: true },
    { label: '土地面积', value: 'isLandArea', checked: true },
    { label: '使用现状', value: 'isCurrentType', checked: true },
    { label: '资产用途', value: 'isUsedType', checked: true }
  ];

  updateAllUnitChecked(): void {
    this.Unitindeterminate = false;
    if (this.allChecked) {
      this.unit.forEach(item => item.checked = true);
    } else {
      this.unit.forEach(item => item.checked = false);
    }
  }

  updateUnitChecked(): void {
    if (this.unit.every(item => item.checked === false)) {
      this.allChecked = false;
      this.Unitindeterminate = false;
    } else if (this.unit.every(item => item.checked === true)) {
      this.allChecked = true;
      this.Unitindeterminate = false;
    } else {
      this.Unitindeterminate = true;
    }
  }

  updateAllAttributesChecked(): void {
    this.Attributesindeterminate = false;
    if (this.allAttributesChecked) {
      this.Attributes.forEach(item => item.checked = true);
    } else {
      this.Attributes.forEach(item => item.checked = false);
    }
  }

  updateAttributesChecked(d): void {
    d.checked = !d.checked;
    if (this.Attributes.every(item => item.checked === false)) {
      this.allAttributesChecked = false;
      this.Attributesindeterminate = false;
    } else if (this.Attributes.every(item => item.checked === true)) {
      this.allAttributesChecked = true;
      this.Attributesindeterminate = false;
    } else {
      this.Attributesindeterminate = true;
    }
  }

  constructor(
    private propertyService: PropertyService,
    private layoutService: LayoutService) {

    this.contentHeight = layoutService.getContentHeight() - 70 - 2;

    this.loading = true;
  }

  ngOnInit() {
    this.tableOption = {
      pageSize: {
        pageIndex: 1,
        pageSize: 15,
        filterCount: 0,
        total: 0
      },
      columns: [
        { name: "name", title: "资产名称", width: 350, left: 70, center: false, showSort: true },
        { name: "propertyType", title: "类别", width: 100, left: 420, center: true, showSort: true },
        { name: "address", title: "坐落位置", width: 300, left: 0, center: true, showSort: true },
        { name: "fourToStation", title: "四至情况", width: 300, left: 0, center: true, showSort: true },
        { name: "governmentName", title: "权属单位", width: 300, left: 0, center: true, showSort: true },
        { name: "getMode", title: "获取方式", width: 150, left: 0, center: true, showSort: true },
        { name: "getedDate", title: "取得时间", width: 150, left: 0, center: true, showSort: true },
        { name: "floor", title: "层数", width: 90, left: 0, center: true, showSort: true },
        { name: "propertyId", title: "产权证号", width: 150, left: 0, center: true, showSort: true },
        { name: "constructorArea", title: "建筑面积", width: 150, left: 0, center: true, showSort: true },
        { name: "constructId", title: "房产证", width: 400, left: 0, center: true, showSort: true },
        // {name:"constructTime",title:"房产证发证时间",width:150,left:0,center:true,showSort:true},
        { name: "landArea", title: "土地面积", width: 150, left: 0, center: true, showSort: true },
        { name: "landId", title: "土地证", width: 400, left: 0, center: true, showSort: true },
        // {name:"landTime",title:"土地证发证时间",width:100,left:0,center:true,showSort:true},
        { name: "usedPeople", title: "使用人员", width: 300, left: 0, center: true, showSort: true },
        { name: "currentType", title: "使用现状", width: 150, left: 0, center: true, showSort: true },
        { name: "useType", title: "用途", width: 100, left: 0, center: true, showSort: true },
        { name: "isAdmission", title: "入账", width: 100, left: 0, center: true, showSort: true },
        { name: "isMortgage", title: "抵押", width: 100, left: 0, center: true, showSort: true }
      ],
      nzScroll: {}
    };

    var fullWidth = 370;
    this.tableOption.columns.forEach(element => {
      console.log(fullWidth);
      fullWidth += element.width;
    });

    this.tableOption.nzScroll = { x: fullWidth + "px" };

    console.log(this.tableOption);
  }

  getAllProperties($event): void {
    console.log("start");
    this.loading = true;
    this.propertyService.getAllProperties($event)
      .subscribe(response => {
        var that = this;
        setTimeout(function () {
          that.loading = false;
          if (response.data != undefined && response.data != null) {
            that.data = response.data;
            that.tableOption.pageSize = response.paging;
          }
        }, 200);


      });
  }

}
