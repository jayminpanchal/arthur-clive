import { Component, AfterViewInit } from '@angular/core';
import { Router } from '@angular/router';
import { CartService } from '@services/cart.service';
@Component({
    selector: 'paymentsuccess',
    templateUrl: './paymentsuccess.html'
})
export class PaymentSuccessComponent implements AfterViewInit {
constructor(private router: Router, private cartService: CartService) {
}
public viewOrders() {
this.router.navigate(['/orderlist']);
}
public continue() {
this.router.navigate(['/']);
}
public ngAfterViewInit() {
this.cartService.cartItems.listOfProducts = [];
this.cartService.refreshCart();
}   
}
