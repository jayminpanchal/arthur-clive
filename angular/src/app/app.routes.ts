import {Routes} from '@angular/router';
import * as Comp from './route.barrel';

export const ROUTES: Routes = [
    {
        path: '',
        component: Comp.HomeComponent,
        data: {
            meta: {
                title: 'Home Page',
                description: 'Home of AURTHUR CLIVE'
            }
        }
    },
    {
        path: 'home', component: Comp.HomeComponent,
        data: {
            meta: {
                title: 'Home Page',
                description: 'Home of AURTHUR CLIVE'
            }
        }
    },
    {
        path: 'about', component: Comp.AboutComponent,
        data: {
            meta: {
                title: 'About Us',
                description: 'Home of AURTHUR CLIVE'
            }
        }
    },
    {path: 'contact', component: Comp.ContactComponent},
    {path: 'loginregister', component: Comp.LoginRegisterComponent},
    {path: 'createaccount', component: Comp.CreateAccountComponent},
    {path: 'checkemail', component: Comp.CheckEmailComponent},
    {path: 'verifyemail/:userName/:otp/:update', component: Comp.VerifyEmailComponent},
    {path: 'forgotpassword', component: Comp.ForgotPasswordComponent},
    {path: 'changepassword', component: Comp.ChangePasswordComponent},
    {path: 'getemail', component: Comp.GetEmailComponent},
    {path: 'verify/:PhoneNumber/:action', component: Comp.VerificationComponent},
    {path: 'welcome', component: Comp.WelcomeComponent, canActivate: [Comp.AuthGuard]},
    {path: 'updatepassword/:userName', component: Comp.UpdatePasswordComponent},
    {path: 'products/:productFor/:productType', component: Comp.ProductsComponent},
    {
        path: 'products/:productFor/:productType/variants/:productDesign',
        component: Comp.VariantsComponent, resolve: {item: Comp.VariantsResolver}
    },
    {path: 'cart', component: Comp.CartComponent},
    {path: 'checkout', component: Comp.CheckOutComponent, canActivate: [Comp.AuthGuard]},
    {path: 'myaccount', component: Comp.MyAccountComponent, canActivate: [Comp.AuthGuard]},
    {path: 'address', component: Comp.AddressComponent, canActivate: [Comp.AuthGuard]},
    //{ path: 'security', component: Comp.SecurityComponent, canActivate: [Comp.AuthGuard] },
    {
        path: 'orderlist', component: Comp.OrderListComponent, canActivate: [Comp.AuthGuard],
        resolve: {orders: Comp.OrderListResolver}
    },
    {path: 'printorder/:orderId', component: Comp.PrintOrderComponent, canActivate: [Comp.AuthGuard]},
    {path: 'wishlist', component: Comp.WishListComponent},
    {path: 'cancellation', component: Comp.CancellationComponent},
    {path: 'delivery', component: Comp.DeliveryComponent},
    {path: 'disclaimer', component: Comp.DisclaimerComponent},
    {path: 'privacy', component: Comp.PrivacyComponent},
    {path: 'terms', component: Comp.TermsComponent},
    {path: 'addedtocart', component: Comp.AddedToCartComponent},
    {path: 'addedtowishlist', component: Comp.AddedToWishListComponent},
    {path: 'paymentsuccess', component: Comp.PaymentSuccessComponent},
    {path: 'paymenterror', component: Comp.PaymentErrorComponent},
    {path: 'paymentcancelled', component: Comp.PaymentCancelledComponent},
    {path: 'subscribe', component: Comp.SubscribeComponent},
    {path: '**', component: Comp.NoContentComponent},
];
