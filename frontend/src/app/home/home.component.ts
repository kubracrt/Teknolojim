import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';
import { Product } from '../Model';
import { ProductService } from '../product.service';
import { RouterModule } from '@angular/router';

@Component({
  selector: 'app-home',
  imports: [CommonModule, RouterModule],
  templateUrl: './home.component.html',
  styleUrl: './home.component.css'
})
export class HomeComponent {
  products: Product[] = [];

  constructor(private productService: ProductService) { }

  ngOnInit(): void {
    this.productService.getProducts().subscribe(
      products => {
      this.products = products;
    });
  }

}
