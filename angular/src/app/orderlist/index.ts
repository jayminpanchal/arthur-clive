import { OrderListComponent } from './orderlist.component';
import { OrderItemComponent } from './orderitem/orderitem.component';
import { PrintOrderComponent } from './printorder/printorder.component.ts';
import { ProductDetailsComponent } from './orderitem/productdetails/productdetails.component';

export const OrderListComponentBarrel = [
    OrderListComponent, OrderItemComponent, PrintOrderComponent,
    ProductDetailsComponent];

export { OrderListResolver } from './orderlist.resolver';

