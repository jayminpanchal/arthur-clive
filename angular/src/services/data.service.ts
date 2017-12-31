import { Inject, Injectable, Optional, EventEmitter } from '@angular/core';
import { ApiService } from './api.service';
import { AppState } from '@app/app.service';

@Injectable()
export class DataService {      
    

    public category: any = { data: {}, time: 0};

    constructor(private apiService: ApiService) {
     
    }
    getCategories() {
        let promise = new Promise((resolve, reject) => {
            let timeNow = new Date().getTime();
            if (timeNow < this.category.time) {
                resolve(this.category.data);
            } else {
                this.apiService.get('category', { useAuth: false }).then(
                    (response: any) => {
                        this.category.data = response.data;
                        this.category.time = new Date().getTime() + 5*60000;
                        resolve(this.category.data);
                    })
                    .catch((error: any) => {
                        console.log(error);
                    });
            }            
        });
        return promise;
    }
}
