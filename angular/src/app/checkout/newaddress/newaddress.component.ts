import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';
import { NgForm } from '@angular/forms';
import { AddressService } from '@services/address.service';
import { ToastMsgService } from '@services/toastmsg.service';

@Component({
  selector: 'newaddress',
  styleUrls: ['./newaddress.component.scss'],
  templateUrl: './newaddress.component.html'
})
export class NewAddressComponent implements OnInit {

  public userLocation: string;
  public dialCode: string;
  @Input() public defaultAddress: boolean;
  @Input() public shippingAddress: boolean;
  @Input() public billingAddress: boolean;
  @Output() public addressAdded: EventEmitter<boolean> = new EventEmitter();
  constructor(private addressService: AddressService, private toastmsg: ToastMsgService) {

  }
  public ngOnInit(){
  this.userLocation = localStorage.getItem('Country');
  this.dialCode = localStorage.getItem('IsdCode');
  }

  public onSubmit(form: NgForm) {
    console.log(form.value);
    if (form.value.billingAddress === true) {
      this.addressService.addressItems.listOfAddress.forEach((add) => {
        add.billingAddress = false;
      });
    }
    this.addressService.addressItems.listOfAddress.forEach((add) => {
      add.shippingAddress = false;
    });
    this.addressService.addressItems.listOfAddress.push(form.value);
    this.addressService.refreshAddressList();
    this.addressAdded.emit(true);
  }
}
