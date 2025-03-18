import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { User } from '../Model';
import { ProductService } from '../product.service';

@Component({
  selector: 'app-auth',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './auth.component.html',
  styleUrl: './auth.component.css'
})
export class AuthComponent {
  isRegisterMode = false;

  toogleMode() {
    this.isRegisterMode = !this.isRegisterMode
  }

  constructor(private productService: ProductService) { }

  addUser(username: string, email: string, password: string) {
    const p: User = {
      username: username,
      email: email,
      password: password
    };

    const roleID = 3;

    this.productService.saveUser(p,roleID).subscribe(
      (user) => {
        console.log("Kullanıcı Kaydedildi:", user);
      },
      (error) => {
        console.error("Kullanıcı kaydedilirken bir hata oluştu:", error);
      }
    );
  }
}
