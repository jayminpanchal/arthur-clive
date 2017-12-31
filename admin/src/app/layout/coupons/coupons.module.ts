import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { CouponsComponent } from './coupons.component';
import { CouponsRoutingModule } from './coupons-routing.module';
import { PageHeaderModule } from './../../shared';
import { CouponItemComponent } from './couponitem/couponitem.component';
import { FormsModule } from '@angular/forms';
import { CouponInsertUpdateComponent } from './coupon-insert-update/coupon-insert-update.component';
import { ImageUploadModule } from 'angular2-image-upload';

@NgModule({
    imports: [
        CommonModule,
        CouponsRoutingModule,      
        PageHeaderModule,
        FormsModule,
        ImageUploadModule.forRoot(),
    ],
    declarations: [CouponsComponent,CouponItemComponent, CouponInsertUpdateComponent]
})
export class CouponsModule {
 }
