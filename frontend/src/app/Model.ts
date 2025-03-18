export class Model {
  products: Array<Product>;
  users:Array<User>;

  constructor() {
    this.products = [];
    this.users=[];
  }
}

export interface Product {
  categoryName: any;
  id: number;
  name: string;
  price: number;
  imageUrl: string;
  categoryId: number;
  stock: number;
}

export interface User {
  username:string;
  email:string;
  password:string;
}


