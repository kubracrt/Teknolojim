import { Component, OnInit } from '@angular/core';
import { ProductService } from '../services/product.service';
import { Product } from '../Model';
import { ActivatedRoute, RouterModule } from '@angular/router';
import { CommonModule } from '@angular/common';
import { ShoppingCardService } from '../services/shoppingCard.service';

@Component({
  selector: 'app-category',
  imports: [CommonModule, RouterModule],
  templateUrl: './category.component.html',
  styleUrls: ['./category.component.css']
})
export class CategoryComponent implements OnInit {

  categoryName: string | undefined;
  products: Product[] = [];

  constructor(private route: ActivatedRoute, private productService: ProductService, private shoppingCardService:ShoppingCardService) { }

  ngOnInit(): void {
    this.route.paramMap.subscribe(params => {
      this.categoryName = params.get('categoryName') ?? '';
      console.log('Kategori adı:', this.categoryName);

      if (this.categoryName) {
        this.productService.getCategoryProduct(this.categoryName).subscribe(
          (products) => {
            console.log('Ürünler:', products);
            this.products = products;
          },
          (error) => {
            console.error('API isteği hatası:', error);
          }
        );
      }
    });
  }

  addShoppingCard(p: Product) {
    console.log("Sepete Eklenecek Ürün Bilgileri");

    const userId = parseInt(localStorage.getItem("userId") || "0");
    const userName = localStorage.getItem("username");

    if (!userId) {
      alert("Kullanıcı Girişi Yapınız");
      console.error("Kullanıcı Girişi Yapınız");
      return;
    }

    const shoppingCard = {
      price: p.price,
      userId,
      userName,
      productName: p.name,
      productId: p.id,
      imageUrl: p.imageUrl,
      quantity: "1",
    };

    this.shoppingCardService.saveShoppingCard(shoppingCard).subscribe({
      next: (response) => {
        console.log("Seppette Kayıtlı Ürün Bilgileri", response);
      }
    })
  }


}



