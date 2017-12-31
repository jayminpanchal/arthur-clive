import { Component, OnInit } from '@angular/core';
import { WishListItemComponent } from 'wishlistitem/wishlistitem.component';
import { WishListService } from '../../services/wishlist.service';
import { Router } from '@angular/router';

@Component({
  selector: 'wishlist',
  templateUrl: './wishlist.component.html',
  styleUrls: ['./wishlist.component.scss']
})
export class WishListComponent implements OnInit {

public wishListItems: any;

constructor(public wishListService: WishListService, private route: Router) {

}

public ngOnInit() {
  this.wishListItems = this.wishListService.wishListItems.listOfProducts;
}

public ngOnDestroy() {
}
public continueShopping() {
  this.route.navigate(['/']);
}
}
