import { Component, OnInit, Input, EventEmitter, Output } from '@angular/core';
import { NgForm } from '@angular/forms';
import { FileHolder, UploadMetadata } from "angular2-image-upload";
import { ToastMsgService } from '../../../shared/services/toastmsg.service';
@Component({
  selector: 'app-coupon-insert-update',
  templateUrl: './coupon-insert-update.component.html',
  styleUrls: ['./coupon-insert-update.component.scss']
})
export class CouponInsertUpdateComponent implements OnInit {

@Input() coupon: any = {};
@Output() cancelButtonClicked = new EventEmitter<any>();
@Output() saveButtonClicked = new EventEmitter<any>();
  constructor(private toastmsg: ToastMsgService) { }

  ngOnInit() {
    console.log(this.coupon);
    console.log(this.coupon.expiryTime.substring(5,7) + '/' + this.coupon.expiryTime.substring(8,10) + '/' + this.coupon.expiryTime.substring(0,4))
  }

cancelClicked(){
  this.cancelButtonClicked.emit(true);
}
onSubmit(form: NgForm) {
  this.saveButtonClicked.emit(form.value);
}
}
