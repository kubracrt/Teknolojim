import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { ProductService } from '../services/product.service';
import { CommonModule } from '@angular/common';
import { UserService } from '../services/user.service';
import { Router } from '@angular/router';
import { Order, Product } from '../Model';
import { User } from '../Model';
import { FormsModule } from '@angular/forms';
import { ProcessedOrdersService } from '../services/processedOrders.service';
import { OrderViewService } from '../services/orderView.service';

@Component({
  selector: 'app-super-admin-detail',
  imports: [CommonModule, FormsModule],
  templateUrl: './super-admin-detail.component.html',
  styleUrls: ['./super-admin-detail.component.css']
})
export class SuperAdminDetailComponent implements OnInit {

  users: any[] = [];
  roles: any[] = [];
  processedOrders: any[] = [];
  orderView: any[] = [];
  userWithRoles: any[] = [];
  products: any[] = [];
  loading = true;
  error: any;
  selectedProduct: Product | null = null;
  selectedUser: User | null = null;

  constructor(
    private userservice: UserService,
    private productservice: ProductService,
    private processedOrdersService: ProcessedOrdersService,
    private orderViewService : OrderViewService,
    private router: Router,
  ) { }

  ngOnInit(): void {
    this.loadUser();
    this.loadProducts();
    this.loadProcessedOrders();
    this.loadOrderView();
  }

  loadUser(): void {
    this.loading = true;
    this.error = null;

    this.userservice.getUsers().subscribe({
      next: (users) => {
        this.users = users;
        this.loadRoles();
      },
      error: (error) => {
        this.error = error;
        this.loading = false;
      }
    });
  }

  loadProcessedOrders (): void {
    this.processedOrdersService.getProcessedOrders().subscribe({
      next: (processedOrders) => {
        this.processedOrders = processedOrders;
        console.log('Processed Orders Yüklendi:', processedOrders);
      },
      error: (error) => {
        console.error('Processed Orders yüklenirken hata oluştu:', error);
        this.error = error;
        this.loading = false;
        
      }
    });
  }

  loadOrderView(): void {
    this.orderViewService.getOrderView().subscribe({
      next: (orderView) => {
        this.orderView = orderView;
        console.log('Order View Yüklendi:', orderView);
      },
      error: (error) => {
        console.error('Order View yüklenirken hata oluştu:', error);
        this.error = error;
        this.loading = false;
      }
    });
    
  }

  loadRoles(): void {
    this.userservice.getUserRoles(  ).subscribe({
      next: (roles) => {
        this.roles = roles;
        this.mergeUsersWithRoles();
      },
      error: (error) => {
        this.error = error;
        this.loading = false;
      }
    });
  }

  loadProducts(): void {
    console.log('loadProducts fonksiyonu çalıştı!');
    this.productservice.getProducts().subscribe({
      next: (products) => {
        console.log('Products Yüklendi:', products);
        this.products = products;
      },
      error: (error) => {
        console.error('Ürünler yüklenirken hata oluştu:', error);
        this.error = error;
        this.loading = false;
      }
    });
  }

  mergeUsersWithRoles(): void {
    this.userWithRoles = this.users.map(user => {
      let rolName = 'Rol Yok';
      const userRole = this.roles.find(role => role.userID === user.id);

      if (userRole && userRole.rolName) {
        rolName = userRole.rolName;
      }

      return { ...user, rolName };
    });
  }

  deleteProduct(product: any) {
    this.productservice.deleteProduct(product.id).subscribe({
      next: () => {
        console.log("Product Silindi");
        this.products = this.products.filter(p => p.id !== product.id);
      },
      error: (error) => {
        console.error("Ürün silme hatası:", error);
      }
    });
  }

  onSelectProduct(product: Product) {
    this.selectedProduct = { ...product };
  }

  editProduct(product: Product) {
    this.productservice.updateProduct(product).subscribe({
      next: (response) => {
        console.log("Ürün Güncellendi", response);
        this.selectedProduct = null;
        this.loadProducts();
      },
      error: (error) => {
        console.error("Ürün güncelleme hatası:", error);
      }
    });
  }

  onSelectUser(user: User) {
    console.log("Kullanıcı seçildi:", user);
    this.selectedUser = { ...user };
  }

  editUser(user: User) {
    this.userservice.updateUser(user).subscribe({
      next: (response) => {
        console.log("User Güncellendi", response);
        this.loadUser();
      },
      error: (error) => {
        console.log("Kullanıcı güncelleme hatası:", error);
      }
    });
  }

  deleteUser(user: any) {
    this.userservice.deleteUser(user.id).subscribe({
      next: () => {
        console.log("Kullanıcı Silindi");
        this.users = this.users.filter(u => u.id !== user.id);
      },
      error: (error) => {
        console.error("Kullanıcı silme hatası:", error);
      }
    });
  }

  logout() {
    localStorage.removeItem("token");
    this.router.navigate(['/login']);
  }
}
