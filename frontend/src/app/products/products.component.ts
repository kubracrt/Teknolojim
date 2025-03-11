import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Model, Product } from '../Model';
import { ProductService } from '../product.service';
import { ProductFormComponent } from "../product-form/product-form.component";
import { ProductDetailsComponent } from "../product-details/product-details.component";

@Component({
  selector: 'app-products',
  standalone: true,
  imports: [CommonModule, ProductFormComponent, ProductDetailsComponent],
  templateUrl: './products.component.html',
  styleUrls: ['./products.component.css']
})
export class ProductsComponent {
  products: Product[] = [];
  selectedProduct?: Product; 

  constructor(private productService: ProductService) { }
  ngOnInit(): void {
    this.productService.getProducts().subscribe(products => {
      this.products = products;
    });
  }

  onSelectProduct(product:Product){
    this.selectedProduct=product;
  }

  deleteProduct(product: Product) {
    this.productService.deleteProduct(product.id).subscribe(
      () => {
        console.log("Ürün Silindi");
        this.products = this.products.filter(p => p.id !== product.id);
      },

    );
  }

}
