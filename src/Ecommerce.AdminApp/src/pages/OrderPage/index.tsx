import PageBreadcrumb from "@/components/common/PageBreadCrumb";
import PageMeta from "@/components/common/PageMeta";
import { useLocation, useNavigate } from "react-router-dom";
import OrderCard from "../../components/order/OrderCard";
import { mockOrders } from "../../data/order";

export default function OrderListPage() {
  const location = useLocation();
  const navigate = useNavigate();

  const queryParams = new URLSearchParams(location.search);
  const customerId = queryParams.get("customerId");

  return (
    <div>
      <PageMeta
        title="Đơn hàng khách hàng | Admin - Tiệm Bánh Ngọt"
        description="Trang danh sách đơn hàng của khách hàng."
      />
      <PageBreadcrumb pageTitle="Đơn hàng khách hàng" />

      <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 gap-4 mt-6">
        {mockOrders.map((order) => (
          <OrderCard
            key={order.id}
            id={order.id}
            date={order.date}
            customerName={order.customer.name}
            total={order.total}
            status={order.status}
            onClick={() => navigate(`/admin/orders/${order.id}`)}
          />
        ))}
      </div>
    </div>
  );
}
