export interface ICustomer {
  id?: string;

  // Thông tin hiển thị chung
  avatarUrl?: string;
  fullName?: string;
  role?: string;
  location?: string;

  // Liên kết mạng xã hội (nếu có)
  socialLinks?: {
    facebook?: string;
    instagram?: string;
    linkedin?: string;
    x?: string;
  };

  // Thông tin người dùng
  userInfo?: {
    firstName?: string;
    lastName?: string;
    email?: string;
    phone?: string;
    bio?: string;
  };

  // Địa chỉ
  address?: {
    country?: string;
    cityState?: string;
    postalCode?: string;
    taxId?: string;
  };

  // Đơn hàng
  orders?: {
    id: string;
    date: string;
    amount: number;
  }[];

  // Tổng quan
  totalOrders?: number;
  totalSpent?: number;

  createdAt?: string;
  updatedAt?: string;
}
