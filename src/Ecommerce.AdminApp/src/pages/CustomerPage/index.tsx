import ComponentCard from "@/components/common/ComponentCard";
import PageBreadcrumb from "@/components/common/PageBreadCrumb";
import PageMeta from "@/components/common/PageMeta";
import CustomerDetailModal from "@/components/customer/CustomerDetailModal";
import CustomerEditModal from "@/components/customer/CustomerEditModal";
import CustomerTable from "@/components/customer/CustomerTable";
import Button from "@/components/ui/button/Button";
import ConfirmDeleteModal from "@/components/ui/modal/ConfirmDeleteModal";
import { customerData } from "@/data/customer";
import { useModal } from "@/hooks/useModal";
import { ICustomer } from "@/types/customer";
import { useState } from "react";
import { useNavigate } from "react-router-dom";

export default function Customer() {
  const [selectedCustomer, setSelectedCustomer] = useState<ICustomer | null>(
    null
  );

  const navigate = useNavigate();

  const detailModal = useModal();
  const editModal = useModal();
  const deleteModal = useModal();

  const handleView = (customer: ICustomer) => {
    setSelectedCustomer(customer);
    detailModal.openModal();
  };

  const handleEdit = (customer: ICustomer) => {
    setSelectedCustomer(customer);
    editModal.openModal();
  };

  const handleDelete = (customer: ICustomer) => {
    setSelectedCustomer(customer);
    deleteModal.openModal();
  };

  const handleViewOrderDetail = (customer: ICustomer) => {
    navigate(`/admin/orders?customerId=${customer.id}`);
  };

  return (
    <div>
      <PageMeta
        title="Quản lý khách hàng | Admin - Tiệm Bánh Ngọt"
        description="Trang quản lý thông tin và tương tác của khách hàng trong hệ thống tiệm bánh."
      />
      <PageBreadcrumb pageTitle="Khách hàng" />

      <div className="space-y-6">
        <ComponentCard
          title="Danh sách khách hàng"
          action={
            <Button
              size="sm"
              onClick={() => alert("Chức năng thêm mới đang phát triển")}
            >
              {" "}
              {/* Tuỳ bạn thay thế bằng modal tạo */}+ Thêm khách hàng mới
            </Button>
          }
        >
          <CustomerTable
            data={customerData}
            onEdit={handleEdit}
            onDelete={handleDelete}
            onView={handleView}
            onViewOrderDetail={handleViewOrderDetail}
          />
        </ComponentCard>
      </div>

      {/* Chi tiết khách hàng */}
      {selectedCustomer && (
        <CustomerDetailModal
          isOpen={detailModal.isOpen}
          onClose={detailModal.closeModal}
          initialData={selectedCustomer}
        />
      )}

      {/* Sửa khách hàng */}
      {selectedCustomer && (
        <CustomerEditModal
          isOpen={editModal.isOpen}
          onClose={editModal.closeModal}
          initialData={selectedCustomer}
          onSubmit={(data: ICustomer) => {
            console.log("Edit:", data);
            editModal.closeModal();
          }}
        />
      )}

      {/* Xoá khách hàng */}
      <ConfirmDeleteModal
        isOpen={deleteModal.isOpen}
        onClose={deleteModal.closeModal}
        onConfirm={() => {
          console.log("Delete:", selectedCustomer?.id);
          deleteModal.closeModal();
        }}
        objectType="khách hàng"
        targetLabel={selectedCustomer?.fullName}
      />
    </div>
  );
}
