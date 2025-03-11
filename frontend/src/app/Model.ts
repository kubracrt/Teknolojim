export class Model {
  products: Array<Product>;

  constructor() {
    this.products = [];
  }
}

export interface Product {
categoryName: any;
  id: number;
  name: string;
  price: number;
  imageUrl: string;
  categoryId: number;
}

