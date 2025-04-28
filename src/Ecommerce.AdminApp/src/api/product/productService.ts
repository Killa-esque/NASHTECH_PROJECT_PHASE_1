import { API_URLS } from "../apiUrls";
import api from "../axiosConfig";
import {
  CreateProductDto,
  ProductListResponseSchema,
  ProductResponseDto,
  ProductSingleResponseSchema,
  UpdateProductDto,
} from "./productTypes";

export const productService = {
  getProducts: async (): Promise<ProductResponseDto[]> => {
    const response = await api.get(API_URLS.PRODUCT.BASE);
    const validatedResponse = ProductListResponseSchema.parse(response.data);
    if (!validatedResponse.status) {
      throw new Error(
        validatedResponse.message ?? "An unknown error occurred."
      );
    }
    return validatedResponse.data.items;
  },
  createProduct: async (
    product: CreateProductDto
  ): Promise<ProductResponseDto> => {
    const response = await api.post(API_URLS.PRODUCT.BASE, product);
    const validatedResponse = ProductSingleResponseSchema.parse(response.data);
    if (!validatedResponse.status) {
      throw new Error(
        validatedResponse.message ?? "An unknown error occurred."
      );
    }
    return validatedResponse.data;
  },
  getProductById: async (id: string): Promise<ProductResponseDto> => {
    const response = await api.get(API_URLS.PRODUCT.BY_ID(id));
    const validatedResponse = ProductSingleResponseSchema.parse(response.data);
    if (!validatedResponse.status) {
      throw new Error(
        validatedResponse.message ?? "An unknown error occurred."
      );
    }
    return validatedResponse.data;
  },
  updateProduct: async (
    id: string,
    product: UpdateProductDto
  ): Promise<ProductResponseDto> => {
    const response = await api.put(API_URLS.PRODUCT.BY_ID(id), product);
    const validatedResponse = ProductSingleResponseSchema.parse(response.data);
    if (!validatedResponse.status) {
      throw new Error(
        validatedResponse.message ?? "An unknown error occurred."
      );
    }
    return validatedResponse.data;
  },
  deleteProduct: async (id: string): Promise<void> => {
    await api.delete(API_URLS.PRODUCT.BY_ID(id));
  },
  setFeatured: async (id: string, isFeatured: boolean): Promise<void> => {
    await api.patch(API_URLS.PRODUCT.SET_FEATURED(id), { isFeatured });
  },
};
