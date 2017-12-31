import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { routerTransition } from '../../router.animations';
import { ApiService } from '../../shared/services/api.service';
import { ToastMsgService } from '../../shared/services/toastmsg.service';

@Component({
  selector: 'app-reviews',
  templateUrl: './reviews.component.html',
  styleUrls: ['./reviews.component.scss'],
  animations: [routerTransition()]
})
export class ReviewsComponent implements OnInit {
  
  public reviews: any = [];
  public rowIndex: number = 1;
  constructor(private apiService: ApiService, private toastmsg: ToastMsgService,
              private cdr: ChangeDetectorRef) { }

  ngOnInit() {
  this.apiService.get('Product/getallreviews').then(
      (response: any) => {
      console.log(response);
      this.reviews = response.data;
      },
      (error: any) => {
      console.log(error);
      }
    );
  }
}
