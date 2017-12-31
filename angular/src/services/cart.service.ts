import { Inject, Injectable, Optional, EventEmitter } from '@angular/core';
import { Observable } from 'rxjs/Observable';
import { ApiService } from './api.service';
import { AppState } from '@app/app.service';
import * as CartModel from '@models/cart.model';
import 'rxjs/add/operator/map';

@Injectable()
export class CartService {

    public cartItems: CartModel.Cart = { listOfProducts: [] };
    public cartUpdated = new EventEmitter<boolean>();

    constructor(private apiService: ApiService, private appState: AppState) {
    this.cartUpdated.subscribe((updated)=> {
       this.refreshCart(); 
    })
    }
    public getCount() {
        return this.cartItems.listOfProducts.length;
    }

    public getCartItems(userName: string) {
        this.apiService.get('user/cart/' + userName, { useAuth: true }).then(
            (response: any) => {
                if ( response.data != null) {
                response.data.forEach((cartitem) => {
                this.cartItems.listOfProducts.push(cartitem);
            });
            }
            })
            .catch((error: any) => {
                console.log(error);
            });
    }
    public refreshCart() {
        let userName = localStorage.getItem('UserName');
        if (userName !== undefined) {
         return   this.apiService.post('user/cart/' + userName,
                                      this.cartItems , { useAuth: true }).then(
                (response: any) => {
                    return true;
                })
                .catch((error: any) => {
                    console.log(error);
                    return false;
                });
        }
    }
}
