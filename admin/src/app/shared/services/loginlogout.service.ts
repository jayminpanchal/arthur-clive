import { Injectable } from '@angular/core';
import { Router } from '@angular/router';
import { ToastMsgService } from './toastmsg.service';
import { AppState } from './app.service';
import {Location} from '@angular/common';

declare const FB: any;

@Injectable()
export class LoginLogoutService {
    public fbResponse: any;
    constructor(private toastmsg: ToastMsgService, private appState: AppState,
                private router: Router) {
    }
    public Login(loginModel: any) {
        this.toastmsg.popToast('success', 'Success', 'Welcome!');
        localStorage.setItem('JWT', loginModel.accessToken);
        localStorage.setItem('FirstName', loginModel.firstName);
        localStorage.setItem('UserName', loginModel.userName);
        this.appState.set('loggedIn', true);        
        this.router.navigate(['/dashboard']);
    }
    public Logout() {
                localStorage.removeItem('JWT');
        this.appState.set('loggedIn', false);
        this.toastmsg.popToast('success', 'Success', 'Logged Out');
        this.router.navigate(['/login']);
    }
}
