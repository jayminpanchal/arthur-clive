import { Component, OnInit } from '@angular/core';
import { routerTransition } from '../../router.animations';
import { ApiService } from '../../shared/services/api.service';
import { ToastMsgService } from '../../shared/services/toastmsg.service';

@Component({
    selector: 'app-coupons',
    templateUrl: './coupons.component.html',
    styleUrls: ['./coupons.component.scss'],
    animations: [routerTransition()]
})
export class CouponsComponent implements OnInit {
    public coupons: any = [];
    public showAdd: boolean = false;
    constructor(private apiService: ApiService, private toastmsg: ToastMsgService) {

    }
    ngOnInit() {
        this.apiService.get('Coupon').then(
            (response: any) => {
                this.coupons = response.data;
            },
            (error: any) => {
                console.log(error);
            }
        );
    }
    cancelClicked(clicked: any) {
        this.showAdd = false;
    }
    addClicked() {
        this.showAdd = true;
    }
    saveClicked(saveCoupon: any) {
    console.log(saveCoupon);
    this.apiService.post('Coupon', saveCoupon,
                                    { useAuth: false }).then(
                (response: any) => {
                console.log(response);
                this.showAdd = false;
                if(response.code != undefined){
                    if(response.code === '200') {
                     this.toastmsg.popToast('success','Success','Coupon Added');
                     this.ngOnInit();
                    }
                } else {
                    throw JSON.parse(response.error);
                }
                })
                .catch((error: any) => {
                console.log(error);
                if(error.code === '401') {
                this.toastmsg.popToast('error','error','Coupon Already Exists');                    
                } else {
                this.toastmsg.popToast('error', 'Error', 'Coupon not added. Please contact admin');                    
                }
                });
    }
    itemDeleted(item: any) {
    this.ngOnInit();
    }
}
