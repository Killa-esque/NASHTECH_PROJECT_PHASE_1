import { productData } from "@/data/product";
import { useModal } from "@/hooks/useModal";
import { IProduct } from "@/types/product";
import { useState } from "react";

import ComponentCard from "@/components/common/ComponentCard";
import PageBreadcrumb from "@/components/common/PageBreadCrumb";
import PageMeta from "@/components/common/PageMeta";
import ProductModal from "@/components/product/ProductModal";
import ProductTable from "@/components/product/ProductTable";
import Button from "@/components/ui/button/Button";
import ConfirmDeleteModal from "@/components/ui/modal/ConfirmDeleteModal";

export default function Product() {
  const [products, setProducts] = useState<IProduct[]>(productData);
  const [selectedProduct, setSelectedProduct] = useState<IProduct | null>(null);

  const createModal = useModal();
  const editModal = useModal();
  const deleteModal = useModal();

  const handleCreate = () => {
    setSelectedProduct(null);
    createModal.openModal();
  };

  const handleEdit = (product: IProduct) => {
    setSelectedProduct(product);
    editModal.openModal();
  };

  const handleDelete = (product: IProduct) => {
    setSelectedProduct(product);
    deleteModal.openModal();
  };

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
          <ProductTable
            data={products}
            onEdit={handleEdit}
            onDelete={handleDelete}
          />
        </ComponentCard>
      </div>

      <ProductModal
        isOpen={createModal.isOpen}
        onClose={createModal.closeModal}
        onSubmit={(data) => {
          const newProduct: IProduct = {
            id: Date.now(),
            name: data.name || "Default Name",
            categoryId: data.categoryId || 0,
            description: data.description || "Default Description",
            price: data.price || 0,
            images: data.images || [],
            createdAt: new Date().toISOString(),
            updatedAt: new Date().toISOString(),
          };
          setProducts((prev) => [...prev, newProduct]);
          createModal.closeModal();
        }}
      />

      <ProductModal
        isOpen={editModal.isOpen}
        onClose={editModal.closeModal}
        isEdit
        initialData={selectedProduct || {}}
        onSubmit={(data) => {
          if (!selectedProduct) return;
          setProducts((prev) =>
            prev.map((item) =>
              item.id === selectedProduct.id
                ? { ...item, ...data, updatedAt: new Date().toISOString() }
                : item
            )
          );
          editModal.closeModal();
        }}
      />

      <ConfirmDeleteModal
        isOpen={deleteModal.isOpen}
        onClose={deleteModal.closeModal}
        onConfirm={() => {
          if (!selectedProduct) return;
          setProducts((prev) =>
            prev.filter((item) => item.id !== selectedProduct.id)
          );
          deleteModal.closeModal();
        }}
        objectType="sản phẩm"
        targetLabel={selectedProduct?.name}
      />
    </div>
  );
}
