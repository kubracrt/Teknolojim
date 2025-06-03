import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';
import { Product } from '../Model';
import { ProductService } from '../services/product.service';
import { RouterModule } from '@angular/router';
import { ShoppingCardService } from '../services/shoppingCard.service';
import { ShoppingCard } from '../Model';

@Component({
  selector: 'app-home',
  imports: [CommonModule, RouterModule],
  templateUrl: './home.component.html',
  styleUrl: './home.component.css'
})
export class HomeComponent {
  products: Product[] = [];

  constructor(private productService: ProductService, private shoppingCardService: ShoppingCardService) { }

  ngOnInit(): void {
    this.productService.getTop10Products().subscribe(
      products => {
        this.products = products;
      });
  }

  addShoppingCard(p: Product) {
    console.log("Sepete Eklenecek Ürün Bilgileri");

    const userId = parseInt(localStorage.getItem("userId") || "0");
    const userName = localStorage.getItem("username");
  
    if (!userId ) {
      alert("Kullanıcı Girişi Yapınız");
      console.error("Kullanıcı Girişi Yapınız");
      return;
    }

    const shoppingCard = {
      price : p.price,
      userId,
      userName,
      productName:p.name,
      productId:p.id,
      imageUrl:p.imageUrl,
      quantity:"1",
    };

    this.shoppingCardService.saveShoppingCard(shoppingCard).subscribe({
      next: (response) => {
        console.log("Seppette Kayıtlı Ürün Bilgileri", response);
      }
    })
  }

}

