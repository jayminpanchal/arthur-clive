import { Inject, Injectable } from '@angular/core';
import { HttpClient, HttpHeaders,
         HttpParams, HttpRequest, HttpResponse } from '@angular/common/http';
import { Router } from '@angular/router';
import { Observable } from 'rxjs/Rx';
import { apiUrl } from '../config/configuration';
import { TokenService } from './token.service';
import * as Util from '../utils/utils';
import 'rxjs/add/operator/mergeMap';
import 'rxjs/add/operator/toPromise';

@Injectable()
export class ApiService {

    public serverUrl = apiUrl.serverUrl;
    public category: any;
    private http: HttpClient;
    private tokenServ: TokenService;

    constructor( http: HttpClient, private router: Router, private tokenService: TokenService) {
        this.http = http;
        this.tokenServ = tokenService;
    }

public header(token) {
        const headers = new HttpHeaders().set('Content-Type', 'application/vnd.api+json')
        .set('Authorization', 'Bearer ' + token);
        return headers;
    }
public get(url: string, options?: any, serverUrl?: string) {
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
               return this.handleSuccess(res, options);
            })
            .catch((err) => {
                return this.handleError(err, options);
            });
    });
    }
public put(url: string, data: any, options?: any, serverUrl?: string) {
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
               return this.handleSuccess(res, options);
            })
            .catch((err) => {
                return this.handleError(err, options);
            });
    });
    }
public patch(url: string, data: any, options?: any, serverUrl?: string) {
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
               return this.handleSuccess(res, options);
            })
            .catch((err) => {
                return this.handleError(err, options);
            });
    });
    }
public post(url: string, data: any, options?: any, serverUrl?: string) {
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
               return this.handleSuccess(res, options);
            })
            .catch((err) => {
                return this.handleError(err, options);
            });
    });
    }
public delete(url: string, data: any, options?: any, serverUrl?: string) {
        let useAuth = Util.checkOptions(options);
        let body: any = data;
        if (serverUrl === undefined) {
        serverUrl = this.serverUrl;
        }
        return this.tokenService.getAuthToken(useAuth).then(
        (token) => {
        const authHeader = this.header(token);
        return this.http.delete(serverUrl + url, { headers: authHeader }).timeout(30000)
            .toPromise()
            .then((res) => {
               return this.handleSuccess(res, options);
            })
            .catch((err) => {
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
}
