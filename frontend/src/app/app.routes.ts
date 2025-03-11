import { Routes } from '@angular/router';
import { HomeComponent } from './home/home.component';
import { AppComponent } from './app.component';
import { ProductService } from './product.service';
import { ProductsComponent } from './products/products.component';
import { CategoryComponent } from './category/category.component';

export const routes: Routes = [
  { path: "", component: HomeComponent },
  { path: "admin", component: ProductsComponent },
  { path: ':categoryName', component: CategoryComponent } 
];
