import { Routes } from '@angular/router';
import { HomeComponent } from './home/home.component';
import { AppComponent } from './app.component';
import { ProductService } from './product.service';
import { ProductsComponent } from './products/products.component';
import { CategoryComponent } from './category/category.component';
import { DetailComponent } from './detail/detail.component';
import { AuthComponent } from './auth/auth.component';
import { SellerAuthComponent } from './seller-auth/seller-auth.component';
import { SuperAdminDetailComponent } from './super-admin-detail/super-admin-detail.component';
import { AdminDetailComponent } from './admin-detail/admin-detail.component';
import { UserDetailComponent } from './user-detail/user-detail.component';

export const routes: Routes = [
  { path: "", component: HomeComponent },
  { path: "admin", component: ProductsComponent },
  { path: "auth", component: AuthComponent },
  { path: "sellerAuth", component: SellerAuthComponent },
  { path: ":categoryName", component: CategoryComponent },
  { path: "süperAdmin/:userId", component: SuperAdminDetailComponent },
  { path: "admin/:userId", component: AdminDetailComponent },
  { path: "user/:userId", component: UserDetailComponent },
  { path: "product/:productId", component: DetailComponent },
];
