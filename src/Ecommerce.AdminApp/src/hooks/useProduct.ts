import { productService } from "@/api/product/productService";
import {
  ICreateProduct,
  IProduct,
  ISetFeaturedRequest,
  IUpdateProduct,
} from "@/types/types";
import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query";

export const useProduct = () => {
  const queryClient = useQueryClient();

  // Fetch all products
  const useProducts = (pageIndex: number = 1, pageSize: number = 10) =>
    useQuery<PagedResult<IProduct>, Error>({
      queryKey: ["products", pageIndex, pageSize],
      queryFn: async () => {
        const response = await productService.getAll(pageIndex, pageSize);
        if (!response.status || !response.data) {
          throw new Error(response.error || "Failed to fetch products");
        }
        return response.data;
      },
    });

  // Fetch product by ID
  const useProductById = (id: string) =>
    useQuery<IProduct, Error>({
      queryKey: ["product", id],
      queryFn: async () => {
        const response = await productService.getById(id);
        if (!response.status || !response.data) {
          throw new Error(response.error || "Failed to fetch product");
        }
        return response.data;
      },
      enabled: !!id,
    });

  // Create product
  const useCreateProduct = () =>
    useMutation<HttpResponse<string>, Error, ICreateProduct>({
      mutationFn: productService.create,
      onSuccess: (response) => {
        if (response.status) {
          queryClient.invalidateQueries({ queryKey: ["products"] });
        }
      },
    });

  // Update product
  const useUpdateProduct = () =>
    useMutation<
      HttpResponse<string>,
      Error,
      { id: string; data: IUpdateProduct }
    >({
      mutationFn: ({ id, data }) => productService.update(id, data),
      onSuccess: (response) => {
        if (response.status) {
          queryClient.invalidateQueries({ queryKey: ["products"] });
        }
      },
    });

  // Delete product
  const useDeleteProduct = () =>
    useMutation<HttpResponse<string>, Error, string>({
      mutationFn: productService.delete,
      onSuccess: (response) => {
        if (response.status) {
          queryClient.invalidateQueries({ queryKey: ["products"] });
        }
      },
    });

  // Set featured status
  const useSetFeatured = () =>
    useMutation<
      HttpResponse<string>,
      Error,
      { id: string; request: ISetFeaturedRequest }
    >({
      mutationFn: ({ id, request }) => productService.setFeatured(id, request),
      onSuccess: (response) => {
        if (response.status) {
          queryClient.invalidateQueries({ queryKey: ["products"] });
        }
      },
    });

  // Upload product images
  const useUploadProductImages = () =>
    useMutation<HttpResponse<string>, Error, { id: string; files: File[] }>({
      mutationFn: ({ id, files }) => productService.uploadImages(id, files),
      onSuccess: (response, { id }) => {
        if (response.status) {
          queryClient.invalidateQueries({ queryKey: ["products"] });
          queryClient.invalidateQueries({ queryKey: ["product", id] });
        }
      },
    });

  // Delete product image
  const useDeleteProductImage = () =>
    useMutation<HttpResponse<string>, Error, { id: string; imageUrl: string }>({
      mutationFn: ({ id, imageUrl }) =>
        productService.deleteImage(id, imageUrl),
      onSuccess: (response, { id }) => {
        if (response.status) {
          queryClient.invalidateQueries({ queryKey: ["products"] });
          queryClient.invalidateQueries({ queryKey: ["product", id] });
        }
      },
    });

  return {
    useProducts,
    useProductById,
    useCreateProduct,
    useUpdateProduct,
    useDeleteProduct,
    useSetFeatured,
    useUploadProductImages,
    useDeleteProductImage,
  };
};
