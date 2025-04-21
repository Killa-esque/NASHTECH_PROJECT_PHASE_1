import { useModal } from "@/hooks/useModal";
import { useState } from "react";

import ComponentCard from "@/components/common/ComponentCard";
import PageBreadcrumb from "@/components/common/PageBreadCrumb";
import PageMeta from "@/components/common/PageMeta";
import Button from "@/components/ui/button/Button";

import CategoryModal from "@/components/category/CategoryModal";
import CategoryTable from "@/components/category/CategoryTable";
import ConfirmDeleteModal from "@/components/ui/modal/ConfirmDeleteModal";

import { categoryData } from "@/data/category";
import { ICategory } from "@/types/category";

export default function Category() {
  const [selectedCategory, setSelectedCategory] = useState<ICategory | null>(
    null
  );

  const createModal = useModal();
  const editModal = useModal();
  const deleteModal = useModal();

  const handleCreate = () => {
    setSelectedCategory(null);
    createModal.openModal();
  };

  const handleEdit = (category: ICategory) => {
    setSelectedCategory(category);
    editModal.openModal();
  };

  const handleDelete = (category: ICategory) => {
    setSelectedCategory(category);
    deleteModal.openModal();
  };

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
          <CategoryTable
            data={categoryData}
            onEdit={handleEdit}
            onDelete={handleDelete}
          />
        </ComponentCard>
      </div>

      {/* Modals */}
      <CategoryModal
        isOpen={createModal.isOpen}
        onClose={createModal.closeModal}
        onSubmit={(data) => {
          console.log("Create:", data);
          createModal.closeModal();
        }}
      />

      <CategoryModal
        isOpen={editModal.isOpen}
        onClose={editModal.closeModal}
        onSubmit={(data) => {
          console.log("Update:", selectedCategory?.id, data);
          editModal.closeModal();
        }}
        isEdit
        initialData={{
          name: selectedCategory?.name || "",
          description: selectedCategory?.description || "",
        }}
      />

      <ConfirmDeleteModal
        isOpen={deleteModal.isOpen}
        onClose={deleteModal.closeModal}
        onConfirm={() => {
          console.log("Delete:", selectedCategory?.id);
          deleteModal.closeModal();
        }}
        objectType="danh mục"
        targetLabel={selectedCategory?.name}
      />
    </div>
  );
}
