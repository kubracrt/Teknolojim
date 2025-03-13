import { Routes } from '@angular/router';
import { HomeComponent } from './home/home.component';
import { AppComponent } from './app.component';
import { ProductService } from './product.service';
import { ProductsComponent } from './products/products.component';
import { CategoryComponent } from './category/category.component';
import { DetailComponent } from './detail/detail.component';
import { AuthComponent } from './auth/auth.component';

export const routes: Routes = [
  { path: "", component: HomeComponent },
  { path: "admin", component: ProductsComponent },
  { path: "auth", component: AuthComponent },
  { path: ':categoryName', component: CategoryComponent },
  { path: "product/:productId", component: DetailComponent },
];
