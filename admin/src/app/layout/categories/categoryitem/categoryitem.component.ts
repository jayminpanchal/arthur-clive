import { Component, Input, Output, EventEmitter } from '@angular/core';
import { NgForm } from '@angular/forms';
import { ApiService } from '../../../shared/services/api.service';
import { ToastMsgService } from '../../../shared/services/toastmsg.service';

@Component({
  selector: '[app-categoryitem]',
  templateUrl: './categoryitem.component.html',
  styleUrls: ['./categoryitem.component.scss']
})

export class CategoryItemComponent {
public showUpdate: boolean = false;
@Input('app-categoryitem') category: any = {};
public categoryItem: any = {};
@Output() deletedItem = new EventEmitter<any>();
  constructor(private apiService: ApiService, private toastmsg: ToastMsgService) {
   }

  updateClicked() {
    this.showUpdate = true;
  }
  cancelClicked(clicked: any) {
    if(clicked === true) {
    this.showUpdate = false;
    }
  }
  saveClicked(saveCategory: any) {
    console.log(saveCategory);
    console.log(this.categoryItem);
    this.apiService.put('Category/'+ this.category.categoryItem.productFor + '/' + this.category.categoryItem.productType, saveCategory,
                                    { useAuth: false }).then(
                (response: any) => {
                this.showUpdate = false;
                let updateDone = { categoryItem: {}, itemIndex: {}}
                updateDone.categoryItem = saveCategory;
                updateDone.itemIndex = this.category.itemIndex;
                this.category = updateDone;
                this.toastmsg.popToast('success','Success','Category Updated');
                })
                .catch((error: any) => {
                    console.log(error);
                this.toastmsg.popToast('error', 'Error', 'Category not Updated. Please contact admin');                    
                });
    }
  deleteClicked(){
    this.apiService.delete('category/'+ this.category.categoryItem.productFor + '/' + this.category.categoryItem.productType, {},
                                    { useAuth: false }).then(
                (response: any) => {
                  if(response.code !== undefined) {
                  if(response.code === '200') {
                  this.deletedItem.emit(this.category.itemIndex);
                  this.toastmsg.popToast('success','Success','Category Deleted');
                  }
                  }                
                })
                .catch((error: any) => {
                    console.log(error);
                this.toastmsg.popToast('error', 'Error', 'Category not Deleted. Please contact admin');                    
                });
    };
}
