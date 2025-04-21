export interface IOrderProduct {
  name: string;
  quantity: number;
  price: number;
}

export interface IOrder {
  id: string;
  date: string;
  customer: {
    name: string;
    phone: string;
    address: string;
  };
  products: IOrderProduct[];
  total: number;
  status: "pending" | "processing" | "completed" | "cancelled";
  note?: string;
}
