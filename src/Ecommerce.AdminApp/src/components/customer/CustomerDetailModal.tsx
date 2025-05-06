import OrderCard from "@/components/order/OrderCard";
import Button from "@/components/ui/button/Button";
import { Modal } from "@/components/ui/modal";
import { useCustomer } from "@/hooks/useCustomer";
import { useModal } from "@/hooks/useModal";
import { useOrder } from "@/hooks/useOrder";
import { ICustomer, IUpdateCustomer } from "@/types/types";
import { message, Pagination, Spin } from "antd";
import { useEffect, useState } from "react";
import { useNavigate } from "react-router-dom";
import CustomerEditModal from "./CustomerModal";

type Props = {
  isOpen: boolean;
  onClose: () => void;
  initialData: ICustomer;
};

export default function CustomerDetailModal({
  isOpen,
  onClose,
  initialData,
}: Props) {
  const navigate = useNavigate();
  const { useUpdateCustomer } = useCustomer();
  const { useOrders } = useOrder();
  const updateCustomerMutation = useUpdateCustomer();
  const [pageIndex, setPageIndex] = useState(1);
  const pageSize = 10;

  const {
    data: ordersResponse,
    isLoading: ordersLoading,
    error: ordersError,
  } = useOrders(pageIndex, pageSize, initialData.id);

  const orders = ordersResponse?.items || [];
  const totalOrders = ordersResponse?.totalCount || 0;
  const totalSpent = orders.reduce(
    (sum, order) => sum + (order.totalAmount || 0),
    0
  );

  const [customerData, setCustomerData] = useState<ICustomer>(initialData);
  const editModal = useModal();

  useEffect(() => {
    if (ordersError) {
      message.error(`Failed to fetch orders: ${ordersError.message}`);
    }
  }, [ordersError]);

  const handleEditSubmit = async (data: IUpdateCustomer) => {
    try {
      const response = await updateCustomerMutation.mutateAsync({
        id: customerData.id,
        data,
      });
      setCustomerData({ ...customerData, ...response.data });
      editModal.closeModal();
      message.success("Cập nhật khách hàng thành công");
    } catch (err) {
      message.error("Cập nhật khách hàng thất bại");
    }
  };

  if (!customerData) return null;

  return (
    <>
      <Modal isOpen={isOpen} onClose={onClose} className="max-w-4xl w-full p-6">
        <div className="space-y-6">
          <div>
            <h3 className="text-xl font-semibold text-gray-800 dark:text-white">
              Thông tin khách hàng
            </h3>
            <div className="mt-4 space-y-2">
              <p>
                <strong>Họ tên:</strong> {customerData.fullName}
              </p>
              <p>
                <strong>Email:</strong> {customerData.email}
              </p>
              <p>
                <strong>SĐT:</strong> {customerData.phoneNumber}
              </p>
              <p>
                <strong>Địa chỉ:</strong> {customerData.defaultAddress}
              </p>
              <p>
                <strong>Giới tính:</strong> {customerData.gender}
              </p>
              <p>
                <strong>Ngày sinh:</strong> {customerData.dateOfBirth || "-"}
              </p>
              <p>
                <strong>Ghi chú dị ứng:</strong>{" "}
                {customerData.allergyNotes || "-"}
              </p>
              {customerData.avatarUrl && (
                <img
                  src={customerData.avatarUrl}
                  alt="Avatar"
                  className="h-16 w-16 object-cover rounded mt-2"
                />
              )}
            </div>
          </div>

          <div>
            <h3 className="text-xl font-semibold text-gray-800 dark:text-white">
              Tổng quan đơn hàng
            </h3>
            <div className="mt-4 space-y-2">
              <p>
                <strong>Tổng số đơn hàng:</strong> {totalOrders}
              </p>
              <p>
                <strong>Tổng chi tiêu:</strong> {totalSpent.toLocaleString()}đ
              </p>
            </div>
          </div>

          <div>
            <h3 className="text-xl font-semibold text-gray-800 dark:text-white">
              Danh sách đơn hàng
            </h3>
            {ordersLoading ? (
              <div className="text-center py-4">
                <Spin tip="Đang tải đơn hàng..." />
              </div>
            ) : orders.length === 0 ? (
              <p className="text-gray-500 py-4">Không có đơn hàng nào.</p>
            ) : (
              <div className="space-y-4 mt-4">
                <div className="grid grid-cols-1 sm:grid-cols-2 gap-4">
                  {orders.map((order) => (
                    <OrderCard
                      key={order.id}
                      order={order}
                      customerName={customerData.fullName}
                      onClick={() => navigate(`/admin/orders/${order.id}`)}
                    />
                  ))}
                </div>
                {totalOrders > pageSize && (
                  <div className="flex justify-center mt-4">
                    <Pagination
                      current={pageIndex}
                      total={totalOrders}
                      pageSize={pageSize}
                      onChange={setPageIndex}
                      showSizeChanger={false}
                    />
                  </div>
                )}
              </div>
            )}
          </div>

          <div className="flex justify-end">
            <Button onClick={editModal.openModal}>Chỉnh sửa</Button>
          </div>
        </div>
      </Modal>

      <CustomerEditModal
        isOpen={editModal.isOpen}
        onClose={editModal.closeModal}
        initialData={customerData}
        onSubmit={handleEditSubmit}
        isLoading={updateCustomerMutation.isPending}
      />
    </>
  );
}
