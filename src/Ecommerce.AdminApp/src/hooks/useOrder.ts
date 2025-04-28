import { orderService } from "@/api/order/orderService";
import { OrderResponseDto, UpdateOrderDto } from "@/api/order/orderTypes";
import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query";

// Query keys cho caching
const ORDER_QUERY_KEYS = {
  all: ["orders"] as const,
  byId: (orderId: string) => ["orders", orderId] as const,
};

// Hook để lấy danh sách orders
export const useGetOrders = () => {
  return useQuery<OrderResponseDto[], Error>({
    queryKey: ORDER_QUERY_KEYS.all,
    queryFn: orderService.getOrders,
  });
};

// Hook để lấy order theo ID
export const useGetOrderById = (orderId: string) => {
  return useQuery<OrderResponseDto, Error>({
    queryKey: ORDER_QUERY_KEYS.byId(orderId),
    queryFn: () => orderService.getOrderById(orderId),
    enabled: !!orderId, // Chỉ chạy query nếu orderId tồn tại
  });
};

// Hook để cập nhật trạng thái order
export const useUpdateOrderStatus = () => {
  const queryClient = useQueryClient();
  return useMutation<void, Error, { orderId: string; data: UpdateOrderDto }>({
    mutationFn: ({ orderId, data }) =>
      orderService.updateOrderStatus(orderId, data),
    onSuccess: (_, { orderId }) => {
      // Invalidate cache cho danh sách và order cụ thể
      queryClient.invalidateQueries({ queryKey: ORDER_QUERY_KEYS.all });
      queryClient.invalidateQueries({
        queryKey: ORDER_QUERY_KEYS.byId(orderId),
      });
    },
  });
};
