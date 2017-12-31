import { Component, OnInit, Input, EventEmitter, Output } from '@angular/core';
import { NgForm } from '@angular/forms';
import { FileHolder, UploadMetadata } from "angular2-image-upload";
import { ToastMsgService } from '../../../shared/services/toastmsg.service';
@Component({
  selector: 'app-category-insert-update',
  templateUrl: './category-insert-update.component.html',
  styleUrls: ['./category-insert-update.component.scss']
})
export class CategoryInsertUpdateComponent implements OnInit {

@Input() category: any = {};
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
  this.category.minioObject_URL = JSON.parse(file.serverResponse['_body']).data;
}

onBeforeUpload(form: NgForm) {
return (metadata: UploadMetadata) => {
  if(form.value.productFor === undefined || form.value.productType === undefined) {
  this.toastmsg.popToast('error', 'Error', 'Please fill the form before uploading the image')
} else {
  metadata.formData = { 'objectName': form.value.productFor + '-' + form.value.productType,
                        'bucketName': 'product-category' }
  console.log(metadata);
  return metadata;
  }
}
} 
}
