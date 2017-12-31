import { Component, OnInit, Input, OnChanges, AfterViewChecked } from '@angular/core';
import { CartService } from '@services/cart.service';
import { AddressService } from '@services/address.service';
import * as Config from '@app/config/configuration';

@Component({
  selector: 'order',
  templateUrl: './order.component.html'
})
export class OrderComponent implements OnInit, OnChanges, AfterViewChecked {

@Input() couponDiscount: any = { value:0, percentage: false};
public couponDiscountAmount: any = 0;
public tax = Config.tax.estimatedTax;
public estimatedTax: number = 0;
public cartTotal: number = 0;
public finalAmount: number = 0;
public deliveryCharges: number = 0;
public runUpdate: boolean = true;
public shippingAddress: any = {};
constructor(private cartService: CartService, private addressService: AddressService) {
    this.updateAddress();
    this.addressService.addressUpdated.subscribe((added) => {
      if (added === true) {
        this.shippingAddress = {};
        this.updateAddress();
      }
    });
}
public getCartTotal() {
      return 
}

public ngOnInit() { 
  this.cartService.cartUpdated.subscribe(updated => {
    this.updateCart();
  });
}
public ngAfterViewChecked() {
  if(this.cartService.cartItems.listOfProducts.length !== 0 && this.runUpdate) {
  this.updateCart();
  this.runUpdate = false;    
  }
}
updateCart(){
this.cartTotal = this.cartService.cartItems.listOfProducts.reduce((acc, item) => {
            return acc + (Number(item.productPrice) * item.productQuantity);
        }, 0);
this.estimatedTax = this.cartService.cartItems.listOfProducts.reduce((acc, item) => {
            if( acc == undefined) {
              acc = 0;
            }
            if(item.productType !== 'Gifts') {
             return acc + ((Number(item.productPrice) * item.productQuantity) * this.tax);
            } else {
              return acc;
            }
        }, 0);
this.finalAmount = this.cartTotal + this.estimatedTax;
if(this.couponDiscount.percentage) {
  this.finalAmount = this.finalAmount  - ((this.couponDiscount.value/100) * this.finalAmount) ;
} else {
  if(this.couponDiscount.value <= this.finalAmount) {
  this.finalAmount = this.finalAmount - this.couponDiscount.value;
  } else {
    this.finalAmount = 0;
    console.log('updatecoupon');
  }
}
 this.finalAmount = this.finalAmount + this.deliveryCharges;
}
public ngOnChanges() {
 this.updateCart();
}
public updateAddress() {
     this.addressService.addressItems.listOfAddress.forEach((add) => {
          if (add.shippingAddress === true) {
            this.shippingAddress = add;
            if(this.shippingAddress.country != 'IN') {
              this.deliveryCharges = 300;
            } else {
              this.deliveryCharges = 0;
            }
          }
        });
this.updateCart();
}
}
