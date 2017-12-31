import { Inject, Injectable, Optional, EventEmitter } from '@angular/core';
import { Observable } from 'rxjs/Observable';
import { ApiService } from './api.service';
import { AppState } from '../app/app.service';
import * as WishListModel from '@models/wishlist.model';
import 'rxjs/add/operator/map';

@Injectable()
export class WishListService {

    public wishListItems: WishListModel.WishList = { listOfProducts: [] };
    public wishlistUpdated = new EventEmitter<boolean>();
    constructor(private apiService: ApiService, private appState: AppState) {
    this.wishlistUpdated.subscribe((updated)=> {
        this.refreshList();
    })
    }
    public getCount() {
        return this.wishListItems.listOfProducts.length;
    }

    public getWishListItems(userName: string) {
        console.log('wishlist of ' + userName);
        this.apiService.get('user/wishlist/' + userName, { useAuth: true }).then(
            (response: any) => {
                if ( response.data != null) {
                response.data.forEach((wishListItem) => {
                this.wishListItems.listOfProducts.push(wishListItem);
            });
            }
            })
            .catch((error: any) => {
                console.log(error);
            });
    }
    public refreshList() {
        let userName = localStorage.getItem('UserName');
        if (userName !== undefined) {
        return  this.apiService.post('user/wishlist/' + userName, this.wishListItems ,
                                    { useAuth: true }).then(
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
