import { Component, OnInit } from '@angular/core';
import { routerTransition } from '../../router.animations';
import { ApiService } from '../../shared/services/api.service';
import { ToastMsgService } from '../../shared/services/toastmsg.service';

@Component({
  selector: 'app-products',
  templateUrl: './products.component.html',
  styleUrls: ['./products.component.scss'],
  animations: [routerTransition()]
})
export class ProductsComponent implements OnInit {
  
  public products: any = [];
  public showAdd: boolean = false;
  constructor(private apiService: ApiService, private toastmsg: ToastMsgService) { }

  ngOnInit() {
  this.apiService.get('Product').then(
      (response: any) => {
      this.products = response.data;
      },
      (error: any) => {
      console.log(error);
      }
    );

  }
  addClicked(){
  this.showAdd = true;
  }
  cancelClicked(clicked: any) {
    if(clicked === true) {
    this.showAdd = false;
    }
  }
  saveClicked(saveProduct: any) {
    console.log(saveProduct);
    this.apiService.post('product', saveProduct,
                                    { useAuth: false }).then(
                (response: any) => {
                this.showAdd = false;
                this.toastmsg.popToast('success','Success','Product Added');
                this.ngOnInit();
                })
                .catch((error: any) => {
                    console.log(error);
                this.toastmsg.popToast('error', 'Error', 'Product not added. Please contact admin');                    
                });
    }
  itemDeleted(item: any) {
    this.ngOnInit();
    }
}
