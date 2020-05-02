import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class ConfigService {

  private authKey: string;
  private apiUrl: string;


  constructor() {
    this.authKey = "authroziationData";
    this.apiUrl = "http://localhost:8844/api/"
  }

  getApiUrl(): string {
    return this.apiUrl;
  }

  getAuthKey(): string {
    return this.authKey;
  }

  getClinetId(): string {
    return "";
  }
}
