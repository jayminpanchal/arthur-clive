import { Component } from '@angular/core';
import { Router } from '@angular/router';

@Component({
    selector: 'paymentcancelled',
    templateUrl: './paymentcancelled.html'
})
export class PaymentCancelledComponent {
constructor(private router: Router) {

}
public viewCart() {
this.router.navigate(['/cart']);
}
public continue() {
this.router.navigate(['/']);
}
}
