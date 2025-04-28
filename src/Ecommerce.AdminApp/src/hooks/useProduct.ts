import { productService } from "@/api/product/productService";
import {
  CreateProductDto,
  ProductResponseDto,
  UpdateProductDto,
} from "@/api/product/productTypes";
import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query";

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
  });
};

// Hook để lấy product theo ID
export const useGetProductById = (id: string) => {
  return useQuery<ProductResponseDto, Error>({
    queryKey: PRODUCT_QUERY_KEYS.byId(id),
    queryFn: () => productService.getProductById(id),
    enabled: !!id, // Chỉ chạy query nếu id tồn tại
  });
};

// Hook để tạo product mới
export const useCreateProduct = () => {
  const queryClient = useQueryClient();
  return useMutation<ProductResponseDto, Error, CreateProductDto>({
    mutationFn: productService.createProduct,
    onSuccess: () => {
      // Invalidate cache để refetch danh sách products
      queryClient.invalidateQueries({ queryKey: PRODUCT_QUERY_KEYS.all });
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
      // Invalidate cache cho danh sách và product cụ thể
      queryClient.invalidateQueries({ queryKey: PRODUCT_QUERY_KEYS.all });
      queryClient.invalidateQueries({ queryKey: PRODUCT_QUERY_KEYS.byId(id) });
    },
  });
};

// Hook để xóa product
export const useDeleteProduct = () => {
  const queryClient = useQueryClient();
  return useMutation<void, Error, string>({
    mutationFn: productService.deleteProduct,
    onSuccess: () => {
      // Invalidate cache để refetch danh sách products
      queryClient.invalidateQueries({ queryKey: PRODUCT_QUERY_KEYS.all });
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
      // Invalidate cache cho danh sách và product cụ thể
      queryClient.invalidateQueries({ queryKey: PRODUCT_QUERY_KEYS.all });
      queryClient.invalidateQueries({ queryKey: PRODUCT_QUERY_KEYS.byId(id) });
    },
  });
};
