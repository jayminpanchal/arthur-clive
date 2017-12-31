import { Component, Input } from '@angular/core';
import { CartService } from '@services/cart.service';
import { Router } from '@angular/router';
import { NgForm } from '@angular/forms';
import { ApiService } from '@services/api.service';
import { ToastMsgService } from '@services/toastmsg.service';

@Component({
  selector: 'productdetails',
  styleUrls: ['./productdetails.component.scss'],
  templateUrl: './productdetails.component.html'
})
export class ProductDetailsComponent {
@Input() public product: any;
@Input() public orderId: any;
public reviewOpened: boolean = false;
public postReview: any = { id:0, name: '', comment: '', rating: 5, orderId: 0}
constructor(private cartService: CartService, private route: Router,
            private apiService: ApiService, private toastmsg: ToastMsgService) {
}
public buyAgain(product: any) {
  this.cartService.cartItems.listOfProducts.push(product);
  this.route.navigate(['/addedtocart']);
}
public review() {
  this.reviewOpened = true;
}
public cancelReview() {
  this.reviewOpened = false;
}
public starClick(event: any) {
  this.postReview.rating = event.rating;
}
public onSubmit(form: NgForm, index: any) {
  this.postReview.comment = form.value.review;
  this.postReview.name = localStorage.getItem('FirstName');
  this.postReview.orderId = this.orderId;
  this.apiService.post('Product/insertreview/' + form.value.productSKU, this.postReview,
                       { useAuth: true }).then(
                (response: any) => {
                console.log(response);
                this.toastmsg.popToast('success','Success','Review Added. Thank You.');
                this.reviewOpened = false;
                this.product.reviewed = true;
                })
                .catch((error: any) => {
                    console.log(error);
                this.toastmsg.popToast('error', 'Error', 'Review not added. Please try Later');                    
                this.reviewOpened = false;                
                });
}
}
