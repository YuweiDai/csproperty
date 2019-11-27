import { Component, OnInit,ViewEncapsulation } from '@angular/core';
import { Router, ActivatedRoute, ParamMap } from '@angular/router';

@Component({
  selector: 'app-header',
  templateUrl: './header.component.html',
  styleUrls: ['./header.component.less'],
  encapsulation: ViewEncapsulation.None // None | Emulated | Native  暂时先不加属性限制 后期完善
})
export class HeaderComponent implements OnInit {

  constructor(private router: Router){}

  data: any[] = [{
    value: 'logout',
    label: '退出登录',
  }];

  ngOnInit() {
  }

  Exit(){
console.log("444");
    this.router.navigate(['/login']);  
  }

  handle(event: object): void {
    console.log(event);
  }
}