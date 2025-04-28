import {
  CategoryResponseDto,
  CreateCategoryDto,
  UpdateCategoryDto,
} from "@/api/category/categoryTypes";
import {
  useCreateCategory,
  useDeleteCategory,
  useGetCategories,
  useUpdateCategory,
} from "@/hooks/useCategory";
import { useModal } from "@/hooks/useModal";
import { useEffect, useState } from "react";

import CategoryModal from "@/components/category/CategoryModal";
import CategoryTable from "@/components/category/CategoryTable";
import ComponentCard from "@/components/common/ComponentCard";
import PageBreadcrumb from "@/components/common/PageBreadCrumb";
import PageMeta from "@/components/common/PageMeta";
import Button from "@/components/ui/button/Button";
import ConfirmDeleteModal from "@/components/ui/modal/ConfirmDeleteModal";

export default function Category() {
  const [selectedCategory, setSelectedCategory] =
    useState<CategoryResponseDto | null>(null);

  const createModal = useModal();
  const editModal = useModal();
  const deleteModal = useModal();

  const {
    data: categories = [],
    isLoading,
    error,
    isFetching,
  } = useGetCategories();

  useEffect(() => {
    console.log("useGetCategories State:", {
      categories,
      isLoading,
      isFetching,
      error,
    });
  }, [categories, isLoading, isFetching, error]);

  const createCategoryMutation = useCreateCategory();
  const updateCategoryMutation = useUpdateCategory();
  const deleteCategoryMutation = useDeleteCategory();

  const handleCreate = () => {
    setSelectedCategory(null);
    createModal.openModal();
  };

  const handleEdit = (category: CategoryResponseDto) => {
    setSelectedCategory(category);
    editModal.openModal();
  };

  const handleDelete = (category: CategoryResponseDto) => {
    setSelectedCategory(category);
    deleteModal.openModal();
  };

  const handleCreateSubmit = async (data: CreateCategoryDto) => {
    try {
      await createCategoryMutation.mutateAsync(data);
      createModal.closeModal();
    } catch (err) {
      console.error("Failed to create category:", err);
    }
  };

  const handleEditSubmit = async (data: UpdateCategoryDto) => {
    if (!selectedCategory) return;
    try {
      await updateCategoryMutation.mutateAsync({
        id: selectedCategory.id,
        data,
      });
      editModal.closeModal();
    } catch (err) {
      console.error("Failed to update category:", err);
    }
  };

  const handleDeleteConfirm = async () => {
    if (!selectedCategory) return;
    try {
      await deleteCategoryMutation.mutateAsync(selectedCategory.id);
      deleteModal.closeModal();
    } catch (err) {
      console.error("Failed to delete category:", err);
    }
  };

  if (isLoading || isFetching) return <div>Loading...</div>;
  if (error) return <div>Error: {(error as Error).message}</div>;

  return (
    <div>
      <PageMeta
        title="Quản lý danh mục sản phẩm | Admin - Tiệm Bánh Ngọt"
        description="Trang quản lý danh mục sản phẩm trong bảng điều khiển admin của Tiệm Bánh Ngọt."
      />
      <PageBreadcrumb pageTitle="Danh mục sản phẩm" />

      <div className="space-y-6">
        <ComponentCard
          title="Danh sách danh mục"
          action={
            <Button size="sm" onClick={handleCreate}>
              + Thêm danh mục
            </Button>
          }
        >
          {categories.length === 0 ? (
            <div className="py-4 text-center text-gray-500">
              Chưa có danh mục nào. Nhấn "Thêm danh mục" để tạo mới.
            </div>
          ) : (
            <CategoryTable
              data={categories}
              onEdit={handleEdit}
              onDelete={handleDelete}
            />
          )}
        </ComponentCard>
      </div>

      <CategoryModal
        isOpen={createModal.isOpen}
        onClose={createModal.closeModal}
        onCreate={handleCreateSubmit}
      />

      <CategoryModal
        isOpen={editModal.isOpen}
        onClose={editModal.closeModal}
        isEdit
        initialData={selectedCategory || {}}
        onEdit={handleEditSubmit}
      />

      <ConfirmDeleteModal
        isOpen={deleteModal.isOpen}
        onClose={deleteModal.closeModal}
        onConfirm={handleDeleteConfirm}
        objectType="danh mục"
        targetLabel={selectedCategory?.name}
      />
    </div>
  );
}
