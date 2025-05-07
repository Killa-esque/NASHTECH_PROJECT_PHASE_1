// src/api/types.ts
export interface ICategory {
  id: string;
  name: string;
  description: string;
  createdDate?: string;
}

export interface IProduct {
  id: string;
  name: string;
  description: string;
  price: number;
  categoryId: string;
  stock: number;
  imageUrls: string[];
  weight?: string;
  ingredients?: string;
  expirationDate?: string;
  storageInstructions?: string;
  isFeatured?: boolean;
  allergens?: string;
  createdDate?: string;
  updatedAt?: string;
}

export interface ISetFeaturedRequest {
  isFeatured: boolean;
}

export interface ICreateCategory {
  name: string;
  description: string;
}

export interface IUpdateCategory {
  name: string;
  description: string;
}

export interface ICreateProduct {
  name: string;
  description: string;
  price: number;
  categoryId: string;
  stock?: number;
  weight?: string;
  ingredients?: string;
  expirationDate?: string;
  storageInstructions?: string;
  allergens?: string;
}

export interface IUpdateProduct {
  name: string;
  description: string;
  price: number;
  categoryId: string;
  stock?: number;
  weight?: string;
  ingredients?: string;
  expirationDate?: string;
  storageInstructions?: string;
  allergens?: string;
}

export interface ICustomer {
  id: string;
  userName: string;
  email: string;
  fullName: string;
  dateOfBirth?: string;
  phoneNumber: string;
  gender: "Male" | "Female" | "Other";
  defaultAddress: string;
  avatarUrl: string;
  allergyNotes: string;
  password?: string;
}

export interface ICreateCustomer {
  email: string;
  password: string;
  fullName: string;
  dateOfBirth?: string;
  phoneNumber: string;
  gender: "Male" | "Female" | "Other";
  defaultAddress: string;
  allergyNotes: string;
}

export interface IUpdateCustomer {
  fullName: string;
  dateOfBirth?: string;
  phoneNumber: string;
  gender: "Male" | "Female" | "Other";
  defaultAddress: string;
  allergyNotes: string;
}

export interface IOrder {
  id: string;
  userId: string;
  orderCode: string;
  paymentMethod: string;
  deliveryDate?: string;
  totalAmount: number;
  status: "Pending" | "Paid" | "Shipped" | "Completed" | "Cancelled";
  note?: string;
  shippingAddress?: string;
  createdDate?: string;
  items?: IOrderItem[];
}

export interface IOrderItem {
  id: string;
  productId: string;
  productName: string;
  quantity: number;
  price: number;
}

export interface IUpdateOrder {
  orderId: string;
  status?: string;
  note?: string;
  shippingAddress?: string;
}
