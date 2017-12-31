import { Component, Input, Output, EventEmitter } from '@angular/core';
import { NgForm } from '@angular/forms';
import { ApiService } from '../../../shared/services/api.service';
import { ToastMsgService } from '../../../shared/services/toastmsg.service';

@Component({
  selector: '[app-couponitem]',
  templateUrl: './couponitem.component.html',
  styleUrls: ['./couponitem.component.scss']
})

export class CouponItemComponent {
public showUpdate: boolean = false;
@Input('app-couponitem') coupon: any = {};
public couponItem: any = {};
@Output() deletedItem = new EventEmitter<any>();
  constructor(private apiService: ApiService, private toastmsg: ToastMsgService) {
   }

  updateClicked() {
    this.showUpdate = true;
  }
  cancelClicked(clicked: any) {
    if(clicked === true) {
    this.showUpdate = false;
    }
  }
  saveClicked(saveCoupon: any) {
    console.log(saveCoupon);
    console.log(this.couponItem);
    this.apiService.put('Coupon/update/'+ this.coupon.couponItem.code, saveCoupon,
                                    { useAuth: false }).then(
                (response: any) => {
                if( response.code != undefined) {
                  if(response.code === '200') {
                  this.showUpdate = false;
                  let updateDone = { couponItem: {}, itemIndex: {}}
                  updateDone.couponItem = saveCoupon;
                  updateDone.itemIndex = this.coupon.itemIndex;
                  this.coupon = updateDone;
                  this.toastmsg.popToast('success','Success','Coupon Updated');
                  }
                } else {
                  throw response.error;
                }
                })
                .catch((error: any) => {
                    console.log(error);
                this.toastmsg.popToast('error', 'Error', 'Coupon not Updated. Please contact admin');                    
                });
    }
  deleteClicked(){
    this.apiService.delete('Coupon/'+ this.coupon.couponItem.code, {},
                                    { useAuth: false }).then(
                (response: any) => {
                  if(response.code !== undefined) {
                  if(response.code === '200') {
                  this.deletedItem.emit(this.coupon.itemIndex);
                  this.toastmsg.popToast('success','Success','Coupon Deleted');
                  }
                  }                
                })
                .catch((error: any) => {
                    console.log(error);
                this.toastmsg.popToast('error', 'Error', 'Coupon not Deleted. Please contact admin');                    
                });
    };
}
