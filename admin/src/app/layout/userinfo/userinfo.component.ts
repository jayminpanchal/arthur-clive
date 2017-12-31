import { Component, OnInit } from '@angular/core';
import { routerTransition } from '../../router.animations';
import { ApiService } from '../../shared/services/api.service';
import { ToastMsgService } from '../../shared/services/toastmsg.service';

@Component({
  selector: 'app-userinfo',
  templateUrl: './userinfo.component.html',
  styleUrls: ['./userinfo.component.scss'],
  animations: [routerTransition()]
})
export class UserInfoComponent implements OnInit {
  
  public userInfo: any = [];
  constructor(private apiService: ApiService, private toastmsg: ToastMsgService) { }

  ngOnInit() {
  this.apiService.get('Admin/getallusers').then(
      (response: any) => {
      console.log(response);
      this.userInfo = response.data;
      },
      (error: any) => {
      console.log(error);
      }
    );
  }
}
