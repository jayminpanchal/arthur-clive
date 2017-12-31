import { Component, OnInit } from '@angular/core';
import { routerTransition } from '../../router.animations';
import { ApiService } from '../../shared/services/api.service';
import { ToastMsgService } from '../../shared/services/toastmsg.service';

@Component({
    selector: 'app-categories',
    templateUrl: './categories.component.html',
    styleUrls: ['./categories.component.scss'],
    animations: [routerTransition()]
})
export class CategoriesComponent implements OnInit {
    public categories: any = [];
    public showAdd: boolean = false;
    constructor(private apiService: ApiService, private toastmsg: ToastMsgService) {

    }
    ngOnInit() {
        this.apiService.get('Category').then(
            (response: any) => {
                this.categories = response.data;
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
    saveClicked(saveCategory: any) {
    console.log(saveCategory);
    this.apiService.post('Category', saveCategory,
                                    { useAuth: false }).then(
                (response: any) => {
                this.showAdd = false;
                this.toastmsg.popToast('success','Success','Category Added');
                this.ngOnInit();
                })
                .catch((error: any) => {
                    console.log(error);
                this.toastmsg.popToast('error', 'Error', 'Category not added. Please contact admin');                    
                });
    }
    itemDeleted(item: any) {
    this.ngOnInit();
    }
}
