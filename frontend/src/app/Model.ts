export class Model {
  products: Array<Product>;
  users:Array<User>;

  constructor() {
    this.products = [];
    this.users=[];
  }
}

export interface Product {
  id:number,
  userId:number;
  categoryName: any;
  userName:any;
  name: string;
  price: number;
  imageUrl: string;
  categoryId: number;
  stock: number;
}

export interface User {
  id:number,
  username:string;
  email:string;
  password:string;
}


