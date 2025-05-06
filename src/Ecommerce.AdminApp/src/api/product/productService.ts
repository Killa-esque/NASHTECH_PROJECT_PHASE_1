import { ICreateProduct, IProduct, ISetFeaturedRequest, IUpdateProduct } from "@/types/types";
import api from "../axiosConfig";
import { API_URLS } from "../apiUrls";

export const productService = {
  getAll: async (
    pageIndex: number = 1,
    pageSize: number = 10
  ): Promise<HttpResponse<PagedResult<IProduct>>> => {
    try {
      const response = await api.get(API_URLS.PRODUCT.BASE, {
        params: { pageIndex, pageSize },
      });
      return response.data;
    } catch (error) {
      return { status: false, error: "Failed to fetch products" };
    }
  },

  getById: async (id: string): Promise<HttpResponse<IProduct>> => {
    try {
      const response = await api.get(API_URLS.PRODUCT.BY_ID(id));
      return response.data;
    } catch (error) {
      return { status: false, error: "Failed to fetch product" };
    }
  },

  create: async (product: ICreateProduct): Promise<HttpResponse<string>> => {
    try {
      const response = await api.post(API_URLS.PRODUCT.BASE, product);
      return response.data;
    } catch (error) {
      return { status: false, error: "Failed to create product" };
    }
  },

  update: async (
    id: string,
    product: IUpdateProduct
  ): Promise<HttpResponse<string>> => {
    try {
      const response = await api.put(API_URLS.PRODUCT.BY_ID(id), product);
      return response.data;
    } catch (error) {
      return { status: false, error: "Failed to update product" };
    }
  },

  delete: async (id: string): Promise<HttpResponse<string>> => {
    try {
      const response = await api.delete(API_URLS.PRODUCT.BY_ID(id));
      return response.data;
    } catch (error) {
      return { status: false, error: "Failed to delete product" };
    }
  },

  setFeatured: async (
    id: string,
    request: ISetFeaturedRequest
  ): Promise<HttpResponse<string>> => {
    try {
      const response = await api.patch(
        API_URLS.PRODUCT.SET_FEATURED(id),
        request
      );
      return response.data;
    } catch (error) {
      return { status: false, error: "Failed to update featured status" };
    }
  },

  uploadImages: async (
    id: string,
    files: File[]
  ): Promise<HttpResponse<string>> => {
    try {
      const formData = new FormData();
      files.forEach((file) => formData.append("files", file));
      const response = await api.post(
        `/api/admin/products/${id}/images`,
        formData,
        {
          headers: { "Content-Type": "multipart/form-data" },
        }
      );
      return response.data;
    } catch (error) {
      return { status: false, error: "Failed to upload images" };
    }
  },

  deleteImage: async (
    id: string,
    imageUrl: string
  ): Promise<HttpResponse<string>> => {
    try {
      const response = await api.delete(`/api/admin/products/${id}/images`, {
        params: { imageUrl },
      });
      return response.data;
    } catch (error) {
      return { status: false, error: "Failed to delete image" };
    }
  },
};
