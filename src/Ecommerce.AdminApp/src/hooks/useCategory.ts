// src/api/useCategory.ts
import { categoryService } from "@/api/category/categoryService";
import { ICategory, ICreateCategory, IUpdateCategory } from "@/types/types";
import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query";

export const useCategory = () => {
  const queryClient = useQueryClient();

  // Fetch all categories
  const useCategories = (pageIndex: number = 1, pageSize: number = 10) =>
    useQuery<PagedResult<ICategory>, Error>({
      queryKey: ["categories", pageIndex, pageSize],
      queryFn: async () => {
        const response = await categoryService.getAll(pageIndex, pageSize);
        if (!response.status || !response.data) {
          throw new Error(response.error || "Failed to fetch categories");
        }
        return response.data;
      },
    });

  // Fetch category by ID
  const useCategoryById = (id: string) =>
    useQuery<ICategory, Error>({
      queryKey: ["category", id],
      queryFn: async () => {
        const response = await categoryService.getById(id);
        if (!response.status || !response.data) {
          throw new Error(response.error || "Failed to fetch category");
        }
        return response.data;
      },
      enabled: !!id,
    });

  // Create category
  const useCreateCategory = () =>
    useMutation<HttpResponse<string>, Error, ICreateCategory>({
      mutationFn: categoryService.create,
      onSuccess: (response) => {
        if (response.status) {
          queryClient.invalidateQueries({ queryKey: ["categories"] });
        }
      },
    });

  // Update category
  const useUpdateCategory = () =>
    useMutation<
      HttpResponse<string>,
      Error,
      { id: string; data: IUpdateCategory }
    >({
      mutationFn: ({ id, data }) => categoryService.update(id, data),
      onSuccess: (response) => {
        if (response.status) {
          queryClient.invalidateQueries({ queryKey: ["categories"] });
        }
      },
    });

  // Delete category
  const useDeleteCategory = () =>
    useMutation<HttpResponse<string>, Error, string>({
      mutationFn: categoryService.delete,
      onSuccess: (response) => {
        if (response.status) {
          queryClient.invalidateQueries({ queryKey: ["categories"] });
        }
      },
    });

  return {
    useCategories,
    useCategoryById,
    useCreateCategory,
    useUpdateCategory,
    useDeleteCategory,
  };
};
