import {
  CategoryListResponseSchema,
  CategoryResponseDto,
  CategoryResponseSchema,
  CreateCategoryDto,
  UpdateCategoryDto,
} from "@/api/category/categoryTypes";
import { API_URLS } from "../apiUrls";
import api from "../axiosConfig";

const categoryService = {
  getCategories: async (): Promise<CategoryResponseDto[]> => {
    const response = await api.get(API_URLS.CATEGORY.BASE);
    const validatedResponse = CategoryListResponseSchema.parse(response.data);
    if (!validatedResponse.status) {
      throw new Error(
        validatedResponse.message ?? "An unknown error occurred."
      );
    }
    return validatedResponse.data.items;
  },

  createCategory: async (
    data: CreateCategoryDto
  ): Promise<CategoryResponseDto> => {
    const response = await api.post(API_URLS.CATEGORY.BASE, data);
    const validatedResponse = CategoryResponseSchema.parse(response.data);
    if (!validatedResponse.status) {
      throw new Error(
        validatedResponse.message ?? "An unknown error occurred."
      );
    }
    return validatedResponse.data;
  },

  updateCategory: async (
    id: string,
    data: UpdateCategoryDto
  ): Promise<CategoryResponseDto> => {
    const response = await api.patch(`${API_URLS.CATEGORY.BASE}/${id}`, data);
    const validatedResponse = CategoryResponseSchema.parse(response.data);
    if (!validatedResponse.status) {
      throw new Error(
        validatedResponse.message ?? "An unknown error occurred."
      );
    }
    return validatedResponse.data;
  },

  deleteCategory: async (id: string): Promise<void> => {
    const response = await api.delete(API_URLS.CATEGORY.BY_ID(id));
    const validatedResponse = CategoryResponseSchema.parse(response.data);
    if (!validatedResponse.status) {
      throw new Error(
        validatedResponse.message ?? "An unknown error occurred."
      );
    }
  },
};

export default categoryService;
