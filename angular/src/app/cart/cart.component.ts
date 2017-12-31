import { Component, OnInit } from '@angular/core';
import { CartService } from '@services/cart.service';
import { Router } from '@angular/router';
import { NgForm } from '@angular/forms';
import { ApiService } from '@services/api.service';
import { ToastMsgService } from '@services/toastmsg.service';

@Component({
  selector: 'cart',
  templateUrl: './cart.component.html',
  styleUrls: ['./cart.component.scss']
})
export class CartComponent implements OnInit {

public cartItems: any = {};
public couponDiscount: any = { value: 0, percentage: false};

constructor(public cartService: CartService, private route: Router,
            private apiService: ApiService, private toastmsg: ToastMsgService) {
}

public ngOnInit() { 
 this.cartItems = this.cartService.cartItems;
}

public checkOut() {
localStorage.setItem('Coupon', JSON.stringify(this.couponDiscount));
this.route.navigate(['/checkout']);
}
public continueShopping() {
this.route.navigate(['/']);
}
public onSubmit(form: NgForm) {
  console.log(form.value);
  let userName = '';
  if(localStorage.getItem('UserName') != null) {
    userName = localStorage.getItem('UserName');
  }
  this.apiService.get('Coupon/check/'+ userName + '/'+ form.value.coupon).then(
      (response: any) => { 
      if ( response.code != undefined) {
        if( response.code === '200') {
         console.log(response);
         this.couponDiscount = response.data;
         localStorage.setItem('CCode', form.value.coupon);
         this.toastmsg.popToast('success', 'Success', 'Coupon Applied');
        }
        }
         else {
          throw response.error;
        }                 
      },
      (error: any) => {      
      console.log(error);      
      }
    )
    .catch(err => {
     console.log('here');
     this.toastmsg.popToast('error', 'Error', 'Coupon Not Valid');
     this.couponDiscount = { value: 0, percentage: false}
    })
}
}
