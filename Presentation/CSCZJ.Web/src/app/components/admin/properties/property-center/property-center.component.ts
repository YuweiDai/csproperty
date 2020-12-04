import { Component, OnInit ,ViewEncapsulation } from '@angular/core';
import { LeftmenuComponent } from "../../common/leftmenu/leftmenu.component";
import { LayoutService } from "../../../../services/layoutService";
import { LeftMenuItem } from "../../../../viewModels/layout/LeftMenuItem";
import{ActivatedRoute,Params} from  '@angular/router';


@Component({
  templateUrl: './property-center.component.html',
  styleUrls: ['./property-center.component.less'],
  encapsulation: ViewEncapsulation.None,
})
export class PropertyCenterComponent implements OnInit {
  perfectScrollbarConfig={};
  containerHeight=100;
  menuItems:LeftMenuItem[];
  Id=0;

  constructor(private layoutService:LayoutService,private activateInfo:ActivatedRoute){
    this.containerHeight=layoutService.getActualScreenSize().height;
    this.containerHeight=layoutService.getContentHeight();  //获取内容高度

    this.menuItems= [
      { icon: "icon-liebiao", title: "资产管理",url:"./",active:true },
      { icon: "icon-liebiao", title: "出租管理",url:"./rentlist",active:false },
      { icon: "icon-liebiao", title: "巡查管理",url:"./patrollist",active:false },
      // { icon: "icon-xinzeng", title: "新增资产",url:"./create",active:false },
     // { icon: "icon-shenpi", title: "审批管理",url:"./",active:false }
    ];

    activateInfo.queryParams.subscribe(queryParams => {
       this.Id = queryParams.id })

      if(this.Id==1){
        this.menuItems[0].active=false;
        this.menuItems[1].active=true;
      }


  }

  ngOnInit() {
    console.log(123);
  }

}
