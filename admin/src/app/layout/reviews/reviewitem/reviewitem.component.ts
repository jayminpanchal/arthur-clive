import { Component, OnInit, Input, Output, EventEmitter} from '@angular/core';
import { NgForm } from '@angular/forms';
import { ApiService } from '../../../shared/services/api.service';
import { ToastMsgService } from '../../../shared/services/toastmsg.service';

@Component({
  selector: 'app-reviewitem',
  templateUrl: './reviewitem.component.html',
  styleUrls: ['./reviewitem.component.scss']
})
export class ReviewItemComponent implements OnInit {
  @Input() public reviewItem: any = {};
  @Input() public productSKU: any = '';
  @Input() public index: number;
  public rowNumber: number;
  @Output() indexChange = new EventEmitter<number>();

  constructor(private apiService: ApiService, private toastmsg: ToastMsgService) {   
   }
  
  ngOnInit() {
    console.log('called');
    this.rowNumber = this.index;
    this.index++;
    this.indexChange.emit(this.index);
  }
  approveClicked() {
  let updateComment = { id: 0, approved: false};
  updateComment.id = this.reviewItem.id;
  updateComment.approved = true;
  this.updateReview(updateComment);
  }
  revokeClicked() {
  let updateComment = { id: 0, approved: false};
  updateComment.id = this.reviewItem.id;
  this.updateReview(updateComment);
  }
  updateReview(reviewUpdate: any){
      this.apiService.put('Product/updatereview/'+ this.productSKU, reviewUpdate,
                                    { useAuth: false }).then(
                (response: any) => {
                this.reviewItem.approved = reviewUpdate.approved;
                this.toastmsg.popToast('success','Success','Review Updated');
                })
                .catch((error: any) => {
                this.toastmsg.popToast('error', 'Error', 'Review not Updated. Please contact admin');                    
                });
  }
}
