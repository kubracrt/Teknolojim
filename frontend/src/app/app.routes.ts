import { Routes } from '@angular/router';
import { HomeComponent } from './home/home.component';
import { AppComponent } from './app.component';
import { ProductService } from './product.service';
import { CategoryComponent } from './category/category.component';
import { DetailComponent } from './detail/detail.component';
import { AuthComponent } from './auth/auth.component';
import { SellerAuthComponent } from './seller-auth/seller-auth.component';
import { SuperAdminDetailComponent } from './super-admin-detail/super-admin-detail.component';
import { AdminDetailComponent } from './admin-detail/admin-detail.component';
import { UserDetailComponent } from './user-detail/user-detail.component';
import { adminGuard } from './guards/admin.guard';

export const routes: Routes = [
  { path: "", component: HomeComponent },
  { path: "auth", component: AuthComponent },
  { path: "sellerAuth", component: SellerAuthComponent },
  { path: ":categoryName", component: CategoryComponent },
  { path: "süperAdmin/:userId", component: SuperAdminDetailComponent },
  { path: "admin/:userId", component: AdminDetailComponent, canActivate:[adminGuard] },
  { path: "user/:userId", component: UserDetailComponent },
  { path: "product/:productId", component: DetailComponent },
];
