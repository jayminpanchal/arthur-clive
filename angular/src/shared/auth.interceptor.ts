import { HttpEvent, HttpHandler, HttpInterceptor, HttpRequest, HttpResponse } from '@angular/common/http';
import { Router } from '@angular/router';
import { Observable } from 'rxjs/Observable';
import { Injectable, Injector } from '@angular/core';
import { Http, Headers } from '@angular/http';
import { TokenService } from '@services/token.service';
import * as Util from '@shared/utils/utils';
import 'rxjs/add/operator/do';

@Injectable()
export class AuthInterceptor implements HttpInterceptor {
    constructor(private inj: Injector) {

    }
public intercept(req: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
        return next.handle(req);
        // .do(event => {
        // console.log(event);
        // if(event instanceof HttpResponse) {
        // console.log(event.url);
        // // if(event.url.includes('https://test.payu.in/_payment_options'))
        // // {
        // //   console.log("intercepted");
        // //   window.location.href = event.url;
        // // }
        // }
        // });
     }
}
