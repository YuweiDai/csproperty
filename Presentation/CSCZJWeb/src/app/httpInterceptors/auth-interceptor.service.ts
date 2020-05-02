import { Injectable } from '@angular/core';
import { HttpInterceptor, HttpRequest, HttpHandler, HttpEvent, HttpResponse, HttpHeaders } from '@angular/common/http';
import { Observable } from "rxjs";

import { LocalStorageService, SessionStorageService } from 'ngx-webstorage';
import { ConfigService } from '../services/config.service';
import { tap, finalize } from 'rxjs/operators';


@Injectable({
  providedIn: 'root'
})
export class AuthInterceptorService implements HttpInterceptor {

  constructor(private localSt: LocalStorageService,
    private configService: ConfigService) { }

  intercept(req: HttpRequest<any>, next: HttpHandler) {



    const started = Date.now();
    let ok: string;

    let url = req.url;

    // 判断是否为登录请求
    if (req.url != this.configService.getApiUrl() + "token") {
      // Get the auth token from the service.
      const authData = this.localSt.retrieve(this.configService.getAuthKey());
      if (authData != null) {

        var headers = new HttpHeaders({
          'Content-Type': 'application/json',
          'Authorization': 'my-auth-token'
        });
        // req.headers.set("Authorization", 'Bearer ' + authData.token);
        // headers.set('Authorization', 'Bearer ' + authData.token);

        const authReq = req.clone({
          headers: req.headers.set('Authorization', 'Bearer ' + authData.token)
        });
        // send the cloned, "secure" request to the next handler.
        return next.handle(authReq);
      }
    }

    return next.handle(req)
      .pipe(
        tap(
          // Succeeds when there is a response; ignore other events
          event => {
            ok = event instanceof HttpResponse ? 'succeeded' : ''
            console.log(event)
          },
          // Operation failed; error is an HttpErrorResponse
          error => {
            ok = 'failed';
            console.log(error);
          }
        ),
        // Log when response observable either completes or errors
        finalize(() => {
          const elapsed = Date.now() - started;
          console.log("http intercepotors：" + elapsed);
          // const msg = `${req.method} "${req.urlWithParams}"
          //    ${ok} in ${elapsed} ms.`;
        })
      );
  }
}
