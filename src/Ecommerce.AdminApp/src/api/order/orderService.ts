import { API_URLS } from "../apiUrls";
import api from "../axiosConfig";
import {
  OrderListResponseSchema,
  OrderResponseDto,
  OrderSingleResponseSchema,
  UpdateOrderDto,
  UpdateOrderSchema,
} from "./orderTypes";

export const orderService = {
  getOrders: async (): Promise<OrderResponseDto[]> => {
    const response = await api.get(API_URLS.ORDER.BASE);
    const validatedResponse = OrderListResponseSchema.parse(response.data);
    if (!validatedResponse.status) {
      throw new Error(
        validatedResponse.message ?? "An unknown error occurred."
      );
    }
    return validatedResponse.data.items;
  },
  getOrderById: async (orderId: string): Promise<OrderResponseDto> => {
    const response = await api.get(API_URLS.ORDER.BY_ID(orderId));
    const validatedResponse = OrderSingleResponseSchema.parse(response.data);
    if (!validatedResponse.status) {
      throw new Error(
        validatedResponse.message ?? "An unknown error occurred."
      );
    }
    return validatedResponse.data;
  },
  updateOrderStatus: async (
    orderId: string,
    updateData: UpdateOrderDto
  ): Promise<void> => {
    const validatedData = UpdateOrderSchema.parse(updateData);
    await api.post(API_URLS.ORDER.UPDATE_STATUS(orderId), validatedData);
  },
};
