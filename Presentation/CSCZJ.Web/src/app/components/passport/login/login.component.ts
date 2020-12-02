import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';

import { NzModalService } from 'ng-zorro-antd/modal';
import { NzMessageService } from 'ng-zorro-antd/message';
import { NzNotificationService } from 'ng-zorro-antd/notification';
import { NzUploadFile } from 'ng-zorro-antd/upload';
import { NzFormModule } from 'ng-zorro-antd/form';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { NzIconModule } from 'ng-zorro-antd/icon';

import { AuthService } from '../../../services/passportService';
import { LoginModel } from '../../../viewModels/passport/LoginModel';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.less']
})
export class LoginComponent implements OnInit {
  form: FormGroup;
  error = '';
  type = 0;
  loading = false;
  validateForm!: FormGroup;
  remember:any;

  constructor(public fb: FormBuilder,
    public router: Router,
    public msg: NzMessageService,
    public modalSrv: NzModalService,
    public authSerivce: AuthService) {
    
     }

  submitForm(): void {
    for (const i in this.validateForm.controls) {
      this.validateForm.controls[i].markAsDirty();
      this.validateForm.controls[i].updateValueAndValidity();
    }


    console.log("123");
    var loginModel = new LoginModel(this.userName.value, this.password.value);

    this.authSerivce.login(loginModel).subscribe(      
      (response: any) => { 
        if(response==undefined||response==null)
        this.msg.error("用户名不存在或密码错误！");
        else
        this.router.navigate(['./admin/dashboard']);
       },
    );
  }
 

  ngOnInit(): void {

    // this.form = this.fb.group({
    //   userName: [null, [Validators.required]],
    //   password: [null, Validators.required],
    //   mobile: [null, [Validators.required, Validators.pattern(/^1\d{10}$/)]],
    //   captcha: [null, [Validators.required]],
    //   remember: [true],
    // });
    // this.modalSrv.closeAll();

    this.validateForm = this.fb.group({
      userName: [null, [Validators.required]],
      password: [null, [Validators.required]],
      remember: [true]
    });
  

  }

  get userName() {
    return this.validateForm.controls.userName;
  }
  get password() {
    return this.validateForm.controls.password;
  }
  get mobile() {
    return this.validateForm.controls.mobile;
  }
  get captcha() {
    return this.validateForm.controls.captcha;
  }

  submit() {
    console.log("123");
    var loginModel = new LoginModel(this.userName.value, this.password.value);

    this.authSerivce.login(loginModel).subscribe(      
      (response: any) => { 
        if(response==undefined||response==null)
        this.msg.error("用户名不存在或密码错误！");
        else
        this.router.navigate(['./admin/dashboard']);
       },
    );
  }

  
}
