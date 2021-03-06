import { Injectable } from '@angular/core';
import { CanActivate } from '@angular/router';

import { HttpClient, HttpHeaders, HttpParams } from '@angular/common/http';
import { Observable,of } from 'rxjs';
import { catchError, map, tap } from 'rxjs/operators';


import { LocalStorageService } from 'ngx-webstorage';

import { ConfigService } from "./configService";
import { LogService } from "./logService";

import { AuthenticationModel } from "../viewModels/passport/AuthenticationModel";
import { LoginModel, LoginResult } from "../viewModels/passport/LoginModel";
import { ResponseModel } from "../viewModels/Response/ResponseModel";

@Injectable()
export class AuthInterceptorService {
    private apiUrl = "";
    private httpOptions = {
        headers: new HttpHeaders({ 'Content-Type': 'application/json' })
    };

    constructor(private http: HttpClient,
        private logService: LogService,
        private configService: ConfigService) {
        this.apiUrl += configService.getApiUrl() + "properties";
    }

    request(): void {
        // config.headers = config.headers || {};

        // var authData = $localStorage.authorizationData;
        // //var authData = $localStorage.get('authorizationData');
        // if (authData) {
        //     config.headers.Authorization = 'Bearer ' + authData.token;
        // }

        // return config;
    }

    responseError(): void {
        // if (rejection.status === 401) {
        //     var authService = $injector.get('authService');
        //     var $state = $injector.get('$state');

        //     var authData = $localStorage.authorizationData;
        //     //var authData = $localStorage.get('authorizationData');

        //     if (authData) {
        //         if (authData.useRefreshTokens) {
        //             $location.path('/refresh');   //ps:这里应该重新发送请求去获取token
        //             return $q.reject(rejection);
        //         }
        //     }
        //     authService.logOut();
        //     //$location.path('/login');
        //     $state.go("access.signin");
        // }
        // return $q.reject(rejection);
    }
}

@Injectable()
export class AuthService {
    private apiUrl = "";
    private authentication= {
        isAuth: false,
        account: "",
        nickName:"",
        useRefreshTokens: false,
        roles: "",
        isAdmin: true
    };
    private isLoggerIn = false;
    redirectUrl: string;

    constructor(private http: HttpClient,
        private localStorageService: LocalStorageService,
        private logService: LogService,
        private configService: ConfigService) {
        this.apiUrl += configService.getApiUrl();
    }

    //登陆
    login(loginModel: LoginModel): Observable<LoginResult> {
        var that = this;
        var url = that.apiUrl + "token";
        var data = "grant_type=password&userName=" + loginModel.account + "&password=" + loginModel.password;
        return that.http.post(url, data, { headers: { 'Content-Type': 'application/x-www-form-urlencoded' } }).pipe(
            tap((response: LoginResult) => {

                //是否为管理员登录
                if (loginModel.isAdmin && !(that.userHasRole(response.userRoles, "管理员") || that.userHasRole(response.userRoles, "注册单位"))) {
                    that.logout();                
                }
                else {
                    var authroziationData = { token: response.access_token, userName: loginModel.account, nickName: response.nickName, refreshToken: "", useRefreshTokens: false, roles: response.userRoles, isAdmin: that.userHasRole(response.userRoles, "管理员") };

                    if (loginModel.useRefreshTokens) {
                        authroziationData.useRefreshTokens = true;
                        authroziationData.refreshToken = response.refresh_token;
                    }

                    //that.localStorageService.store("authroziationData", authroziationData);

                    that.authentication.isAuth = true;
                    that.authentication.account = loginModel.account;
                    that.authentication.nickName = response.nickName;
                    that.authentication.useRefreshTokens = loginModel.useRefreshTokens;
                    that.authentication.roles = response.userRoles;
                    that.authentication.isAdmin = that.userHasRole(response.userRoles, "管理员");

                    that.localStorageService.store(that.configService.getAuthKey(), authroziationData);

                   // console.log(that.localStorageService.retrieve("authorizationData"));
                    this.log(`getRefreshTokens`);
                }
            }),
            catchError(this.handleError<any>(`getTokens`))
        )
    }

    logout(): void {
        this.localStorageService.store("authroziationData", null);

        this.authentication.isAuth = false;
        this.authentication.account = "";
        this.authentication.nickName = "";
        this.authentication.useRefreshTokens = false;
    }

    getAuthorizationToken(): string {
        var authData = this.localStorageService.retrieve(this.configService.getAuthKey()) as AuthenticationModel;
        if (authData != undefined && authData != null) return authData.token;
        else "";
    }

    isAdmin(): boolean {
        return this.userHasRole(this.authentication.roles, "管理员");
    }

    isLoggedIn():boolean
    {
        return this.authentication.isAuth;
    }

    fillAuthData(): void {
        var authData = this.localStorageService.retrieve("authroziationData");
        if (authData) {
            this.authentication.isAuth = true;
            this.authentication.account = authData.userName;
            this.authentication.nickName = authData.nickName;
            this.authentication.roles = authData.roles;
            this.authentication.useRefreshTokens = authData.useRefreshTokens;
            this.authentication.isAdmin = authData.isAdmin;
        }
    }

    refreshToken(): void {
        var that=this;
        var authData = this.localStorageService.retrieve("authroziationData");
        var url = that.apiUrl + "token";
        if (authData) {

            if (authData.useRefreshTokens) {

                var data = "grant_type=refresh_token&refresh_token=" + authData.refreshToken + "&client_id=" + this.configService.getClinetId();

                this.localStorageService.store("authroziationData", null);

                this.http.post(url, data, { headers: { 'Content-Type': 'application/x-www-form-urlencoded' } }).pipe(
                    tap((response: LoginResult) => {
                        var authorizationData = { token: response.access_token, userName: response.userName,nickName:response.nickName, 
                            refreshToken: response.refresh_token, useRefreshTokens: true, isAdmin: that.userHasRole(response.userRoles, "管理员")  };

                            that.localStorageService.store("authorizationData",authorizationData);
                    }),
                   // catchError(this.handleError<any>(`getRefreshTokens`))
                );
            }
        }
    }

    

    private userHasRole(roles: string, targetRole: string): boolean {
        return roles.indexOf(targetRole) > -1;
    }

    /**
     * Handle Http operation that failed.
     * Let the app continue.
     * @param operation - name of the operation that failed
     * @param result - optional value to return as the observable result
     */
    private handleError<T>(operation = 'operation', result?: T) {
        return (error: any): Observable<T> => {

            // TODO: send the error to remote logging infrastructure
            console.error(error); // log to console instead

            // TODO: better job of transforming error for user consumption
            this.log(`${operation} failed: ${error.message}`);

            // var response=new ResponseModel();
            // response.Code=-2;
            // response.Message="error";
            // response.Data=null;

            // Let the app keep running by returning an empty result.
            return of(result as T);
        };
    }

    /** Log a PropertyService message with the MessageService */
    private log(message: string) {
        this.logService.add('PropertyService: ' + message);
    }
}

@Injectable()
export class TokensManagerService {
    private apiUrl = "";

    constructor(private http: HttpClient,
        private logService: LogService,
        private configService: ConfigService) {
        this.apiUrl += configService.getApiUrl();
    }


    getRefreshTokens(): Observable<any> {
        const url = this.apiUrl + "/refreshtokens";

        return this.http.get<any>(url).pipe(
            tap(_ => this.log(`getRefreshTokens`)),
            catchError(this.handleError<any>(`getRefreshTokens`))
        );
    }


    deleteRefreshTokens(tokenid: string): Observable<any> {
        const url = this.apiUrl + "/refreshtokens/?" + tokenid;

        return this.http.delete<any>(url).pipe(
            tap(_ => this.log(`deleteRefreshTokens`)),
            catchError(this.handleError<any>(`deleteRefreshTokens`))
        );
    }

    /**
     * Handle Http operation that failed.
     * Let the app continue.
     * @param operation - name of the operation that failed
     * @param result - optional value to return as the observable result
     */
    private handleError<T>(operation = 'operation', result?: T) {
        return (error: any): Observable<T> => {

            // TODO: send the error to remote logging infrastructure
            console.error(error); // log to console instead

            // TODO: better job of transforming error for user consumption
            this.log(`${operation} failed: ${error.message}`);

            // Let the app keep running by returning an empty result.
            return of(result as T);
        };
    }

    /** Log a PropertyService message with the MessageService */
    private log(message: string) {
        this.logService.add('PropertyService: ' + message);
    }
}