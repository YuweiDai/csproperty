
import { Injectable } from '@angular/core';
import { HttpInterceptor, HttpHandler, HttpRequest, HttpResponseBase, HttpErrorResponse, HttpEvent } from '@angular/common/http';
import { Observable, of, throwError } from 'rxjs';
import { catchError, filter, mergeMap, switchMap, take, tap } from 'rxjs/operators';
import { AuthService } from '../services/passportService';

const CODEMESSAGE: { [key: number]: string } = {
    200: '服务器成功返回请求的数据。',
    201: '新建或修改数据成功。',
    202: '一个请求已经进入后台排队（异步任务）。',
    204: '删除数据成功。',
    400: '发出的请求有错误，服务器没有进行新建或修改数据的操作。',
    401: '用户没有权限（令牌、用户名、密码错误）。',
    403: '用户得到授权，但是访问是被禁止的。',
    404: '发出的请求针对的是不存在的记录，服务器没有进行操作。',
    406: '请求的格式不可得。',
    410: '请求的资源被永久删除，且不会再得到的。',
    422: '当创建一个对象时，发生一个验证错误。',
    500: '服务器发生错误，请检查服务器。',
    502: '网关错误。',
    503: '服务不可用，服务器暂时过载或维护。',
    504: '网关超时。',
};

@Injectable()
export class AuthInterceptor implements HttpInterceptor {

    constructor(private authService: AuthService) {
    }

    intercept(req: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
        // 统一加上服务端前缀

        // Get the auth token from the service.
        const authToken = this.authService.getAuthorizationToken();
        let authReq = req.clone();
        if (authToken != undefined && authToken != null && authToken != "") {

            // Clone the request and replace the original headers with
            // cloned headers, updated with the authorization.
            authReq = req.clone({
                headers: req.headers.set('Authorization', "Bearer " + authToken)
            });
        }

        return next.handle(authReq).pipe(
            mergeMap((ev) => {
                // 允许统一对请求错误处理
                if (ev instanceof HttpResponseBase) {
                    return this.handleData(ev, authReq, next);
                }
                // 若一切都正常，则后续操作
                return of(ev);
            }),
            catchError((err: HttpErrorResponse) => this.handleData(err, authReq, next)),
        );
    }

    // intercept(req: HttpRequest<any>, next: HttpHandler) {
    //     console.log(req);
    //     // Get the auth token from the service.
    //     const authToken = this.authService.getAuthorizationToken();
    //     let authReq = req.clone();
    //     if (authToken != undefined && authToken != null && authToken != "") {

    //         // Clone the request and replace the original headers with
    //         // cloned headers, updated with the authorization.
    //         authReq = req.clone({
    //             headers: req.headers.set('Authorization', "Bearer " + authToken)
    //         });
    //     }

    //     // send cloned request with header to the next handler.
    //     return next.handle(authReq).pipe(mergeMap((event: any) => {         
    //         // 正常返回，处理具体返回参数
    //         if (event instanceof HttpResponse && event.status === 200)
    //             return this.handleData(event);//具体处理请求返回数据
    //         return of(event);
    //     }),
    //         catchError((err: HttpErrorResponse) => this.handleData(err)))
    // }

    private checkStatus(ev: HttpResponseBase): void {
        if ((ev.status >= 200 && ev.status < 300) || ev.status === 401) {
            return;
        }

        const errortext = CODEMESSAGE[ev.status] || ev.statusText;
        // this.notificationService.errorNotification(`请求错误 ${ev.status}: ${ev.url}`, errortext, 1500);
      //  this.notificationService.errorNotification('请求失败','', 1500);
        console.error(ev);
    }

    private handleData(ev: HttpResponseBase, req: HttpRequest<any>, next: HttpHandler): Observable<any> {
        // 可能会因为 `throw` 导出无法执行 `_HttpClient` 的 `end()` 操作
        // if (ev.status > 0) {
        //     this.http.end();
        // }
        this.checkStatus(ev);
        // 业务处理：一些通用操作
        switch (ev.status) {
            case 200:
                // 业务层级错误处理，以下是假定restful有一套统一输出格式（指不管成功与否都有相应的数据格式）情况下进行处理
                // 例如响应内容：
                //  错误内容：{ status: 1, msg: '非法参数' }
                //  正确内容：{ status: 0, response: {  } }
                // 则以下代码片断可直接适用
                // if (ev instanceof HttpResponse) {
                //   const body = ev.body;
                //   if (body && body.status !== 0) {
                //     this.injector.get(NzMessageService).error(body.msg);
                //     // 继续抛出错误中断后续所有 Pipe、subscribe 操作，因此：
                //     // this.http.get('/').subscribe() 并不会触发
                //     return throwError({});
                //   } else {
                //     // 重新修改 `body` 内容为 `response` 内容，对于绝大多数场景已经无须再关心业务状态码
                //     return of(new HttpResponse(Object.assign(ev, { body: body.response })));
                //     // 或者依然保持完整的格式
                //     return of(ev);
                //   }
                // }
                break;
            case 401:
                // if (this.refreshTokenType === 're-request') {
                //     return this.tryRefreshToken(ev, req, next);
                // }
               // this.navigateService.navToLoginPage();
                break;
            case 403:
            case 404:
            case 500:
                // this.goTo(`/exception/${ev.status}`);
                break;
            default:
                if (ev instanceof HttpErrorResponse) {
                    console.warn(
                        '未可知错误，大部分是由于后端不支持跨域CORS或无效配置引起，请参考 https://ng-alain.com/docs/server 解决跨域问题',
                        ev,
                    );
                }
                break;
        }
        if (ev instanceof HttpErrorResponse) {
            return throwError(ev);
        } else {
            return of(ev);
        }
    }

    // private handleData(
    //     event: HttpResponse<any> | HttpErrorResponse,
    // ): Observable<any> {
    //     // 业务处理：一些通用操作
    //     switch (event.status) {
    //         case 200:
    //             // if (event instanceof HttpResponse) {
    //             //     const body: any = event.body;
    //             //     if (body && body.rc == 3) {
    //             //         this.navigateService.goTo('/test');
    //             //     }
    //             // }
    //             break;
    //         case 401: // 未登录状态码
    //             this.navigateService.navToLoginPage();
    //             break;
    //         case 404:
    //         case 500:
    //             break;
    //         default:
    //             return of(event);
    //     }
    // }
}