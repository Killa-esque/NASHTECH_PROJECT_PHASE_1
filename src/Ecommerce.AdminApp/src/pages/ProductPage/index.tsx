import ComponentCard from "@/components/common/ComponentCard";
import PageBreadcrumb from "@/components/common/PageBreadCrumb";
import PageMeta from "@/components/common/PageMeta";
import ProductModal from "@/components/product/ProductModal";
import ProductTable from "@/components/product/ProductTable";
import Button from "@/components/ui/button/Button";
import ConfirmDeleteModal from "@/components/ui/modal/ConfirmDeleteModal";
import { useAuth } from "@/hooks/useAuth";
import { useModal } from "@/hooks/useModal";
import { useProduct } from "@/hooks/useProduct";
import { ICreateProduct, IProduct, IUpdateProduct } from "@/types/types";
import { message } from "antd";
import { useCallback, useEffect, useState } from "react";
import { Navigate } from "react-router-dom";

export default function Product() {
  const { isAuthenticated, isAdminUser, isLoading: authLoading } = useAuth();
  const {
    useProducts,
    useProductById,
    useCreateProduct,
    useUpdateProduct,
    useDeleteProduct,
    useSetFeatured,
  } = useProduct();
  const [pageIndex, setPageIndex] = useState(1);
  const [pageSize, setPageSize] = useState(10);
  const { data, isLoading, isFetching, error } = useProducts(
    pageIndex,
    pageSize
  );
  const products = data?.items || [];
  const totalCount = data?.totalCount || 0;
  const createProductMutation = useCreateProduct();
  const updateProductMutation = useUpdateProduct();
  const deleteProductMutation = useDeleteProduct();
  const setFeaturedMutation = useSetFeatured();

  const createModal = useModal();
  const editModal = useModal();
  const deleteModal = useModal();

  const [selectedProductId, setSelectedProductId] = useState<string | null>(
    null
  );
  const { data: selectedProduct, isLoading: productLoading } = useProductById(
    selectedProductId || ""
  );

  useEffect(() => {
    console.log("useProducts State:", {
      products,
      isLoading,
      isFetching,
      error,
    });
    if (error) {
      message.error(`Failed to fetch products: ${error.message}`);
    }
  }, [products, isLoading, isFetching, error]);

  const handleCreate = useCallback(() => {
    setSelectedProductId(null);
    createModal.openModal();
  }, [createModal]);

  const handleEdit = useCallback(
    (product: IProduct) => {
      setSelectedProductId(product.id);
      editModal.openModal();
    },
    [editModal]
  );

  const handleDelete = useCallback(
    (product: IProduct) => {
      setSelectedProductId(product.id);
      deleteModal.openModal();
    },
    [deleteModal]
  );

  const handleSetFeatured = useCallback(
    async (product: IProduct, isFeatured: boolean) => {
      try {
        await setFeaturedMutation.mutateAsync({
          id: product.id,
          request: { isFeatured },
        });
        message.success(
          isFeatured
            ? "Đặt sản phẩm nổi bật thành công"
            : "Hủy trạng thái nổi bật thành công"
        );
      } catch (err) {
        console.error("Failed to set featured status:", err);
        message.error("Không thể cập nhật trạng thái nổi bật");
      }
    },
    [setFeaturedMutation]
  );

  const handleCreateSubmit = useCallback(
    async (data: ICreateProduct) => {
      try {
        const response = await createProductMutation.mutateAsync(data);
        createModal.closeModal();
        message.success("Product created successfully");
        return response.data; // Return the product ID for image upload
      } catch (err) {
        console.error("Failed to create product:", err);
        message.error("Failed to create product");
        throw err;
      }
    },
    [createProductMutation, createModal]
  );

  const handleEditSubmit = useCallback(
    async (data: IUpdateProduct) => {
      if (!selectedProductId) return;
      try {
        await updateProductMutation.mutateAsync({
          id: selectedProductId,
          data,
        });
        editModal.closeModal();
        message.success("Product updated successfully");
      } catch (err) {
        console.error("Failed to update product:", err);
        message.error("Failed to update product");
      }
    },
    [updateProductMutation, editModal, selectedProductId]
  );

  const handleDeleteConfirm = useCallback(async () => {
    if (!selectedProductId) return;
    try {
      await deleteProductMutation.mutateAsync(selectedProductId);
      deleteModal.closeModal();
      message.success("Product deleted successfully");
      setSelectedProductId(null);
    } catch (err) {
      console.error("Failed to delete product:", err);
      message.error("Failed to delete product");
    }
  }, [deleteProductMutation, deleteModal, selectedProductId]);

  const handlePageChange = useCallback(
    (newPage: number, newPageSize: number) => {
      setPageIndex(newPage);
      setPageSize(newPageSize);
    },
    []
  );

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
    return <div>Error: {error.message}</div>;
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
              onSetFeatured={handleSetFeatured}
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

      <ProductModal
        isOpen={createModal.isOpen}
        onClose={createModal.closeModal}
        onCreate={handleCreateSubmit}
      />

      <ProductModal
        isOpen={editModal.isOpen}
        onClose={() => {
          editModal.closeModal();
          setSelectedProductId(null);
        }}
        isEdit
        initialData={selectedProduct || {}}
        onEdit={handleEditSubmit}
        isLoading={productLoading}
      />

      <ConfirmDeleteModal
        isOpen={deleteModal.isOpen}
        onClose={() => {
          deleteModal.closeModal();
          setSelectedProductId(null);
        }}
        onConfirm={handleDeleteConfirm}
        objectType="sản phẩm"
        targetLabel={selectedProduct?.name}
      />
    </div>
  );
}
