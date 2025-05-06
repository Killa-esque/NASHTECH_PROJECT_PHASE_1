import {
  ICreateCustomer,
  ICustomer,
  IUpdateCustomer,
} from "@/types/types";
import { API_URLS } from "../apiUrls";
import api from "../axiosConfig";

export const customerService = {
  getCustomers: async (
    pageIndex: number = 1,
    pageSize: number = 10
  ): Promise<HttpResponse<PagedResult<ICustomer>>> => {
    try {
      const response = await api.get(API_URLS.CUSTOMER.BASE, {
        params: { pageIndex, pageSize },
      });
      return response.data;
    } catch (error) {
      return { status: false, error: "Failed to fetch products" };
    }
  },

  getCustomerById: async (id: string): Promise<HttpResponse<ICustomer>> => {
    try {
      const response = await api.get(API_URLS.CUSTOMER.BY_ID(id));
      return response.data;
    } catch (error) {
      return { status: false, error: "Failed to fetch product" };
    }
  },

  createCustomer: async (
    data: ICreateCustomer
  ): Promise<HttpResponse<ICustomer>> => {
    try {
      const response = await api.post(API_URLS.CUSTOMER.BASE, data);
      return response.data;
    } catch (error) {
      return { status: false, error: "Failed to create product" };
    }
  },

  updateCustomer: async (
    id: string,
    data: IUpdateCustomer
  ): Promise<HttpResponse<ICustomer>> => {
    try {
      const response = await api.put(API_URLS.CUSTOMER.BY_ID(id), data);
      return response.data;
    } catch (error) {
      return { status: false, error: "Failed to update product" };
    }
  },

  deleteCustomer: async (id: string): Promise<HttpResponse<boolean>> => {
    try {
      const response = await api.delete(API_URLS.CUSTOMER.BY_ID(id));
      return response.data;
    } catch (error) {
      return { status: false, error: "Failed to delete product" };
    }
  },

};
