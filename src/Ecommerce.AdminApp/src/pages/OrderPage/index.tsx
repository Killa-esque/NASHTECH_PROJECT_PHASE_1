import PageBreadcrumb from "@/components/common/PageBreadCrumb";
import PageMeta from "@/components/common/PageMeta";
import OrderCard from "@/components/order/OrderCard";
import { useCustomer } from "@/hooks/useCustomer";
import { useOrder } from "@/hooks/useOrder";
import { IOrder } from "@/types/types";
import { message, Pagination } from "antd";
import { useEffect, useState } from "react";
import { useLocation, useNavigate } from "react-router-dom";

export default function OrderListPage() {
  const location = useLocation();
  const navigate = useNavigate();
  const { useOrders } = useOrder();
  const [pageIndex, setPageIndex] = useState(1);
const pageSize = 12;

  const queryParams = new URLSearchParams(location.search);
  const customerId = queryParams.get("customerId") || undefined;

  const {
    data: ordersResponse,
    isLoading,
    isFetching,
    error,
  } = useOrders(pageIndex, pageSize, customerId);

  const { useCustomerById } = useCustomer();

  const { data: customer, isLoading: isCustomerLoading } = useCustomerById(
    customerId || "",
    {
      enabled: !!customerId,
      refetchOnMount: true,
    }
  );

  const orders = ordersResponse?.items || [];
  const totalCount = ordersResponse?.totalCount || 0;

  useEffect(() => {
    if (error) {
      message.error(`Failed to fetch orders: ${error.message}`);
    }
  }, [error]);

  if (isLoading || isFetching) {
    return <div className="text-center p-8">Đang tải...</div>;
  }

  return (
    <div>
      <PageMeta
        title="Đơn hàng khách hàng | Admin - Tiệm Bánh Ngọt"
        description="Trang danh sách đơn hàng của khách hàng."
      />
      <PageBreadcrumb
        pageTitle={
          customerId
            ? `Đơn hàng của khách hàng ${customer?.fullName}`
            : "Đơn hàng"
        }
      />

      <div className="mt-6">
        {orders.length === 0 ? (
          <p className="text-center p-8 text-gray-500">
            Không có đơn hàng nào.
          </p>
        ) : (
          <>
            <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 gap-4">
              {orders.map((order: IOrder) => (
                <OrderCard
                  key={order.id}
                  order={order}
                  customerName={customer?.fullName || ""}
                  isLoading={isCustomerLoading}
                  onClick={() => navigate(`/admin/orders/${order.id}`)}
                />
              ))}
            </div>
            <div className="mt-6 flex justify-center">
              <Pagination
                current={pageIndex}
                total={totalCount}
                pageSize={pageSize}
                onChange={setPageIndex}
                showSizeChanger={false}
              />
            </div>
          </>
        )}
      </div>
    </div>
  );
}
