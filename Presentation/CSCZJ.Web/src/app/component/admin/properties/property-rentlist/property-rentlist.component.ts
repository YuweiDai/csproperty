import { Component, OnInit } from '@angular/core';
import { TablePageSize,TableColumn,TableOption } from "../../../../viewModels/common/TableOption";
import { PropertyService } from '../../../../services/propertyService';

@Component({
  selector: 'app-property-rentlist',
  templateUrl: './property-rentlist.component.html',
  styleUrls: ['./property-rentlist.component.less']
})
export class PropertyRentlistComponent implements OnInit {
  tabs = [ "即将过期", "已经过期", "全部信息" ];
  data:any[];
  tableOption:TableOption;
  rentdata=1;
  httpResponse={
    responseType: "arraybuffer"
  }
  isVisible=false;
  isOkLoading=false;
  pageIndex = 1;
  pageSize = 10;
  total = 1;
  dataSet = [];
  loading = true;
  sortValue = null;
  sortKey = null;
  tabKey="即将过期";



  filterGender = [
    { text: 'male', value: 'male' },
    { text: 'female', value: 'female' }
  ];
  searchGenderList: string[] = [];

  sort(sort: { key: string, value: string }): void {
    this.sortKey = sort.key;
    this.sortValue = sort.value;
    this.searchData();
  }

  searchData(reset: boolean = false): void {
    if (reset) {
      this.pageIndex = 1;
    }
    this.loading = true;
    this.propertyService.getUsers(this.pageIndex, this.pageSize, this.sortKey, this.sortValue, this.tabKey).subscribe((data: any) => {
      this.loading = false;
      this.total = data.paging.total;
      this.dataSet = data.data;
    });
  }

  updateFilter(value: string[]): void {
    this.searchGenderList = value;
    this.searchData(true);
  }


  getRents(index):void{

    switch(index){

      case "即将过期":
      this.tabKey="即将过期";
      this.searchData();
      break;

      case "已经过期":
      this.tabKey="已经过期";
      this.searchData();
      break;

      case "全部信息":
      this.tabKey="全部信息";
      this.searchData();
      break;

    }
    
  }

    
  expotToExl():void{

    this.isVisible = true;

  }

  

  constructor( private propertyService:PropertyService) { 
   
   
  }

  ngOnInit() {
    this.searchData();
    console.log(123);
  }


  handleOk(): void {
    this.isOkLoading = true;
    setTimeout(() => {
      this.isVisible = false;
      this.isOkLoading = false;
    }, 1000);
    
    this.propertyService.exportToExl().subscribe((response:any) => {


      var blob = new Blob([response], {type: "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"});  
      var objectUrl = URL.createObjectURL(blob);  
      var a = document.createElement("a");
      document.body.appendChild(a);
      a.setAttribute("style", "display:none");
      a.setAttribute("href", objectUrl);
      a.setAttribute("download", "出租表");
      a.click();
      URL.revokeObjectURL(objectUrl); 

    })

  }

  handleCancel(): void {
    this.isVisible = false;
  }


}
