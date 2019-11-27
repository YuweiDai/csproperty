import { Injectable } from '@angular/core';
import { ScreenSize } from "../viewModels/layout/ScreenSize";

@Injectable()
export class LayoutService{
    headerHeight:number;
    footerHeight:number;

    constructor(){
        this.headerHeight=80;
        this.footerHeight=51;
    }

    getActualScreenSize():ScreenSize{
        var s=new ScreenSize();

        s.height=window.innerHeight;;
        s.width=window.innerWidth;

        console.log(s);
        return s;
    }

    getContentHeight():number{
        return this.getActualScreenSize().height-this.headerHeight-this.footerHeight;
    }
}