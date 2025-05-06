import { IOrder, IUpdateOrder } from "@/types/types";
import { API_URLS } from "../apiUrls";
import api from "../axiosConfig";

export const orderService = {
  getOrders: async (
    pageIndex: number = 1,
    pageSize: number = 10,
    customerId?: string
  ): Promise<HttpResponse<PagedResult<IOrder>>> => {
    try {
      const response = await api.get(API_URLS.ORDER.BASE, {
        params: { pageIndex, pageSize, customerId },
      });
      return response.data;
    } catch (error) {
      return { status: false, error: "Failed to fetch orders" };
    }
  },

  getOrderById: async (orderId: string): Promise<HttpResponse<IOrder>> => {
    try {
      const response = await api.get(API_URLS.ORDER.BY_ID(orderId));
      return response.data;
    } catch (error) {
      return { status: false, error: "Failed to fetch order" };
    }
  },

  updateOrderStatus: async (
    orderId: string,
    data: IUpdateOrder
  ): Promise<HttpResponse<string>> => {
    try {
      const response = await api.post(API_URLS.ORDER.UPDATE_STATUS(orderId), data);
      return response.data;
    } catch (error) {
      return { status: false, error: "Failed to update order" };
    }
  },

  cancelOrder: async (orderId: string): Promise<HttpResponse<string>> => {
    try {
      const response = await api.post(`/api/admin/orders/${orderId}/cancel`);
      return response.data;
    } catch (error) {
      return { status: false, error: "Failed to cancel order" };
    }
  },
};
