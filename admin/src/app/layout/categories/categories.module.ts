import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { CategoriesComponent } from './categories.component';
import { CategoriesRoutingModule } from './categories-routing.module';
import { PageHeaderModule } from './../../shared';
import { CategoryItemComponent } from './categoryitem/categoryitem.component';
import { FormsModule } from '@angular/forms';
import { CategoryInsertUpdateComponent } from './category-insert-update/category-insert-update.component';
import { ImageUploadModule } from 'angular2-image-upload';

@NgModule({
    imports: [
        CommonModule,
        CategoriesRoutingModule,      
        PageHeaderModule,
        FormsModule,
        ImageUploadModule.forRoot(),
    ],
    declarations: [CategoriesComponent,CategoryItemComponent, CategoryInsertUpdateComponent]
})
export class CategoriesModule {
 }
