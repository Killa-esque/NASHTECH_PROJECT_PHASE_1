import { z } from "zod";
import {
  HttpResponseSchema,
  PaginatedResponseSchema,
} from "../httpResponseTypes";

// Schema cho OrderItem
const OrderItemSchema = z.object({
  id: z.string(),
  productId: z.string(),
  quantity: z.number(),
  price: z.number(),
});

export type OrderItemDto = z.infer<typeof OrderItemSchema>;

// Schema cho Order (chỉ phần data)
const OrderSchema = z.object({
  id: z.string(),
  userId: z.string(),
  orderCode: z.string(),
  shippingAddress: z.string(),
  paymentMethod: z.string(),
  note: z.string(),
  status: z.string(),
  createdDate: z.string().optional(),
  items: z.array(OrderItemSchema).nullable(),
  totalAmount: z.number(),
});

export type OrderResponseDto = z.infer<typeof OrderSchema>;

// Schema cho response trả về danh sách (có pagination)
const OrderPaginatedSchema = PaginatedResponseSchema(OrderSchema);
const OrderListResponseSchema = HttpResponseSchema(OrderPaginatedSchema);
const OrderSingleResponseSchema = HttpResponseSchema(OrderSchema);

// Types cho response
export type OrderListResponseDto = z.infer<typeof OrderListResponseSchema>;
export type OrderSingleResponseDto = z.infer<typeof OrderSingleResponseSchema>;

// Type cho request (loại bỏ các thuộc tính không cần thiết)
export type CreateOrderDto = Omit<
  OrderResponseDto,
  "id" | "userId" | "orderCode" | "status" | "createdDate"
>;

// Schema và type cho update
const UpdateOrderSchema = z.object({
  status: z.string(),
  note: z.string(),
  shippingAddress: z.string(),
});

export type UpdateOrderDto = z.infer<typeof UpdateOrderSchema>;

// Export schemas để dùng trong service
export {
  OrderListResponseSchema,
  OrderSchema,
  OrderSingleResponseSchema,
  UpdateOrderSchema,
};
