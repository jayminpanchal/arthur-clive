import { Component } from '@angular/core';
import { Router } from '@angular/router';

@Component({
    selector: 'subscribe',
    templateUrl: './subscribe.html'
})
export class SubscribeComponent {
constructor(private router: Router) {

}
public continueShopping() {
this.router.navigate(['/']);
}
}
