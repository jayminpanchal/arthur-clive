import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { UserInfoRoutingModule } from './userinfo-routing.module';
import { UserInfoComponent } from './userinfo.component';
import { UserComponent } from './user/user.component';
import { PageHeaderModule } from './../../shared';
import { FormsModule } from '@angular/forms';
import { ViewUserComponent } from './viewuser/viewuser.component';
import { ImageUploadModule } from 'angular2-image-upload';

@NgModule({
  imports: [
    CommonModule,
    UserInfoRoutingModule,
    PageHeaderModule,
    ImageUploadModule.forRoot(),
    FormsModule
  ],
  declarations: [UserInfoComponent, UserComponent, ViewUserComponent]
})
export class UserInfoModule { }
