import { Component, OnInit } from '@angular/core';
import {Router} from '@angular/router'

@Component({
  selector: 'app-header',
  templateUrl: './header.component.html',
  styleUrls: ['./header.component.less']
})
export class HeaderComponent implements OnInit {
  visible:boolean;
  
  constructor(private router: Router) { }

  data: any[] = [{
    value: 'logout',
    label: '退出登录',
  }];

  ngOnInit() {
  }

  Exit() {
    console.log("444");
    this.router.navigate(['/login']);
  }

  handle(event: object): void {
    console.log(event);
  }

}
