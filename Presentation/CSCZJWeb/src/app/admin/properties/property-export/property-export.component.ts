import { Component, OnInit } from '@angular/core';

@Component({
  selector: 'app-property-export',
  templateUrl: './property-export.component.html',
  styleUrls: ['./property-export.component.less']
})
export class PropertyExportComponent implements OnInit {

  allChecked = true;
  Unitindeterminate = true;
  allAttributesChecked=true;
  Attributesindeterminate=true;

  unit = [
    { label: '常山县财政局', value: '1', checked: true },
    { label: '常山县公路管理局', value: '2', checked: true },
    { label: '常山县教育局', value: '3', checked: true }
  ];
  Attributes= [
    { label: '名称', value: 'isName', checked: true },
    { label: '地址', value: 'isAddress', checked: true },
    { label: '所属单位', value: 'isGovermentId', checked: true },
    { label: '资产类别', value: 'isPropertyType', checked: true },
    { label: '所属乡镇', value: 'isRegion', checked: true },
    { label: '获取方式', value: 'isGetMode', checked: true },
    { label: '产权证号', value: 'isPropertyID', checked: true },
    { label: '使用方', value: 'isUsedPeople', checked: true },
    { label: '四至情况', value: 'isFourToStation', checked: true },
    { label: '不动产证', value: 'isEstatedId', checked: true },
    { label: '建筑面积', value: 'isConstructArea', checked: true },
    { label: '房产证', value: 'isConstructId', checked: true },
    { label: '土地面积', value: 'isLandArea', checked: true },
    { label: '使用现状', value: 'isCurrentType', checked: true },
    { label: '用途', value: 'isUsedType', checked: true }
  ];


  constructor() { }

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

  updateAttributesChecked(): void {
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


  ngOnInit() {

  }

}
