import { Component, OnInit } from '@angular/core';

interface DataItem {
  name: string;
  type: string;
  address: string;
}

@Component({
  selector: 'app-property-filelist',
  templateUrl: './property-filelist.component.html',
  styleUrls: ['./property-filelist.component.less']
})
export class PropertyFilelistComponent implements OnInit {

  searchValue = '';
  searchnameValue="";
  visible = false;
  namevisible=false;
  
 
  listOfData: DataItem[] = [
    {
      name: 'John Brown',
      type: "历史档案",
      address: '天马路11号'
    },
    {
      name: 'Jim Green',
      type: "历史档案",
      address: '红旗街235号'
    },
    {
      name: 'Joe Black',
      type: "历史档案",
      address: '定阳北路777号'
    },
    {
      name: 'Jim Red',
      type: "政府文件",
      address: '市民广场中心体育场'
    },
    {
      name: 'John Brown',
      type: "历史档案",
      address: '天马路11号'
    },
    {
      name: 'Jim Green',
      type: "历史档案",
      address: '红旗街235号'
    },
    {
      name: 'Joe Black',
      type: "历史档案",
      address: '定阳北路777号'
    },
    {
      name: 'Jim Red',
      type: "政府文件",
      address: '市民广场中心体育场'
    },
    {
      name: 'John Brown',
      type: "历史档案",
      address: '天马路11号'
    },
    {
      name: 'Jim Green',
      type: "历史档案",
      address: '红旗街235号'
    },
    {
      name: 'Joe Black',
      type: "历史档案",
      address: '定阳北路777号'
    },
    {
      name: 'Jim Red',
      type: "政府文件",
      address: '市民广场中心体育场'
    }
  ];
  listOfDisplayData = [...this.listOfData];

  reset(): void {
    this.searchValue = '';
    this.search();
  }

  search(): void {
    this.visible = false;
    this.listOfDisplayData = this.listOfData.filter((item: DataItem) => item.address.indexOf(this.searchValue) !== -1);
  }

  resetname(): void {
    this.searchnameValue = '';
    this.search();
  }

  searchname(): void {
    this.namevisible = false;
    this.listOfDisplayData = this.listOfData.filter((item: DataItem) => item.name.indexOf(this.searchnameValue) !== -1);
  }

  constructor() { }

  ngOnInit(): void {


  }

}
