import { Inject, Injectable } from '@angular/core';
import {
    HttpClient, HttpHeaders,
    HttpParams, HttpRequest, HttpResponse
} from '@angular/common/http';
import { Router } from '@angular/router';
import { Observable } from 'rxjs/Rx';
import { apiUrl } from '@app/config/configuration';
import { TokenService } from '@services/token.service';
import { SpinnerService } from 'angular-spinners';
import * as Util from '@shared/utils/utils';
import 'rxjs/add/operator/mergeMap';
import 'rxjs/add/operator/toPromise';

@Injectable()
export class ApiService {

    public serverUrl = apiUrl.serverUrl;
    public category: any;
    private http: HttpClient;
    private tokenServ: TokenService;

    constructor(http: HttpClient, private router: Router, private tokenService: TokenService,
        private spinnerService: SpinnerService) {
        this.http = http;
        this.tokenServ = tokenService;
    }

    public header(token) {
        const headers = new HttpHeaders().set('Content-Type', 'application/vnd.api+json')
            .set('Authorization', 'Bearer ' + token);
        return headers;
    }
    public get(url: string, options?: any, serverUrl?: string) {
        this.spinnerService.show('acSpinner');
        let useAuth = Util.checkOptions(options);
        if (serverUrl === undefined) {
            serverUrl = this.serverUrl;
        }
        return this.tokenService.getAuthToken(useAuth).then(
            (token) => {
                const authHeader = this.header(token);
                return this.http.get(serverUrl + url, { headers: authHeader }).timeout(30000)
                    .toPromise()
                    .then((res) => {
                        this.spinnerService.hide('acSpinner');
                        return this.handleSuccess(res, options);
                    })
                    .catch((err) => {
                        this.spinnerService.hide('acSpinner');
                        return this.handleError(err, options);
                    });
            });
    }
    public put(url: string, data: any, options?: any, serverUrl?: string) {
        this.spinnerService.show('acSpinner');
        let useAuth = Util.checkOptions(options);
        let body: any = data;
        if (serverUrl === undefined) {
            serverUrl = this.serverUrl;
        }
        return this.tokenService.getAuthToken(useAuth).then(
            (token) => {
                const authHeader = this.header(token);
                return this.http.put(serverUrl + url, body, { headers: authHeader }).timeout(30000)
                    .toPromise()
                    .then((res) => {
                        this.spinnerService.hide('acSpinner');
                        return this.handleSuccess(res, options);
                    })
                    .catch((err) => {
                        this.spinnerService.hide('acSpinner');
                        return this.handleError(err, options);
                    });
            });
    }
    public patch(url: string, data: any, options?: any, serverUrl?: string) {
        this.spinnerService.show('acSpinner');
        let useAuth = Util.checkOptions(options);
        let body: any = data;
        if (serverUrl === undefined) {
            serverUrl = this.serverUrl;
        }
        return this.tokenService.getAuthToken(useAuth).then(
            (token) => {
                const authHeader = this.header(token);
                return this.http.post(serverUrl + url, body, { headers: authHeader }).timeout(30000)
                    .toPromise()
                    .then((res) => {
                        this.spinnerService.hide('acSpinner');
                        return this.handleSuccess(res, options);
                    })
                    .catch((err) => {
                        this.spinnerService.hide('acSpinner');
                        return this.handleError(err, options);
                    });
            });
    }

    public post(url: string, data: any, options?: any, serverUrl?: string) {
        this.spinnerService.show('acSpinner');
        let useAuth = Util.checkOptions(options);
        let body: any = data;
        if (serverUrl === undefined) {
            serverUrl = this.serverUrl;
        }
        return this.tokenService.getAuthToken(useAuth).then(
            (token) => {
                const authHeader = this.header(token);
                return this.http.post(serverUrl + url, body, { headers: authHeader }).timeout(30000)
                    .toPromise()
                    .then((res) => {
                        this.spinnerService.hide('acSpinner');
                        return this.handleSuccess(res, options);
                    })
                    .catch((err) => {
                        this.spinnerService.hide('acSpinner');
                        return this.handleError(err, options);
                    });
            });
    }

    public delete(url: string, data: any, options?: any, serverUrl?: string) {
        this.spinnerService.show('acSpinner');
        let useAuth = Util.checkOptions(options);
        let body: any = data;
        if (serverUrl === undefined) {
            serverUrl = this.serverUrl;
        }
        return this.tokenService.getAuthToken(useAuth).then(
            (token) => {
                const authHeader = this.header(token);
                return this.http.post(serverUrl + url, body, { headers: authHeader }).timeout(30000)
                    .toPromise()
                    .then((res) => {
                        this.spinnerService.hide('acSpinner');
                        return this.handleSuccess(res, options);
                    })
                    .catch((err) => {
                        this.spinnerService.hide('acSpinner');
                        return this.handleError(err, options);
                    });
            });
    }
    private handleSuccess(response: any, options: any) {
        return response;
    }
    private handleError(error: any, options: any) {
        return error;
    }
    public postPayment(url: string, data: any, options?: any, serverUrl?: string) {
        this.spinnerService.show('acSpinner');
        let body: any = data;
        if (serverUrl === undefined) {
            serverUrl = this.serverUrl;
        }
        const paymentHeaders = new HttpHeaders().set('Content-Type', 'application/x-www-form-urlencoded');
        paymentHeaders.set('Access-Control-Allow-Origin', '*');
//        const paymentHeaders = new HttpHeaders();
        console.log(url);
        console.log(serverUrl);
        return this.http.post(serverUrl + url, body, { headers: paymentHeaders }).timeout(30000)
            .toPromise()
            .then((res) => {
                console.log('in api');
                console.log(res);
                this.spinnerService.hide('acSpinner');
                return this.handleSuccess(res, options);
            })
            .catch((err) => {
                console.log(err);
                this.spinnerService.hide('acSpinner');
                return this.handleError(err, options);
            });
    }

    public formpost(path, params, method) {
    method = method || "post"; // Set method to post by default if not specified.

    // The rest of this code assumes you are not using a library.
    // It can be made less wordy if you use one.
    var form = document.createElement("form");
    form.setAttribute("method", method);
    form.setAttribute("action", path);

    for(var key in params) {
        if(params.hasOwnProperty(key)) {
            var hiddenField = document.createElement("input");
            hiddenField.setAttribute("type", "hidden");
            hiddenField.setAttribute("name", key);
            hiddenField.setAttribute("value", params[key]);

            form.appendChild(hiddenField);
        }
    }

    document.body.appendChild(form);
    form.submit();
}
}
