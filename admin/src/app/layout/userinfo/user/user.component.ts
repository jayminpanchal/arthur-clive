import { Component, OnInit, Input, Output } from '@angular/core';
import { NgForm } from '@angular/forms';
import { ApiService } from '../../../shared/services/api.service';
import { ToastMsgService } from '../../../shared/services/toastmsg.service';

@Component({
  selector: '[app-user]',
  templateUrl: './user.component.html',
  styleUrls: ['./user.component.scss']
})
export class UserComponent implements OnInit {
  @Input('app-user') user: any = {};
  public showUpdate: boolean = false;
  public showAdd: boolean = false;
  constructor(private apiService: ApiService, private toastmsg: ToastMsgService) { }

  ngOnInit() {
    console.log(this.user);
  }
  updateClicked() {
    this.showUpdate = true;
  }
  cancelClicked(clicked: any) {
    if (clicked === true) {
      this.showUpdate = false;
    }
  }
  saveClicked(saveStatus: any) {
    console.log(saveStatus);
    this.showUpdate = false;
  }
}
