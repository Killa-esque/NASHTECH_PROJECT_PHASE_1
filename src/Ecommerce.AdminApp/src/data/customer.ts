import { ICustomer } from "@/types/customer";

export const customerData: ICustomer[] = [
  {
    id: "1",
    fullName: "Nguyễn Thị Mai",
    location: "Hà Nội",
    role: "Khách hàng",
    userInfo: {
      firstName: "Nguyễn",
      lastName: "Mai",
      email: "mai@gmail.com",
      phone: "0909123456",
      bio: "Thường mua bánh vào dịp cuối tuần.",
    },
    address: {
      country: "Việt Nam",
      cityState: "Hà Nội",
      postalCode: "100000",
      taxId: "TX999123",
    },
    totalOrders: 5,
    totalSpent: 850000,
    createdAt: "2024-03-01",
    orders: [
      { id: "HD001", date: "2024-02-25", amount: 150000 },
      { id: "HD002", date: "2024-02-28", amount: 700000 },
    ],
  },
  {
    id: "2",
    fullName: "Trần Văn A",
    location: "TP. HCM",
    role: "Khách hàng",
    userInfo: {
      firstName: "Trần",
      lastName: "A",
      email: "tranvana@gmail.com",
      phone: "0912345678",
      bio: "Khách hàng thân thiết khu vực miền Nam.",
    },
    address: {
      country: "Việt Nam",
      cityState: "TP. HCM",
      postalCode: "700000",
      taxId: "TX888888",
    },
    totalOrders: 3,
    totalSpent: 480000,
    createdAt: "2024-03-05",
    orders: [
      { id: "HD010", date: "2024-03-01", amount: 180000 },
      { id: "HD011", date: "2024-03-04", amount: 300000 },
    ],
  },
];
