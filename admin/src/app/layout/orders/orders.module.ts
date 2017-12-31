import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { OrdersRoutingModule } from './orders-routing.module';
import { OrdersComponent } from './orders.component';
import { OrderItemComponent } from './orderitem/orderitem.component';
import { PageHeaderModule } from './../../shared';
import { FormsModule } from '@angular/forms';
import { OrderItemInsertUpdateComponent } from './orderitem-insert-update/orderitem-insert-update.component';
import { ImageUploadModule } from 'angular2-image-upload';

@NgModule({
  imports: [
    CommonModule,
    OrdersRoutingModule,
    PageHeaderModule,
    ImageUploadModule.forRoot(),
    FormsModule
  ],
  declarations: [OrdersComponent, OrderItemComponent, OrderItemInsertUpdateComponent]
})
export class OrdersModule { }
