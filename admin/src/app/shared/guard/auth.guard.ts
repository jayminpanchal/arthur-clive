import { Injectable } from '@angular/core';
import { CanActivate } from '@angular/router';
import { Router } from '@angular/router';
import { AppState } from '../services/app.service';

@Injectable()
export class AuthGuard implements CanActivate {

    constructor(private router: Router, private appState: AppState) { }

    canActivate() {
        console.log(this.appState.get('loggedIn'));
        if (this.appState.get('loggedIn') == true) {
            return true;
        }

        this.router.navigate(['/login']);
        return false;
    }
}
