import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { routerTransition } from '../router.animations';
import { NgForm } from '@angular/forms';
import { ApiService } from '../shared/services/api.service';
import { apiUrl } from '../shared/config/configuration';
import { LoginLogoutService } from '../shared/services/loginlogout.service';
import { ToastMsgService } from '../shared/services/toastmsg.service';

@Component({
    selector: 'app-login',
    templateUrl: './login.component.html',
    styleUrls: ['./login.component.scss'],
    animations: [routerTransition()]
})
export class LoginComponent implements OnInit {

    constructor(public router: Router, private apiService: ApiService,
                private loginLogout: LoginLogoutService, private toastmsg: ToastMsgService) {
    }

    ngOnInit() {
    }
    onSignin(form: NgForm) {
    const loginDetails = form.value;
    this.apiService.post('/login', loginDetails, undefined, apiUrl.authServer).then(
      (response: any) => {
        if (response.value === undefined) {
          throw JSON.parse(response.error);
        }
        if (response.value.code === '999') {
          let loginModel = { accessToken: response.value.data,
                             firstName: response.value.content.FirstName,
                             userName: response.value.content.UserName};
          this.loginLogout.Login(loginModel);
        }
      })
      .catch(
      (error: any) => {
        if (error.code === '401') {
          this.toastmsg.popToast('error', 'Error', 'Wrong Credentials. Please try again');
        }
        if (error.code === '404') {
          this.toastmsg.popToast('error', 'Error', 'User not Found. Please register to continue.');
        }
      }
    );
 }

    onLoggedin() {
        localStorage.setItem('isLoggedin', 'true');
    }

}
