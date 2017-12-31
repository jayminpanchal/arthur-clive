import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';
import { NgForm } from '@angular/forms';
import { FileHolder, UploadMetadata } from "angular2-image-upload";
import { ToastMsgService } from '../../../shared/services/toastmsg.service';
@Component({
  selector: 'app-product-insert-update',
  templateUrl: './product-insert-update.component.html',
  styleUrls: ['./product-insert-update.component.scss']
})
export class ProductInsertUpdateComponent implements OnInit {

@Input() product: any = { productItem: {}};
@Output() cancelButtonClicked = new EventEmitter<any>();
@Output() saveButtonClicked = new EventEmitter<any>();
  constructor(private toastmsg: ToastMsgService) { }

  ngOnInit() {
  }
  cancelClicked(){
    this.cancelButtonClicked.emit(true);
  }
onSubmit(form: NgForm) {
  this.saveButtonClicked.emit(form.value);
}
onUploadFinished(file: FileHolder) {
  console.log(JSON.parse(file.serverResponse['_body']).data);
  this.product.productItem.minioObject_URL = JSON.parse(file.serverResponse['_body']).data;
}

onBeforeUpload(form: NgForm) {
return (metadata: UploadMetadata) => {
  if(form.value.productFor === undefined || form.value.productType === undefined) {
  this.toastmsg.popToast('error', 'Error', 'Please fill the form before uploading the image')
} else {
  metadata.formData = { 'objectName': form.value.productFor + '-' + form.value.productType + 
                        '-' + form.value.productDesign + '-' + form.value.productColour + '-' +
                        form.value.productSize,
                        'bucketName': 'arthurclive-products' }
  console.log(metadata);
  return metadata;
  }
}
}
}
