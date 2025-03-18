import { Component } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { ProductService } from '../product.service';
import { User } from '../Model';
import { RouterModule } from '@angular/router';

@Component({
  selector: 'app-seller-auth',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterModule],
  templateUrl: './seller-auth.component.html',
  styleUrl: './seller-auth.component.css'
})
export class SellerAuthComponent {
  constructor(private productService: ProductService) { }

  addSeller(username: string, email: string, password: string) {
    const p: User = {
      username: username,
      email: email,
      password: password
    };

    const roleID = 2;

    this.productService.saveUser(p, roleID).subscribe(
      (seller) => {
        console.log("Kullanıcı Kaydedildi:", seller);
      },
      (error) => {
        console.error("Kullanıcı kaydedilirken bir hata oluştu:", error);
      }
    );
  }
}
