import { Component } from '@angular/core';

import { LayoutService } from "./services/layoutService";

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.less']
})
export class AppComponent {
  title = 'CSCZJ';
  perfectScrollbarConfig={};
  containerHeight=100;

  constructor(private layoutService:LayoutService){
    //this.containerHeight=layoutService.getActualScreenSize().height;
   // this.containerHeight=this.containerHeight-80-54;  //计算除了header footer的高度
  }
}
