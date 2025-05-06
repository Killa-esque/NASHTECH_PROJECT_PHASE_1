import ComponentCard from "@/components/common/ComponentCard";
import PageBreadcrumb from "@/components/common/PageBreadCrumb";
import PageMeta from "@/components/common/PageMeta";
import CustomerDetailModal from "@/components/customer/CustomerDetailModal";
import CustomerModal from "@/components/customer/CustomerModal";
import CustomerTable from "@/components/customer/CustomerTable";
import Button from "@/components/ui/button/Button";
import ConfirmDeleteModal from "@/components/ui/modal/ConfirmDeleteModal";
import { useCustomer } from "@/hooks/useCustomer";
import { useModal } from "@/hooks/useModal";
import { ICreateCustomer, ICustomer, IUpdateCustomer } from "@/types/types";
import { message, Pagination } from "antd";
import { useEffect, useState } from "react";
import { useNavigate } from "react-router-dom";

export default function CustomerPage() {
  const navigate = useNavigate();
  const {
    useCustomers,
    useCustomerById,
    useCreateCustomer,
    useUpdateCustomer,
    useDeleteCustomer,
  } = useCustomer();
  const [pageIndex, setPageIndex] = useState(1);
  const pageSize = 10;
  const {
    data: customersResponse,
    isLoading: isCustomersLoading,
    isFetching: isCustomersFetching,
    error: customersError,
  } = useCustomers(pageIndex, pageSize);
  const customers = customersResponse?.items || [];
  const totalCount = customersResponse?.totalCount || 0;
  const createCustomerMutation = useCreateCustomer();
  const updateCustomerMutation = useUpdateCustomer();
  const deleteCustomerMutation = useDeleteCustomer();

  const createModal = useModal();
  const editModal = useModal();
  const deleteModal = useModal();
  const detailModal = useModal();

  const [selectedCustomerId, setSelectedCustomerId] = useState<string | null>(
    null
  );
  const [selectedCustomer, setSelectedCustomer] = useState<ICustomer | null>(
    null
  );

  // Fetch customer data when selectedCustomerId changes (for edit/view)
  const {
    data: customerData,
    isLoading: isCustomerLoading,
    error: customerError,
  } = useCustomerById(selectedCustomerId || "", {
    enabled: !!selectedCustomerId, // Only fetch if selectedCustomerId is set
    refetchOnMount: true,
  });

  useEffect(() => {
    if (customerData) {
      setSelectedCustomer(customerData);
    }
  }, [customerData]);

  useEffect(() => {
    if (customersError) {
      message.error(`Failed to fetch customers: ${customersError.message}`);
    }
    if (customerError) {
      message.error(`Failed to fetch customer: ${customerError.message}`);
    }
  }, [customersError, customerError]);

  const handleCreate = () => {
    setSelectedCustomer(null);
    setSelectedCustomerId(null);
    createModal.openModal();
  };

  const handleView = (customer: ICustomer) => {
    setSelectedCustomerId(customer.id);
    detailModal.openModal();
  };

  const handleEdit = (customer: ICustomer) => {
    setSelectedCustomerId(customer.id); // chỉ set ID
    editModal.openModal();
  };

  const handleDelete = (customer: ICustomer) => {
    setSelectedCustomer(customer);
    setSelectedCustomerId(null);
    deleteModal.openModal();
  };

  const handleViewOrderDetail = (customer: ICustomer) => {
    navigate(`/admin/orders?customerId=${customer.id}`);
  };

  const handleCreateSubmit = async (data: ICreateCustomer) => {
    try {
      await createCustomerMutation.mutateAsync(data);
      createModal.closeModal();
      message.success("Customer created successfully");
    } catch (err) {
      message.error("Failed to create customer");
    }
  };

  const handleEditSubmit = async (data: IUpdateCustomer) => {
    if (!selectedCustomer) return;
    try {
      await updateCustomerMutation.mutateAsync({
        id: selectedCustomer.id,
        data,
      });
      setSelectedCustomer({ ...selectedCustomer, ...data });
      editModal.closeModal();
      message.success("Customer updated successfully");
    } catch (err) {
      message.error("Failed to update customer");
    }
  };

  const handleDeleteConfirm = async () => {
    if (!selectedCustomer) return;
    try {
      await deleteCustomerMutation.mutateAsync(selectedCustomer.id);
      deleteModal.closeModal();
      setSelectedCustomer(null);
      setSelectedCustomerId(null);
      message.success("Customer deleted successfully");
    } catch (err) {
      message.error("Failed to delete customer");
    }
  };

  if (isCustomersLoading || isCustomersFetching) {
    return <div>Loading...</div>;
  }

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
            <Button size="sm" onClick={handleCreate}>
              + Thêm khách hàng mới
            </Button>
          }
        >
          {customers.length === 0 ? (
            <div className="py-4 text-center text-gray-500">
              Chưa có khách hàng nào. Nhấn "Thêm khách hàng" để tạo mới.
            </div>
          ) : (
            <>
              <CustomerTable
                data={customers}
                onEdit={handleEdit}
                onDelete={handleDelete}
                onView={handleView}
                onViewOrderDetail={handleViewOrderDetail}
              />
              <Pagination
                current={pageIndex}
                total={totalCount}
                onChange={setPageIndex}
              />
            </>
          )}
        </ComponentCard>
      </div>

      <CustomerModal
        isOpen={createModal.isOpen}
        onClose={createModal.closeModal}
        initialData={{
          id: "",
          userName: "",
          email: "",
          fullName: "",
          phoneNumber: "",
          gender: "Other",
          defaultAddress: "",
          avatarUrl: "",
          allergyNotes: "",
        }}
        onSubmit={(data) => {
          if (createModal.isOpen) {
            handleCreateSubmit(data as ICreateCustomer);
          }
        }}
        isCreate={true}
      />

      {selectedCustomer && (
        <CustomerModal
          isOpen={editModal.isOpen}
          onClose={() => {
            editModal.closeModal();
            setSelectedCustomer(null);
            setSelectedCustomerId(null);
          }}
          initialData={selectedCustomer}
          onSubmit={handleEditSubmit}
          isLoading={isCustomerLoading}
        />
      )}

      {selectedCustomer && (
        <CustomerDetailModal
          isOpen={detailModal.isOpen}
          onClose={() => {
            detailModal.closeModal();
            setSelectedCustomer(null);
            setSelectedCustomerId(null);
          }}
          initialData={selectedCustomer}
        />
      )}

      <ConfirmDeleteModal
        isOpen={deleteModal.isOpen}
        onClose={() => {
          deleteModal.closeModal();
          setSelectedCustomer(null);
          setSelectedCustomerId(null);
        }}
        onConfirm={handleDeleteConfirm}
        objectType="khách hàng"
        targetLabel={selectedCustomer?.fullName}
      />
    </div>
  );
}
