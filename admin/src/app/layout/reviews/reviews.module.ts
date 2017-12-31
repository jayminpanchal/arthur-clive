import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReviewsRoutingModule } from './reviews-routing.module';
import { ReviewsComponent } from './reviews.component';
import { ReviewItemComponent } from './reviewitem/reviewitem.component';
import { PageHeaderModule } from './../../shared';
import { FormsModule } from '@angular/forms';
import { ImageUploadModule } from 'angular2-image-upload';

@NgModule({
  imports: [
    CommonModule,
    ReviewsRoutingModule,
    PageHeaderModule,
    ImageUploadModule.forRoot(),
    FormsModule
  ],
  declarations: [ReviewsComponent, ReviewItemComponent]
})
export class ReviewsModule { }
