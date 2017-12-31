import { Component, ViewChild, OnDestroy } from '@angular/core';
import { AddressService } from '@services/address.service';
import { ToastMsgService } from '@services/toastmsg.service';
import { ApiService } from '@services/api.service';
import { PrimaryAddressComponent } from './primaryaddress/primaryaddress.component';
import { OrderComponent } from '@app/cart/order/order.component';
import { CartService } from '@services/cart.service';
import * as Config from '@app/config/configuration';
import * as Util from '@shared/utils/utils.ts';
import { Router } from '@angular/router';

@Component({
  selector: 'checkout',
  templateUrl: './checkout.component.html'
})

export class CheckOutComponent implements OnDestroy {

  public newDeliveryAddress: boolean = true;
  public sameAsDeliveryAddress: boolean = true;
  public couponDiscount: any = { value: 0, percentage: false };
  public couponUsage: number = 0;
  
  @ViewChild(OrderComponent) public orderComponent: OrderComponent;
  
  constructor(private addressService: AddressService, private toastmsg: ToastMsgService,
    private apiService: ApiService, private cartService: CartService, private route: Router) {
    this.addressService.getAddresses();
    if (localStorage.getItem('Coupon') != null) {
      this.couponDiscount = JSON.parse(localStorage.getItem('Coupon'));
    }
  }

  public ngOnDestroy() {
    localStorage.removeItem('Coupon');
    localStorage.removeItem('CCode');
  }

  public isAddressAvailable() {
    return true;
  }
  public check(type: boolean) {
    this.newDeliveryAddress = type;
  }

  public checkBilling(type: boolean) {
    this.sameAsDeliveryAddress = type;
  }

  public addNewAddressClicked(addNew: boolean) {
    this.newDeliveryAddress = addNew;
  }

  public addressAdded(event: boolean) {
    this.newDeliveryAddress = !event;
    this.toastmsg.popToast('success', 'Success', 'Address added.');
  }
  public confirmAndPay() {
    if (this.addressService.addressItems.listOfAddress.length == 0) {
      this.toastmsg.popToast('error', 'Error', 'Please add Delivery and Billing Address.');
    } else {

      let userName = localStorage.getItem('UserName');
      if (userName !== undefined) {
        let postOder = { totalAmount: 0, couponDiscount: 0, estimatedTax: 0 };
        // postOder.totalAmount = this.cartService.cartItems.listOfProducts.reduce((acc, item) => {
        //   return acc + (Number(item.productPrice) * item.productQuantity);
        // }, 0);
        // postOder.estimatedTax = postOder.totalAmount * Config.tax.estimatedTax;
        // postOder.totalAmount = postOder.totalAmount + postOder.estimatedTax;

        postOder.estimatedTax = this.orderComponent.estimatedTax;
        postOder.totalAmount = this.orderComponent.finalAmount;

        let initialTotal = postOder.totalAmount;

        if (this.couponDiscount.percentage) {
          postOder.totalAmount = postOder.totalAmount - ((this.couponDiscount.value / 100) * postOder.totalAmount);
          this.couponUsage = 0;
        } else {
          if (this.couponDiscount.value <= postOder.totalAmount) {
            postOder.totalAmount = postOder.totalAmount - this.couponDiscount.value;
            this.couponUsage = this.couponDiscount.value;
          } else {
            postOder.totalAmount = 0;
            this.couponUsage = this.couponDiscount.value - postOder.totalAmount;
          }
        }
        postOder.couponDiscount = initialTotal - postOder.totalAmount;

        let couponUsageCount = { usageCount: 1, amount: postOder.couponDiscount }

        if (this.couponDiscount.value == 0) {
          this.postCart(postOder);
        } else {
          this.apiService.put('Coupon/' + localStorage.getItem('CCode'), couponUsageCount,
            { useAuth: true }, undefined).then(
            (response: any) => {
              console.log(response);
              if (response.code === '200') {
                this.postCart(postOder);
              } else {
                throw response.error;
              }
            })
            .catch(
            (error: any) => {
              console.log(error);
            }
            );
        }
      }

    }
  }
  postCart(postOrder: any) {
    let userName = localStorage.getItem('UserName');
    if (userName !== undefined) {
      this.apiService.post('Order/placeorder/' + userName,
        postOrder, { useAuth: true }, undefined).then(
        (response: any) => {
          console.log(response);
          if (response.code === '200') {
            let postData = Util.xwwwfurlenc(response.data);
            this.apiService.formpost(Config.apiUrl.paymentUrl, response.data, 'post');
            return true;
          } else if (response.code === '201') {
            this.route.navigate(['/paymentsuccess']);
          }
          else {
            throw response.error;
          }
        })
        .catch(
        (error: any) => {
          console.log(error);
          if (error.code === '403') {
            this.toastmsg.popToast('error', 'Error', 'Available stock is less than selected Quantity. Please reduce quantity and try again.');
            this.route.navigate(['/cart']);
          }
        }
        );
    }
  }
}
