import PageBreadcrumb from "@/components/common/PageBreadCrumb";
import PageMeta from "@/components/common/PageMeta";
import Select from "@/components/form/Select";
import OrderDetailCard from "@/components/order/OrderDetailCard";
import Button from "@/components/ui/button/Button";
import { useCustomer } from "@/hooks/useCustomer";
import { useOrder } from "@/hooks/useOrder";
import { message } from "antd";
import { useEffect, useState } from "react";
import { useParams } from "react-router-dom";

export default function OrderDetailPage() {
  const { orderId } = useParams();
  const { useOrderById, useUpdateOrderStatus, useCancelOrder } = useOrder();
  const { useCustomerById } = useCustomer();
  const updateOrderStatusMutation = useUpdateOrderStatus();
  const cancelOrderMutation = useCancelOrder();

  const {
    data: order,
    isLoading: isOrderLoading,
    error: orderError,
  } = useOrderById(orderId || "");

  const {
    data: customer,
    isLoading: isCustomerLoading,
    error: customerError,
  } = useCustomerById(order?.userId || "", {
    enabled: !!order?.userId,
    refetchOnMount: true,
  });

  const [status, setStatus] = useState<string | undefined>(order?.status);

  useEffect(() => {
    if (order) {
      setStatus(order.status);
    }
  }, [order]);

  useEffect(() => {
    if (orderError) {
      message.error(`Failed to fetch order: ${orderError.message}`);
    }
    if (customerError) {
      message.error(`Failed to fetch customer: ${customerError.message}`);
    }
  }, [orderError, customerError]);

  const handleStatusChange = async (newStatus: string) => {
    if (!orderId) return;
    try {
      await updateOrderStatusMutation.mutateAsync({
        id: orderId,
        data: { orderId, status: newStatus },
      });
      setStatus(newStatus);
      message.success("Cập nhật trạng thái thành công");
    } catch (err) {
      message.error("Cập nhật trạng thái thất bại");
    }
  };

  const handleCancelOrder = async () => {
    if (!orderId) return;
    try {
      await cancelOrderMutation.mutateAsync({ id: orderId });
      setStatus("Cancelled");
      message.success("Hủy đơn hàng thành công");
    } catch (err) {
      message.error("Hủy đơn hàng thất bại");
    }
  };

  if (isOrderLoading || isCustomerLoading) {
    return <div className="text-center p-8">Đang tải...</div>;
  }

  if (!order || !customer) {
    return (
      <p className="text-center p-8">
        Không tìm thấy đơn hàng hoặc khách hàng.
      </p>
    );
  }

  return (
    <div>
      <PageMeta
        title={`Chi tiết đơn hàng ${order.orderCode} | Admin - Tiệm Bánh Ngọt`}
        description={`Trang chi tiết đơn hàng mã ${order.orderCode}`}
      />
      <PageBreadcrumb pageTitle={`Đơn hàng ${customer.fullName}`} />

      <div className="mt-6 space-y-6">
        <div className="flex justify-end gap-4">
          <Select
            options={[
              { value: "Pending", label: "Pending" },
              { value: "Paid", label: "Paid" },
              { value: "Shipped", label: "Shipped" },
              { value: "Completed", label: "Completed" },
              { value: "Cancelled", label: "Cancelled" },
            ]}
            onChange={handleStatusChange}
            className="w-[200px]"
            placeholder="Chọn trạng thái"
            defaultValue={status}
            disabled={status === "Cancelled"}
          />
          {status !== "Cancelled" && (
            <Button
              size="sm"
              variant="outline"
              onClick={handleCancelOrder}
              disabled={cancelOrderMutation.isPending}
            >
              Hủy đơn hàng
            </Button>
          )}
        </div>
        <OrderDetailCard
          id={order.orderCode}
          date={order.createdDate || "-"}
          customer={{
            name: customer.fullName,
            phone: customer.phoneNumber,
            address: order.shippingAddress || customer.defaultAddress,
          }}
          products={order.items || []}
          total={order.totalAmount}
          status={order.status}
          note={order.note}
        />
      </div>
    </div>
  );
}
