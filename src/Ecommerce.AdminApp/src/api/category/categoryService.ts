// src/api/categoryService.ts

import { ICategory, ICreateCategory, IUpdateCategory } from "@/types/types";
import { API_URLS } from "../apiUrls";
import api from "../axiosConfig";

export const categoryService = {
  getAll: async (
    pageIndex: number = 1,
    pageSize: number = 10
  ): Promise<HttpResponse<PagedResult<ICategory>>> => {
    try {
      const response = await api.get(API_URLS.CATEGORY.BASE, {
        params: { pageIndex, pageSize },
      });
      return response.data;
    } catch (error) {
      return { status: false, error: "Failed to fetch categories" };
    }
  },

  getById: async (id: string): Promise<HttpResponse<ICategory>> => {
    try {
      const response = await api.get(API_URLS.CATEGORY.BY_ID(id));
      return response.data;
    } catch (error) {
      return { status: false, error: "Failed to fetch category" };
    }
  },

  create: async (category: ICreateCategory): Promise<HttpResponse<string>> => {
    try {
      const response = await api.post(API_URLS.CATEGORY.BASE, category);
      return response.data;
    } catch (error) {
      return { status: false, error: "Failed to create category" };
    }
  },

  update: async (
    id: string,
    category: IUpdateCategory
  ): Promise<HttpResponse<string>> => {
    try {
      const response = await api.put(API_URLS.CATEGORY.BY_ID(id), category);
      return response.data;
    } catch (error) {
      return { status: false, error: "Failed to update category" };
    }
  },

  delete: async (id: string): Promise<HttpResponse<string>> => {
    try {
      const response = await api.delete(API_URLS.CATEGORY.BY_ID(id));
      return response.data;
    } catch (error) {
      return { status: false, error: "Failed to delete category" };
    }
  },
};
