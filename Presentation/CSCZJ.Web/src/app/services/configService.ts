import { Injectable } from '@angular/core';

@Injectable()
export class ConfigService{
    private authKey: string;
    private apiUrl: string;
    private ddAPPId: string;
    private redirectUrl: string;
    public showRentCard:boolean;

    constructor(){
        this.authKey = "authroziationData";
        this.apiUrl = "/api/";
        this.ddAPPId = "dingoayqnfibggoazv13rm";
        this.showRentCard=true;
       // this.redirectUrl = "http://localhost:4200/passport/binding";
        //this.redirectUrl="http://localhost:8084/login"
    }

    getShowCard():boolean{
      return this.showRentCard;
    }

    getApiUrl(): string {
        return this.apiUrl;
      }
    
      getAuthKey(): string {
        return this.authKey;
      }
    
      getDDAPPId(): string {
        return this.ddAPPId;
      }
    
      getClinetId(): string {
        return "";
      }
    
      getDDLoginGotoUrl(): string {
        var url = "https://oapi.dingtalk.com/connect/oauth2/sns_authorize?appid={{appid}}&response_type=code&scope=snsapi_login&state=STATE&redirect_uri={{redirect_uri}}";
        return encodeURIComponent(url.replace("{{appid}}", this.ddAPPId).replace("{{redirect_uri}}", this.redirectUrl));
      }
    
      getRedirectUrl(loginTmpCode: string): string {
        var url = "https://oapi.dingtalk.com/connect/oauth2/sns_authorize?appid={{appid}}&response_type=code&scope=snsapi_login&state=STATE&redirect_uri={{redirect_uri}}&loginTmpCode="
          + loginTmpCode;
        url = url.replace("{{appid}}", this.ddAPPId).replace("{{redirect_uri}}", this.redirectUrl);
        return url;
      }
    
      //生成随机数
      generateUUID(length): string {
        return Number(Math.random().toString().substring(3, length) + Date.now()).toString(36);
      }
}