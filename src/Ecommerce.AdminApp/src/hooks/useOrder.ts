import { orderService } from "@/api/order/orderService";
import { IOrder, IUpdateOrder } from "@/types/types";
import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query";

export const useOrder = () => {
  const queryClient = useQueryClient();

  const useOrders = (
    pageIndex: number = 1,
    pageSize: number = 10,
    customerId?: string
  ) =>
    useQuery<PagedResult<IOrder>, Error>({
      queryKey: ["orders", pageIndex, pageSize, customerId],
      queryFn: async () => {
        const response = await orderService.getOrders(
          pageIndex,
          pageSize,
          customerId
        );
        if (!response.status || !response.data) {
          throw new Error(response.error || "Failed to fetch orders");
        }
        return response.data;
      },
    });

  const useOrderById = (id: string) =>
    useQuery<IOrder, Error>({
      queryKey: ["order", id],
      queryFn: async () => {
        const response = await orderService.getOrderById(id);
        if (!response.status || !response.data) {
          throw new Error(response.error || "Failed to fetch order");
        }
        return response.data;
      },
      enabled: !!id,
    });

  const useUpdateOrderStatus = () =>
    useMutation<
      HttpResponse<string>,
      Error,
      { id: string; data: IUpdateOrder }
    >({
      mutationFn: ({ id, data }) => orderService.updateOrderStatus(id, data),
      onSuccess: (response) => {
        if (response.status) {
          queryClient.invalidateQueries({ queryKey: ["orders"] });
        }
      },
    });

  const useCancelOrder = () =>
    useMutation<HttpResponse<string>, Error, { id: string }>({
      mutationFn: ({ id }) => orderService.cancelOrder(id),
      onSuccess: (response) => {
        if (response.status) {
          queryClient.invalidateQueries({ queryKey: ["orders"] });
        }
      },
    });

  return {
    useOrders,
    useOrderById,
    useUpdateOrderStatus,
    useCancelOrder,
  };
};
