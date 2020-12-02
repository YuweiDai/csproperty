import { Component, OnInit } from '@angular/core';
import { TablePageSize,TableColumn,TableOption } from "../../../../viewModels/common/TableOption";
import { PropertyService } from '../../../../services/propertyService';

@Component({
  selector: 'app-property-patrollist',
  templateUrl: './property-patrollist.component.html',
  styleUrls: ['./property-patrollist.component.less']
})
export class PropertyPatrollistComponent implements OnInit {
  tabs = [ "全部","今年巡查", "往年巡查" ];
  data:any[];
  tableOption:TableOption;
  isVisible=false;
  isOkLoading=false;
  pageIndex = 1;
  pageSize = 10;
  total = 1;
  dataSet = [];
  allpatrols=[];
  searchpatrols=[];
  loading = true;
  sortValue = null;
  sortKey = null;
  tabKey="全部";
  monthlist:any[];
  dateFormat = 'yyyy/MM/dd';
 

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
    this.propertyService.getPatrols(this.pageIndex, this.pageSize, this.sortKey, this.sortValue, this.tabKey).subscribe((data: any) => {
      this.loading = false;
      this.total = data.paging.total;
      this.allpatrols=data.data;
      this.dataSet = data.data;
    });
  }

  updateFilter(value: string[]): void {
    this.searchGenderList = value;
    this.searchData(true);
  }

  expotToExl():void{

    this.isVisible = true;

  }

 handleOk(): void {
    this.isOkLoading = true;
    setTimeout(() => {
      this.isVisible = false;
      this.isOkLoading = false;
    }, 1000);
  }

  handleCancel(): void {
    this.isVisible = false;
  }

  addmonth(number:any){

    if(this.monthlist.indexOf(number)>-1){
      this.monthlist.splice(number,1);
      
    }
    else{
      this.monthlist.push(number);
    }

    this.dataSet.forEach(data=>{
      if(this.monthlist.indexOf(new Date(data.patrolDate).getMonth()+1)){
        this.searchpatrols.push(data);
      }
    })

    this.dataSet=this.searchpatrols;
    

  }

  getPatrols(index):void{

    switch(index){

      case "今年巡查":
      this.tabKey="今年巡查";
      this.searchData();
      break;

      case "往年巡查":
      this.tabKey="往年巡查";
      this.searchData();
      break;

      case "全部":
      this.tabKey="全部";
      this.searchData();
      break;

    }
    
  }



  constructor(private propertyService:PropertyService) { }

  ngOnInit() {
    console.log(1111);
    this.searchData();
    
  }

}
