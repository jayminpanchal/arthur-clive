import { Component } from '@angular/core';
import { Router } from '@angular/router';

@Component({
    selector: 'paymenterror',
    templateUrl: './paymenterror.html'
})
export class PaymentErrorComponent {
constructor(private router: Router) {

}
public viewCart() {
this.router.navigate(['/cart']);
}
public continue() {
this.router.navigate(['/']);
}
}
