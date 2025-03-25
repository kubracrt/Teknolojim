import { Component, Input } from '@angular/core';
import { Product } from '../Model';
import { ProductService } from '../services/product.service';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-product-details',
  imports: [CommonModule, FormsModule],
  standalone: true,
  templateUrl: './product-details.component.html',
  styleUrls: ['./product-details.component.css']
})
export class ProductDetailsComponent {
  @Input() selectedProduct!: Product;

  constructor(private productService: ProductService) {}

  editProduct(product: Product) {
    this.productService.saveProduct(product).subscribe(
      (updatedProduct) => {
        console.log("Ürün başarıyla güncellendi:", updatedProduct);
      },
      (error) => {
        console.error("Ürün güncellenirken bir hata oluştu:", error);
      }
    );
  }
}
