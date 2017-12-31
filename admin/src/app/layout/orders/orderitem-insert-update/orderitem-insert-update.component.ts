import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';
import { NgForm } from '@angular/forms';
import { FileHolder, UploadMetadata } from "angular2-image-upload";
import { ToastMsgService } from '../../../shared/services/toastmsg.service';
import { ApiService } from '../../../shared/services/api.service';

@Component({
  selector: 'app-orderitem-insert-update',
  templateUrl: './orderitem-insert-update.component.html',
  styleUrls: ['./orderitem-insert-update.component.scss']
})
export class OrderItemInsertUpdateComponent implements OnInit {

  @Input() order: any = { orderItem: {} };
  @Output() cancelButtonClicked = new EventEmitter<any>();
  @Output() saveButtonClicked = new EventEmitter<any>();
  public orderStatus: Array<{}>;
  public shippingAddress: any = {};
  public billingAddress: any = {};
  constructor(private toastmsg: ToastMsgService, private apiService: ApiService) { }

  ngOnInit() {
    this.orderStatus = Array<{ id: 0, name: '' }>();
    this.orderStatus.push({ id: 1, name: 'Order Placed' });
    this.orderStatus.push({ id: 2, name: 'Packing In Progress' });
    this.orderStatus.push({ id: 3, name: 'Order Shipped' });
    this.orderStatus.push({ id: 4, name: 'Order Delivered' });
    this.orderStatus.push({ id: 5, name: 'Order Cancelled' });
    this.orderStatus.push({ id: 6, name: 'Order Replacement Initiated' });
    this.orderStatus.push({ id: 7, name: 'Order Replaced' });
    this.orderStatus.push({ id: 8, name: 'Order Refund Initiated' });
    this.orderStatus.push({ id: 9, name: 'Order Refunded' });

    this.shippingAddress = this.order.orderItem.address.reduce(
      (address) => {
        return address.shippingAddress == true;
      }
    );
    this.billingAddress = this.order.orderItem.address.reduce(
      (address) => {
        return address.billingAddress == true;
      }
    );
  }
  cancelClicked() {
    this.cancelButtonClicked.emit(true);
  }
  saveClicked() {
    this.apiService.put('Order/deliverystatus/update/' + this.order.orderItem.orderId + '/' + this.order.orderItem.orderStatus, {},
      { useAuth: false }).then(
      (response: any) => {
        console.log(response);
        if (response.code != undefined) {
          if (response.code === '200') {
            this.toastmsg.popToast('success', 'Success', 'Order Status Updated');
            this.saveButtonClicked.emit(true);
          } else {
            throw response.error;            
          }
        }
        else {
          throw response.error;
        }
      })
      .catch((error: any) => {
        console.log(error);
        this.toastmsg.popToast('error', 'Error', 'Status not Updated. Please contact admin');
        this.saveButtonClicked.emit(true);
      });
  }
}
