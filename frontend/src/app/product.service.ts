import { Injectable } from '@angular/core';
import { Product } from './Model';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { User } from './Model';

@Injectable({
  providedIn: 'root'
})
export class ProductService {

  baseUrl: string = "http://localhost:5248/";

  constructor(private http: HttpClient) { }

  getProducts(): Observable<Product[]> {
    return this.http.get<Product[]>(this.baseUrl + "api/Product/getproducts");
  }

  saveProduct(product: Product): Observable<Product> {
    if (product.id === 0) {
      return this.http.post<Product>(this.baseUrl + "api/Product/addproducts", product);
    } else {
      return this.http.put<Product>(this.baseUrl + `api/Product/putproduct/${product.id}`, product);
    }
  }

  deleteProduct(id: number): Observable<void> {
    return this.http.delete<void>(this.baseUrl + `api/Product/deleteproduct/${id}`);
  }

  getCategoryProduct(categoryName:string):Observable<Product[]>{
    return this.http.get<Product[]>(this.baseUrl+`api/Category/GetProductsCategory/${categoryName}`);
  }

  getProduct(id:number):Observable<Product>{
    return this.http.get<Product>(this.baseUrl+`api/Product/getproduct/${id}`);
  }

  getUsers():Observable<User[]>{
    return this.http.get<User[]>(this.baseUrl+"api/User/getusers");
  }
}
