import { IOrder } from "@/types/order";

export const mockOrders: IOrder[] = [
  {
    id: "HD001",
    date: "2024-04-01",
    customer: {
      name: "Nguyễn Văn A",
      phone: "0909111222",
      address: "123 Đường Bánh Kem, Quận 5, TP.HCM",
    },
    products: [
      { name: "Bánh kem sô-cô-la", quantity: 2, price: 120000 },
      { name: "Bánh bông lan", quantity: 1, price: 90000 },
    ],
    total: 330000,
    status: "completed",
    note: "Giao buổi sáng trước 10h.",
  },
  {
    id: "HD002",
    date: "2024-04-02",
    customer: {
      name: "Trần Thị B",
      phone: "0909333444",
      address: "456 Lê Văn Sỹ, Quận 3, TP.HCM",
    },
    products: [
      { name: "Bánh flan", quantity: 3, price: 30000 },
      { name: "Bánh quy bơ", quantity: 2, price: 40000 },
    ],
    total: 170000,
    status: "pending",
  },
  {
    id: "HD003",
    date: "2024-04-03",
    customer: {
      name: "Lê Minh C",
      phone: "0909888777",
      address: "789 Nguyễn Trãi, Quận 1, TP.HCM",
    },
    products: [{ name: "Bánh mì ngọt", quantity: 5, price: 25000 }],
    total: 125000,
    status: "processing",
  },
];
