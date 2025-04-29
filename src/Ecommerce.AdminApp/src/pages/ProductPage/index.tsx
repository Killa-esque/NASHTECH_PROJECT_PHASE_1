// src/pages/ProductPage/index.tsx
import {
  CreateProductDto,
  ProductResponseDto,
  UpdateProductDto,
} from "@/api/product/productTypes";
import { useAuth } from "@/hooks/useAuth";
import { useModal } from "@/hooks/useModal";
import {
  useCreateProduct,
  useDeleteProduct,
  useGetProducts,
  useUpdateProduct,
} from "@/hooks/useProduct";
import { useEffect, useState } from "react";
import { Navigate } from "react-router-dom";

import ComponentCard from "@/components/common/ComponentCard";
import PageBreadcrumb from "@/components/common/PageBreadCrumb";
import PageMeta from "@/components/common/PageMeta";
import ProductModal from "@/components/product/ProductModal";
import ProductTable from "@/components/product/ProductTable";
import Button from "@/components/ui/button/Button";
import ConfirmDeleteModal from "@/components/ui/modal/ConfirmDeleteModal";
import { message } from "antd";

export default function Product() {
  const { isAuthenticated, isAdminUser, isLoading: authLoading } = useAuth();
  const [selectedProduct, setSelectedProduct] =
    useState<ProductResponseDto | null>(null);

  const createModal = useModal();
  const editModal = useModal();
  const deleteModal = useModal();

  const {
    data: products = [],
    isLoading,
    isFetching,
    error,
  } = useGetProducts();

  useEffect(() => {
    console.log("useGetProducts State:", {
      products,
      isLoading,
      isFetching,
      error,
    });
    if (error) {
      message.error(`Failed to fetch products: ${(error as Error).message}`);
    }
  }, [products, isLoading, isFetching, error]);

  const createProductMutation = useCreateProduct();
  const updateProductMutation = useUpdateProduct();
  const deleteProductMutation = useDeleteProduct();

  const handleCreate = () => {
    setSelectedProduct(null);
    createModal.openModal();
  };

  const handleEdit = (product: ProductResponseDto) => {
    setSelectedProduct(product);
    editModal.openModal();
  };

  const handleDelete = (product: ProductResponseDto) => {
    setSelectedProduct(product);
    deleteModal.openModal();
  };

  const handleCreateSubmit = async (data: CreateProductDto) => {
    try {
      await createProductMutation.mutateAsync(data);
      createModal.closeModal();
    } catch (err) {
      console.error("Failed to create product:", err);
      message.error("Failed to create product");
    }
  };

  const handleEditSubmit = async (data: UpdateProductDto) => {
    if (!selectedProduct) return;
    try {
      await updateProductMutation.mutateAsync({ id: selectedProduct.id, data });
      editModal.closeModal();
    } catch (err) {
      console.error("Failed to update product:", err);
      message.error("Failed to update product");
    }
  };

  const handleDeleteConfirm = async () => {
    if (!selectedProduct) return;
    try {
      await deleteProductMutation.mutateAsync(selectedProduct.id);
      deleteModal.closeModal();
    } catch (err) {
      console.error("Failed to delete product:", err);
      message.error("Failed to delete product");
    }
  };

  if (authLoading || isLoading || isFetching) {
    return <div>Loading...</div>;
  }

  if (!isAuthenticated) {
    return <Navigate to="/signin" replace />;
  }

  if (!isAdminUser) {
    return <Navigate to="/unauthorized" replace />;
  }

  if (error) {
    return <div>Error: {(error as Error).message}</div>;
  }

  return (
    <div>
      <PageMeta
        title="Quản lý sản phẩm | Admin - Tiệm Bánh Ngọt"
        description="Trang quản lý sản phẩm trong bảng điều khiển admin của Tiệm Bánh Ngọt."
      />
      <PageBreadcrumb pageTitle="Sản phẩm" />

      <div className="space-y-6">
        <ComponentCard
          title="Danh sách sản phẩm"
          action={
            <Button size="sm" onClick={handleCreate}>
              + Thêm sản phẩm
            </Button>
          }
        >
          {products.length === 0 ? (
            <div className="py-4 text-center text-gray-500">
              Chưa có sản phẩm nào. Nhấn "Thêm sản phẩm" để tạo mới.
            </div>
          ) : (
            <ProductTable
              data={products}
              onEdit={handleEdit}
              onDelete={handleDelete}
            />
          )}
        </ComponentCard>
      </div>

      <ProductModal
        isOpen={createModal.isOpen}
        onClose={createModal.closeModal}
        onCreate={handleCreateSubmit}
      />

      <ProductModal
        isOpen={editModal.isOpen}
        onClose={editModal.closeModal}
        isEdit
        initialData={selectedProduct || {}}
        onEdit={handleEditSubmit}
      />

      <ConfirmDeleteModal
        isOpen={deleteModal.isOpen}
        onClose={deleteModal.closeModal}
        onConfirm={handleDeleteConfirm}
        objectType="sản phẩm"
        targetLabel={selectedProduct?.name}
      />
    </div>
  );
}
