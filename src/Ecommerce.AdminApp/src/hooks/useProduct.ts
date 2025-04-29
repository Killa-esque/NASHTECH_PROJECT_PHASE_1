// src/api/product/useProduct.ts
import { productService } from "@/api/product/productService";
import {
  CreateProductDto,
  ProductResponseDto,
  UpdateProductDto,
} from "@/api/product/productTypes";
import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query";
import { message } from "antd";

// Query keys cho caching
const PRODUCT_QUERY_KEYS = {
  all: ["products"] as const,
  byId: (id: string) => ["products", id] as const,
};

// Hook để lấy danh sách products
export const useGetProducts = () => {
  return useQuery<ProductResponseDto[], Error>({
    queryKey: PRODUCT_QUERY_KEYS.all,
    queryFn: productService.getProducts,
    retry: (failureCount, error) => {
      // Retry tối đa 2 lần, trừ khi lỗi là 401 (xử lý bởi axios interceptor)
      if (failureCount >= 2 || (error as any).response?.status === 401) {
        return false;
      }
      return true;
    },
  });
};

// Hook để lấy product theo ID
export const useGetProductById = (id: string) => {
  return useQuery<ProductResponseDto, Error>({
    queryKey: PRODUCT_QUERY_KEYS.byId(id),
    queryFn: () => productService.getProductById(id),
    enabled: !!id,
    retry: (failureCount, error) => {
      if (failureCount >= 2 || (error as any).response?.status === 401) {
        return false;
      }
      return true;
    },
  });
};

// Hook để tạo product mới
export const useCreateProduct = () => {
  const queryClient = useQueryClient();
  return useMutation<ProductResponseDto, Error, CreateProductDto>({
    mutationFn: productService.createProduct,
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: PRODUCT_QUERY_KEYS.all });
      message.success("Product created successfully");
    },
    onError: (error) => {
      message.error("Failed to create product: " + error.message);
    },
  });
};

// Hook để cập nhật product
export const useUpdateProduct = () => {
  const queryClient = useQueryClient();
  return useMutation<
    ProductResponseDto,
    Error,
    { id: string; data: UpdateProductDto }
  >({
    mutationFn: ({ id, data }) => productService.updateProduct(id, data),
    onSuccess: (_, { id }) => {
      queryClient.invalidateQueries({ queryKey: PRODUCT_QUERY_KEYS.all });
      queryClient.invalidateQueries({ queryKey: PRODUCT_QUERY_KEYS.byId(id) });
      message.success("Product updated successfully");
    },
    onError: (error) => {
      message.error("Failed to update product: " + error.message);
    },
  });
};

// Hook để xóa product
export const useDeleteProduct = () => {
  const queryClient = useQueryClient();
  return useMutation<void, Error, string>({
    mutationFn: productService.deleteProduct,
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: PRODUCT_QUERY_KEYS.all });
      message.success("Product deleted successfully");
    },
    onError: (error) => {
      message.error("Failed to delete product: " + error.message);
    },
  });
};

// Hook để set featured cho product
export const useSetProductFeatured = () => {
  const queryClient = useQueryClient();
  return useMutation<void, Error, { id: string; isFeatured: boolean }>({
    mutationFn: ({ id, isFeatured }) =>
      productService.setFeatured(id, isFeatured),
    onSuccess: (_, { id }) => {
      queryClient.invalidateQueries({ queryKey: PRODUCT_QUERY_KEYS.all });
      queryClient.invalidateQueries({ queryKey: PRODUCT_QUERY_KEYS.byId(id) });
      message.success("Product featured status updated");
    },
    onError: (error) => {
      message.error("Failed to update featured status: " + error.message);
    },
  });
};
