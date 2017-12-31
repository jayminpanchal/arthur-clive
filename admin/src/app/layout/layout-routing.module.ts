import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { LayoutComponent } from './layout.component';

const routes: Routes = [
    {
        path: '', component: LayoutComponent,
        children: [
            { path: 'dashboard', loadChildren: './dashboard/dashboard.module#DashboardModule' },
            { path: 'charts', loadChildren: './charts/charts.module#ChartsModule' },
            { path: 'categories', loadChildren: './categories/categories.module#CategoriesModule' },
            { path: 'products', loadChildren: './products/products.module#ProductsModule' },
            { path: 'orders', loadChildren: './orders/orders.module#OrdersModule' },
            { path: 'userinfo', loadChildren: './userinfo/userinfo.module#UserInfoModule' },
            { path: 'reviews', loadChildren: './reviews/reviews.module#ReviewsModule' },
            { path: 'coupons', loadChildren: './coupons/coupons.module#CouponsModule' },
            { path: 'blank-page', loadChildren: './blank-page/blank-page.module#BlankPageModule' },
        ]
    }
];

@NgModule({
    imports: [RouterModule.forChild(routes)],
    exports: [RouterModule]
})
export class LayoutRoutingModule { }
