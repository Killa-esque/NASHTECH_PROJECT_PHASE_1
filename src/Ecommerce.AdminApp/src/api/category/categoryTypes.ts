import { z } from "zod";
import {
  HttpResponseSchema,
  PaginatedResponseSchema,
} from "../httpResponseTypes";

// Định nghĩa schema cho một danh mục
const CategorySchema = z.object({
  id: z.string(),
  name: z.string(),
  description: z.string(),
  createdAt: z.string().optional(),
  updatedAt: z.string().optional(),
});

// Định nghĩa schema cho danh sách danh mục (có phân trang)
export const CategoryListResponseSchema = HttpResponseSchema(
  PaginatedResponseSchema(CategorySchema)
);

// Định nghĩa schema cho phản hồi khi tạo/sửa/xóa danh mục
export const CategoryResponseSchema = HttpResponseSchema(CategorySchema);

// Xuất các type từ schema
export type CategoryResponseDto = z.infer<typeof CategorySchema>;
export type CreateCategoryDto = Omit<
  CategoryResponseDto,
  "id" | "createdAt" | "updatedAt"
>;
export type UpdateCategoryDto = Partial<CreateCategoryDto>;
