import { Component, Input, OnInit } from '@angular/core';
import { CartService } from '@services/cart.service';
import { Router } from '@angular/router';
import { NgForm } from '@angular/forms';
import { ApiService } from '@services/api.service';
import { ToastMsgService } from '@services/toastmsg.service';

@Component({
  selector: 'orderitem',
  styleUrls: ['./orderitem.component.scss'],
  templateUrl: './orderitem.component.html'
})
export class OrderItemComponent implements OnInit {
@Input() public orderItem: any;
public opened: boolean = false;
public paymentStatus: string;
constructor(private route: Router) {
}
public viewOrder() {
  localStorage.setItem('orderItem', JSON.stringify(this.orderItem));
  this.route.navigate(['/printorder', this.orderItem.orderId])
}
public ngOnInit() {
  this.paymentStatus = this.orderItem.paymentDetails['status'][this.orderItem.paymentDetails['status'].length - 1].description;
}
}
