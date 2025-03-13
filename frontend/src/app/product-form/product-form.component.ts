import { Component, Input } from '@angular/core';
import { Product } from '../Model';
import { ProductService } from '../product.service';

@Component({
  selector: 'app-product-form',
  templateUrl: './product-form.component.html',
  styleUrls: ['./product-form.component.css']
})
export class ProductFormComponent {

  @Input() products: Product[] | undefined;

  constructor(private productService: ProductService) { }

  addProduct(name: string, price: string, categoryId: string, imageUrl : string, stock:string) {
    const categoryIdNumber = parseInt(categoryId, 10);
    const stockNumber = parseInt(stock);

    const p: Product = {
      id: 0,
      name: name,
      price: parseFloat(price),
      imageUrl: imageUrl,
      categoryId: categoryIdNumber,
      categoryName: undefined,
      stock: stockNumber
    };

    this.productService.saveProduct(p).subscribe(
      (product) => {
        if (this.products) {
          this.products.push(product);
        } else {
          this.products = [product];
        }
      },
      (error) => {
        console.error("Ürün kaydedilemedi:", error);
      }
    );
  }


}

