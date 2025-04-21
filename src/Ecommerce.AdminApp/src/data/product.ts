import { IProduct } from "@/types/product";

export const productData: IProduct[] = [
  {
    id: 1,
    name: "Bánh kem socola",
    categoryId: 1,
    description: "Bánh kem tươi phủ socola đắng",
    price: 120000,
    images: ["/images/products/banh-kem-socola.jpg"],
    createdAt: "2024-04-01",
    updatedAt: "2024-04-10",
  },
  {
    id: 2,
    name: "Bánh su kem",
    categoryId: 3,
    description: "Nhân kem trứng, vỏ mềm",
    price: 25000,
    images: ["/images/products/su-kem.jpg"],
    createdAt: "2024-04-05",
    updatedAt: "2024-04-10",
  },
];
