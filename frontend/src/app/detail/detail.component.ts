import { Component, OnInit } from '@angular/core';
import { Product } from '../Model';
import { ProductService } from '../services/product.service';
import { ActivatedRoute } from '@angular/router';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-detail',
  imports: [CommonModule],
  templateUrl: './detail.component.html',
  styleUrl: './detail.component.css'
})
export class DetailComponent implements OnInit {

  productId: number | undefined;
  product: Product | undefined;

  constructor(private route: ActivatedRoute, private productService: ProductService) { }

  ngOnInit(): void {
    this.route.paramMap.subscribe(params => {
      console.log("Tüm Parametreler:", params.keys);
      const id = params.get('productId');

      this.productId = id ? +id : undefined;
      console.log('Dönüştürülen Ürün ID:', this.productId);

      if (this.productId) {
        this.productService.getProduct(this.productId).subscribe(
          product_ => {
            console.log("Ürün:", product_);
            this.product = product_;
          },
          (error) => {
            console.log("API isteği hatası:", error);
          }
        );
      }
    });
  }
}
