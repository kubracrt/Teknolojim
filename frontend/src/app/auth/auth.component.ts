import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { User, } from '../Model';
import { ProductService } from '../product.service';
import { RouterModule, Router } from '@angular/router';

@Component({
  selector: 'app-auth',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterModule],
  templateUrl: './auth.component.html',
  styleUrl: './auth.component.css'
})
export class AuthComponent {
  isRegisterMode = false;
  errorMessage: string = " ";
  successMessage: string = " ";

  user: User = {
    username: '',
    email: '',
    password: ''
  };

  constructor(private productService: ProductService, private router: Router) { }

  toggleMode() {
    this.isRegisterMode = !this.isRegisterMode;
  }

  addUser(username: string, email: string, password: string) {
    const p: User = { username, email, password };
    const roleID = 3;

    this.productService.saveUser(p, roleID).subscribe({
      next: (user) => {
        console.log("Kullanıcı Kaydedildi:", user);
      },
      error: (error) => {
        console.error("Kullanıcı kaydedilirken bir hata oluştu:", error);
        this.errorMessage = 'Kayıt işlemi başarısız oldu.';
      }
    });
  }

  loginUser(email: string, password: string) {
    this.errorMessage = '';
    this.successMessage = '';

    this.productService.loginUser(email, password).subscribe({
      next: (response) => {
        console.log('Giriş başarılı:', response);
        this.successMessage = "Giriş Başarılı.";
        const userId = response.id;

        this.productService.getUserRoles(userId).subscribe((roles: Role[]) => {
          console.log("Kullanıcı Rolleri:", roles);

          if (roles.some((r: Role) => r.rolName === "Süper Admin")) {
            this.router.navigate([`süperAdmin/${userId}`]);
          } else if (roles.some((r: Role) => r.rolName === "Admin")) {
            this.router.navigate([`admin/${userId}`]);
          }
          else {
            this.router.navigate([`user/${userId}`]);
          }
        }, error => {
          console.error("Rol bilgisi alınamadı:", error);
          this.errorMessage = "Rol bilgisi alınırken hata oluştu.";
        });
      },
      error: (error) => {
        console.error('Giriş hatası:', error);
        this.errorMessage = 'Geçersiz e-posta veya şifre.';
      }
    });
  }
}

export interface Role {
  id: number;
  rolName: string;
}
