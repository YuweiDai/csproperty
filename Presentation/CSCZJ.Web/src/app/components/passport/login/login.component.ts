import { Component, OnInit } from '@angular/core';
import { Router, ActivatedRoute, ParamMap } from '@angular/router';

import { NzModalService } from 'ng-zorro-antd/modal';
import { NzMessageService } from 'ng-zorro-antd/message';
import { NzNotificationService } from 'ng-zorro-antd/notification';
import { NzUploadFile } from 'ng-zorro-antd/upload';
import { NzFormModule } from 'ng-zorro-antd/form';
import { FormBuilder, FormControl, FormGroup, ValidationErrors, Validators, ValidatorFn } from '@angular/forms';

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
  //validateForm!: FormGroup;

  // submitForm(): void {
  //   for (const i in this.validateForm.controls) {
  //     this.validateForm.controls[i].markAsDirty();
  //     this.validateForm.controls[i].updateValueAndValidity();
  //   }
  // }
  constructor(public fb: FormBuilder,
    public router: Router,
    public msg: NzMessageService,
    public modalSrv: NzModalService,
    public authSerivce: AuthService) {
    
     }

  ngOnInit(): void {

    this.form = this.fb.group({
      userName: [null, [Validators.required]],
      password: [null, Validators.required],
      mobile: [null, [Validators.required, Validators.pattern(/^1\d{10}$/)]],
      captcha: [null, [Validators.required]],
      remember: [true],
    });
    this.modalSrv.closeAll();

    // this.validateForm = this.fb.group({
    //   userName: [null, [Validators.required]],
    //   password: [null, [Validators.required]],
    //   remember: [true]
    // });

  }

  get userName() {
    return this.form.controls.userName;
  }
  get password() {
    return this.form.controls.password;
  }
  get mobile() {
    return this.form.controls.mobile;
  }
  get captcha() {
    return this.form.controls.captcha;
  }

  submit() {
    console.log("123");
    var loginModel = new LoginModel(this.userName.value, this.password.value);

    this.authSerivce.login(loginModel).subscribe(      
      (response: any) => { 
        if(response==undefined||response==null)
        this.msg.error("用户名不存在或密码错误！");
        else
      this.router.navigate(['/admin/dashboard']);
       },
    );
  }

  
}
