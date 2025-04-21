import PageBreadcrumb from "@/components/common/PageBreadCrumb";
import PageMeta from "@/components/common/PageMeta";
import OrderDetailCard from "@/components/order/OrderDetailCard";
import { mockOrders } from "@/data/order";
import { useParams } from "react-router-dom";

export default function OrderDetailPage() {
  const { orderId } = useParams();
  const order = mockOrders.find((o) => o.id === orderId);

  if (!order)
    return <p className="text-center p-8">Không tìm thấy đơn hàng.</p>;

  return (
    <div>
      <PageMeta
        title={`Chi tiết đơn hàng ${order.id} | Admin - Tiệm Bánh Ngọt`}
        description={`Trang chi tiết đơn hàng mã ${order.id}`}
      />
      <PageBreadcrumb pageTitle={`Đơn hàng #${order.id}`} />

      <div className="mt-6">
        <OrderDetailCard {...order} />
      </div>
    </div>
  );
}
