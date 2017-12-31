import { NgModule } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Http, HttpModule } from '@angular/http';
import { BrowserModule } from '@angular/platform-browser';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { TranslateLoader, TranslateModule } from '@ngx-translate/core';
import { TranslateHttpLoader } from '@ngx-translate/http-loader';
import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { AuthGuard } from './shared';
// AoT requires an exported function for factories
import { ApiService } from './shared/services/api.service';
import { AppState } from './shared/services/app.service';
import { TokenService } from './shared/services/token.service';
import { ToastMsgService } from './shared/services/toastmsg.service';
import { LoginLogoutService } from './shared/services/loginlogout.service';
import { HttpClientModule } from '@angular/common/http';
import { ToasterModule, ToasterService, ToasterConfig } from 'angular2-toaster';

export function HttpLoaderFactory(http: Http) {
    // for development
    // return new TranslateHttpLoader(http, '/start-angular/SB-Admin-BS4-Angular-4/master/dist/assets/i18n/', '.json');
    return new TranslateHttpLoader(http, '/assets/i18n/', '.json');
}
@NgModule({
    declarations: [
        AppComponent
    ],
    imports: [
        BrowserModule,
        BrowserAnimationsModule,
        FormsModule,
        HttpModule,
        HttpClientModule,
        ToasterModule,
        AppRoutingModule,
        TranslateModule.forRoot({
            loader: {
                provide: TranslateLoader,
                useFactory: HttpLoaderFactory,
                deps: [Http]
            }
        })
    ],
    providers: [AuthGuard,
                AppState,
                ApiService,
                ToastMsgService,
                LoginLogoutService,
                TokenService],
    bootstrap: [AppComponent]
})
export class AppModule {
}
