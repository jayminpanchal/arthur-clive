import { Component, OnInit } from '@angular/core';
import { routerTransition } from '../../router.animations';
import { ApiService } from '../../shared/services/api.service';
import { ToastMsgService } from '../../shared/services/toastmsg.service';

@Component({
  selector: 'app-orders',
  templateUrl: './orders.component.html',
  styleUrls: ['./orders.component.scss'],
  animations: [routerTransition()]
})
export class OrdersComponent implements OnInit {
  
  public orders: any = [];
  constructor(private apiService: ApiService, private toastmsg: ToastMsgService) { }

  ngOnInit() {
  this.apiService.get('Order/viewallorders').then(
      (response: any) => {
      this.orders = response.data;
      },
      (error: any) => {
      console.log(error);
      }
    );

  }
}
