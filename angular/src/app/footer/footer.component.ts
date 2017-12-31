import {
  Component
} from '@angular/core';
import { NgForm } from '@angular/forms';
import { ApiService } from '@services/api.service';
import { Router } from '@angular/router';
import { ToastMsgService } from '@services/toastmsg.service';

@Component({
  selector: 'footer',  // <footer></footer>

  styleUrls: ['./footer.component.css'],

  templateUrl: './footer.component.html'
})
export class FooterComponent {

  constructor(private apiService: ApiService, private route: Router,
              private toastmsg: ToastMsgService) {

  }
  public onSubscribe(form: NgForm) {
    console.log(form.value);
    let body = null;
    this.apiService.post('admin/subscribe/' + form.value.Email,
      body, { useAuth: false }, undefined).then(
      (response: any) => {
        console.log(response);
        if(response.code !== undefined) {
        if (response.code === '200') {
        form.resetForm();
        this.route.navigate(['/subscribe'])
      }
      } else {
       throw response.error;
      }
      })
      .catch(
      (error: any) => {
          console.log(error);
          if(error.code = 401) {
          form.resetForm();
          this.toastmsg.popToast('info', 'Info', 'You are already Subscribed');
          }
      }
      );
  }
}
