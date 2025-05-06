// src/pages/CategoryPage.tsx
import CategoryModal from "@/components/category/CategoryModal";
import CategoryTable from "@/components/category/CategoryTable";
import ComponentCard from "@/components/common/ComponentCard";
import PageBreadcrumb from "@/components/common/PageBreadCrumb";
import PageMeta from "@/components/common/PageMeta";
import Button from "@/components/ui/button/Button";
import ConfirmDeleteModal from "@/components/ui/modal/ConfirmDeleteModal";
import { useCategory } from "@/hooks/useCategory";
import { useModal } from "@/hooks/useModal";
import { ICategory, ICreateCategory, IUpdateCategory } from "@/types/types";
import { message } from "antd";
import { useEffect, useState } from "react";

export default function Category() {
  const {
    useCategories,
    useCategoryById,
    useCreateCategory,
    useUpdateCategory,
    useDeleteCategory,
  } = useCategory();
  const [pageIndex, setPageIndex] = useState(1);
  const [pageSize, setPageSize] = useState(10);
  const { data, isLoading, error, isFetching } = useCategories(
    pageIndex,
    pageSize
  );
  const categories = data?.items || [];
  const totalCount = data?.totalCount || 0;
  const createCategoryMutation = useCreateCategory();
  const updateCategoryMutation = useUpdateCategory();
  const deleteCategoryMutation = useDeleteCategory();

  const createModal = useModal();
  const editModal = useModal();
  const deleteModal = useModal();

  const [selectedCategoryId, setSelectedCategoryId] = useState<string | null>(
    null
  );
  const { data: selectedCategory, isLoading: categoryLoading } =
    useCategoryById(selectedCategoryId || "");

  useEffect(() => {
    console.log("useCategories State:", {
      categories,
      isLoading,
      isFetching,
      error,
    });
    if (error) {
      message.error(`Failed to fetch categories: ${error.message}`);
    }
  }, [categories, isLoading, isFetching, error]);

  const handleCreate = () => {
    setSelectedCategoryId(null);
    createModal.openModal();
  };

  const handleEdit = (category: ICategory) => {
    setSelectedCategoryId(category.id);
    editModal.openModal();
  };

  const handleDelete = (category: ICategory) => {
    setSelectedCategoryId(category.id);
    deleteModal.openModal();
  };

  const handleCreateSubmit = async (data: ICreateCategory) => {
    try {
      await createCategoryMutation.mutateAsync(data);
      createModal.closeModal();
      message.success("Category created successfully");
    } catch (err) {
      console.error("Failed to create category:", err);
      message.error("Failed to create category");
    }
  };

  const handleEditSubmit = async (data: IUpdateCategory) => {
    if (!selectedCategoryId) return;
    try {
      await updateCategoryMutation.mutateAsync({
        id: selectedCategoryId,
        data,
      });
      editModal.closeModal();
      message.success("Category updated successfully");
    } catch (err) {
      console.error("Failed to update category:", err);
      message.error("Failed to update category");
    }
  };

  const handleDeleteConfirm = async () => {
    if (!selectedCategoryId) return;
    try {
      await deleteCategoryMutation.mutateAsync(selectedCategoryId);
      deleteModal.closeModal();
      message.success("Category deleted successfully");
      setSelectedCategoryId(null);
    } catch (err) {
      console.error("Failed to delete category:", err);
      message.error("Failed to delete category");
    }
  };

  const handlePageChange = (newPage: number, newPageSize: number) => {
    setPageIndex(newPage);
    setPageSize(newPageSize);
  };

  if (isLoading || isFetching) return <div>Loading...</div>;
  if (error) return <div>Error: {error.message}</div>;

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
              pagination={{
                totalCount,
                pageIndex,
                pageSize,
                onPageChange: handlePageChange,
              }}
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
        onClose={() => {
          editModal.closeModal();
          setSelectedCategoryId(null);
        }}
        isEdit
        initialData={selectedCategory || {}}
        onEdit={handleEditSubmit}
        isLoading={categoryLoading}
      />

      <ConfirmDeleteModal
        isOpen={deleteModal.isOpen}
        onClose={() => {
          deleteModal.closeModal();
          setSelectedCategoryId(null);
        }}
        onConfirm={handleDeleteConfirm}
        objectType="danh mục"
        targetLabel={selectedCategory?.name}
      />
    </div>
  );
}
