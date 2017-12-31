import { Component, OnInit, OnDestroy } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { ApiService } from '@services/api.service';

@Component({
    selector: 'printorder',
    styleUrls: ['./printorder.component.scss'],
    templateUrl: './printorder.component.html'
})
export class PrintOrderComponent implements OnInit, OnDestroy {

    public orderItem: any = {};
    public shippingAddress: any = {};
    public billingAddress: any = {};
    public orderId: any = '';
    constructor(private route: ActivatedRoute, private apiService: ApiService) {
        this.orderId = route.snapshot.paramMap.get('orderId');
    }
    public ngOnInit() {
        if (localStorage.getItem('orderItem') != null) {
            this.orderItem = JSON.parse(localStorage.getItem('orderItem'));
            if (this.orderId != this.orderItem.orderId) {
                this.getOrder();
            }
            else {
                this.getAddress();
            }
        } else {
            this.getOrder();
        }
    }

    public getOrder() {
        this.apiService.get('Order/viewsingleorder/' + this.orderId, { useAuth: false }).then(
            (response: any) => {
                this.orderItem = response.data;
                this.getAddress();
            })
            .catch((error: any) => {
                console.log(error);
            });
    }
    public getAddress() {
        this.shippingAddress = this.orderItem.address.reduce(
            (address) => {
                return address.shippingAddress == true;
            }
        );
        this.billingAddress = this.orderItem.address.reduce(
            (address) => {
                return address.billingAddress == true;
            }
        );
    }
    public printContent() {
    let printContents, popupWin;
    printContents = document.getElementById('printSection').innerHTML;
    popupWin = window.open('', '_blank', 'top=0,left=0,height=100%,width=auto');
    popupWin.document.open();
    popupWin.document.write(`
      <html>
        <head>
          <title>Print tab</title>
          <link rel="stylesheet" type="text/css" href="./printorder.component.scss" />
        </head>
    <body onload="window.print();window.close()">${printContents}</body>
      </html>`
    );
    popupWin.document.close();
}
public ngOnDestroy(){
localStorage.removeItem('orderItem');
}
}
