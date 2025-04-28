import { z } from "zod";
import {
  HttpResponseSchema,
  PaginatedResponseSchema,
} from "../httpResponseTypes";

// Schema cho Product (chỉ phần data)
const ProductSchema = z.object({
  id: z.string(),
  name: z.string(),
  description: z.string(),
  price: z.number(),
  imageUrl: z.string(),
  categoryId: z.string(),
  stock: z.number(),
  createdAt: z.string().optional(),
});

export type ProductResponseDto = z.infer<typeof ProductSchema>;

// Schema cho response trả về danh sách (có pagination)
const ProductPaginatedSchema = PaginatedResponseSchema(ProductSchema);
const ProductListResponseSchema = HttpResponseSchema(ProductPaginatedSchema);
const ProductSingleResponseSchema = HttpResponseSchema(ProductSchema);

// Types cho response
export type ProductListResponseDto = z.infer<typeof ProductListResponseSchema>;
export type ProductSingleResponseDto = z.infer<
  typeof ProductSingleResponseSchema
>;

// Type cho request (loại bỏ id và createdAt)
export type CreateProductDto = Omit<ProductResponseDto, "id" | "createdAt">;
export type UpdateProductDto = Partial<CreateProductDto>;

// Export schemas để dùng trong service
export {
  ProductListResponseSchema,
  ProductSchema,
  ProductSingleResponseSchema,
};
