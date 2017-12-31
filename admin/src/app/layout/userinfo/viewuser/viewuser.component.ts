import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';
import { NgForm } from '@angular/forms';
import { FileHolder, UploadMetadata } from "angular2-image-upload";
import { ToastMsgService } from '../../../shared/services/toastmsg.service';
import { ApiService } from '../../../shared/services/api.service';

@Component({
  selector: 'app-viewuser',
  templateUrl: './viewuser.component.html',
  styleUrls: ['./viewuser.component.scss']
})
export class ViewUserComponent implements OnInit {

  @Input() user: any = { userInfo: {} };
  @Output() cancelButtonClicked = new EventEmitter<any>();
  @Output() saveButtonClicked = new EventEmitter<any>();
    public shippingAddress: any = {};
  public billingAddress: any = {};
  constructor(private toastmsg: ToastMsgService, private apiService: ApiService) { }

  ngOnInit() {
    this.shippingAddress = this.user.userInfo.shippingAddress;
    this.billingAddress = this.user.userInfo.billingAddress;
  }
  cancelClicked() {
    this.cancelButtonClicked.emit(true);
  }
}
