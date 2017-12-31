import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';
import { NgForm } from '@angular/forms';
import { ApiService } from '../../../shared/services/api.service';
import { ToastMsgService } from '../../../shared/services/toastmsg.service';

@Component({
  selector: '[app-productitem]',
  templateUrl: './productitem.component.html',
  styleUrls: ['./productitem.component.scss']
})
export class ProductitemComponent implements OnInit {
  @Input('app-productitem') product: any = {};
  public showUpdate: boolean = false;
  public showAdd: boolean = false;
  @Output() deletedItem = new EventEmitter<any>();
  constructor(private apiService: ApiService, private toastmsg: ToastMsgService) { }

  ngOnInit() {
    console.log(this.product);
  }
  updateClicked() {
    this.showUpdate = true;
  }
  cancelClicked(clicked: any) {
    if (clicked === true) {
      this.showUpdate = false;
    }
  }
  addClicked() {
    this.showAdd = true;
  }
  saveClicked(saveProduct: any) {
    console.log(saveProduct);
    let productSku = this.product.productItem.productFor + '-' + this.product.productItem.productType + '-' +
                     this.product.productItem.productDesign + '-' + this.product.productItem.productColour +
                     '-' + this.product.productItem.productSize;
    this.apiService.put('product/' + productSku, saveProduct,
      { useAuth: false }).then(
      (response: any) => {
        this.showUpdate = false;
        let updateDone = { productItem: {}, itemIndex: {} }
        updateDone.productItem = saveProduct;
        updateDone.itemIndex = this.product.itemIndex;
        this.product = updateDone;
        this.toastmsg.popToast('success', 'Success', 'Product Updated');
      })
      .catch((error: any) => {
        console.log(error);
        this.toastmsg.popToast('error', 'Error', 'Product not Updated. Please contact admin');
      });
  }
  deleteClicked() {
    let productSku = this.product.productItem.productFor + '-' + this.product.productItem.productType + '-' +
                     this.product.productItem.productDesign + '-' + this.product.productItem.productColour +
                     '-' + this.product.productItem.productSize;
    this.apiService.delete('product/' + productSku, {},
      { useAuth: false }).then(
      (response: any) => {
        if (response.code !== undefined) {
          if (response.code === '200') {
            this.deletedItem.emit(this.product.itemIndex);
            this.toastmsg.popToast('success', 'Success', 'Product Deleted');
          }
        }
      })
      .catch((error: any) => {
        console.log(error);
        this.toastmsg.popToast('error', 'Error', 'Product not Deleted. Please contact admin');
      });
  };

}
