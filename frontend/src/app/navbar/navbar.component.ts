import { Component } from '@angular/core';
import { RouterModule } from '@angular/router';
import { Router } from '@angular/router';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-navbar',
  standalone: true,
  imports: [RouterModule, CommonModule],
  templateUrl: './navbar.component.html',
  styleUrl: './navbar.component.css'
})

export class NavbarComponent {
  constructor(private router: Router) {}

  get username(): string {
    return localStorage.getItem("username") || "";
  }

  get isLoggedIn(): boolean {
    return !!localStorage.getItem("token");
  }

  get rolName():string {
    return localStorage.getItem("rolName") || "";
  }

  goToProfile(){
    const userId = localStorage.getItem("userId");
    const rolName= this.rolName.toLowerCase();
    switch (rolName){
      case "süper admin":
        this.router.navigate([`/süperAdmin/${userId}`]);
        break;
      case "admin":  
       this.router.navigate([`/admin/${userId}`]);
        break;
      case "user":
        this.router.navigate([`/user/${userId}`]);
    }
    
  }

  logout() {
    localStorage.removeItem("token");
    localStorage.removeItem("username");
    this.router.navigate(['/']);
  }

}
