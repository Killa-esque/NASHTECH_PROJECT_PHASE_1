// src/api/useCustomer.ts
import { customerService } from "@/api/customer/customerService";
import { ICreateCustomer, ICustomer, IUpdateCustomer } from "@/types/types";
import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query";

export const useCustomer = () => {
  const queryClient = useQueryClient();

  const useCustomers = (pageIndex: number = 1, pageSize: number = 10) =>
    useQuery<PagedResult<ICustomer>, Error>({
      queryKey: ["customers", pageIndex, pageSize],
      queryFn: async () => {
        const response = await customerService.getCustomers(
          pageIndex,
          pageSize
        );
        if (!response.status || !response.data) {
          throw new Error(response.error || "Failed to fetch products");
        }
        return response.data;
      },
    });

  const useCustomerById = (
    id: string,
    { enabled, refetchOnMount }: { enabled: boolean; refetchOnMount: boolean }
  ) =>
    useQuery<ICustomer, Error>({
      queryKey: ["customer", id],
      queryFn: async () => {
        const response = await customerService.getCustomerById(id);
        if (!response.status || !response.data) {
          throw new Error(response.error || "Failed to fetch product");
        }
        return response.data;
      },
      enabled,
      refetchOnMount,
    });

  const useCreateCustomer = () =>
    useMutation<HttpResponse<ICustomer>, Error, ICreateCustomer>({
      mutationFn: customerService.createCustomer,
      onSuccess: (response) => {
        if (response.status) {
          queryClient.invalidateQueries({ queryKey: ["customers"] });
        }
      },
    });

  const useUpdateCustomer = () =>
    useMutation<
      HttpResponse<ICustomer>,
      Error,
      { id: string; data: IUpdateCustomer }
    >({
      mutationFn: ({ id, data }) => customerService.updateCustomer(id, data),
      onSuccess: (response, { id }) => {
        if (response.status) {
          queryClient.invalidateQueries({ queryKey: ["customers"] });
          queryClient.invalidateQueries({ queryKey: ["customer", id] });
        }
      },
    });

  const useDeleteCustomer = () =>
    useMutation<HttpResponse<boolean>, Error, string>({
      mutationFn: customerService.deleteCustomer,
      onSuccess: (response) => {
        if (response.status) {
          queryClient.invalidateQueries({ queryKey: ["customers"] });
        }
      },
    });

  return {
    useCustomers,
    useCustomerById,
    useCreateCustomer,
    useUpdateCustomer,
    useDeleteCustomer,
  };
};
