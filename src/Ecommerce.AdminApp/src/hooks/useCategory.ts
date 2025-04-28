import categoryService from "@/api/category/categoryService";
import {
  CategoryResponseDto,
  CreateCategoryDto,
  UpdateCategoryDto,
} from "@/api/category/categoryTypes";
import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query";

export const CATEGORY_QUERY_KEYS = {
  all: ["categories"] as const,
  detail: (id: string) => [...CATEGORY_QUERY_KEYS.all, id] as const,
};

export const useGetCategories = () => {
  return useQuery<CategoryResponseDto[], Error>({
    queryKey: CATEGORY_QUERY_KEYS.all,
    queryFn: categoryService.getCategories,
  });
};

export const useCreateCategory = () => {
  const queryClient = useQueryClient();
  return useMutation<CategoryResponseDto, Error, CreateCategoryDto>({
    mutationFn: categoryService.createCategory,
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: CATEGORY_QUERY_KEYS.all });
    },
  });
};

export const useUpdateCategory = () => {
  const queryClient = useQueryClient();
  return useMutation<
    CategoryResponseDto,
    Error,
    { id: string; data: UpdateCategoryDto }
  >({
    mutationFn: ({ id, data }) => categoryService.updateCategory(id, data),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: CATEGORY_QUERY_KEYS.all });
    },
  });
};

export const useDeleteCategory = () => {
  const queryClient = useQueryClient();
  return useMutation<void, Error, string>({
    mutationFn: categoryService.deleteCategory,
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: CATEGORY_QUERY_KEYS.all });
    },
  });
};
