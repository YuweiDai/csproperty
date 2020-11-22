import { Component, OnInit , Input} from '@angular/core';

import { LeftMenuItem } from "../../../../viewModels/layout/LeftMenuItem";

@Component({
  selector: 'app-leftmenu',
  templateUrl: './leftmenu.component.html',
  styleUrls: ['./leftmenu.component.less']
})
export class LeftmenuComponent implements OnInit {
  @Input() menuItems:LeftMenuItem[];
  @Input() height:number;

  private currentItem:LeftMenuItem;

  constructor() { }

  ngOnInit(): void {

    this.currentItem=  this.menuItems.filter(m=>m.active)[0];  //设置初始选中的菜单项
  }

  menuClick(item:LeftMenuItem):void{
 
    this.currentItem.active=false;
    item.active=true;

    this.currentItem=item;
  }

}
