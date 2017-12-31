import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { ProductsRoutingModule } from './products-routing.module';
import { ProductsComponent } from './products.component';
import { ProductitemComponent } from './productitem/productitem.component';
import { PageHeaderModule } from './../../shared';
import { FormsModule } from '@angular/forms';
import { ProductInsertUpdateComponent } from './product-insert-update/product-insert-update.component';
import { ImageUploadModule } from 'angular2-image-upload';

@NgModule({
  imports: [
    CommonModule,
    ProductsRoutingModule,
    PageHeaderModule,
    ImageUploadModule.forRoot(),
    FormsModule
  ],
  declarations: [ProductsComponent, ProductitemComponent, ProductInsertUpdateComponent]
})
export class ProductsModule { }
