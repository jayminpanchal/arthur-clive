import { Component } from '@angular/core';
import { Router } from '@angular/router';

@Component({
    selector: 'welcome',
    styleUrls: ['welcome.component.scss'],
    templateUrl: './welcome.component.html'
})
export class WelcomeComponent {
constructor(private router: Router) {

}
}
