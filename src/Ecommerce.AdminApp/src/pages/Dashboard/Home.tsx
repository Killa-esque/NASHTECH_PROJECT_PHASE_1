import PageMeta from "@/components/common/PageMeta";
import { useOrder } from "@/hooks/useOrder";
import { useCustomer } from "@/hooks/useCustomer";
import { Spin, message } from "antd";
import { useEffect, useState } from "react";
import { useNavigate } from "react-router-dom";
import { Trophy, Package, Users } from "lucide-react";
import dayjs from "dayjs";

export default function Home() {
  const navigate = useNavigate();
  const { useOrders } = useOrder();
  const { useCustomers } = useCustomer();
  const [fullName, setFullName] = useState<string>("");

  // Fetch metrics
  const {
    data: ordersData,
    isLoading: ordersLoading,
    error: ordersError,
  } = useOrders(1, 5); // Recent 5 orders
  const {
    data: completedOrdersData,
    isLoading: completedOrdersLoading,
    error: completedOrdersError,
  } = useOrders(1, 1000, "Completed"); // All completed orders for revenue
  const {
    data: customersData,
    isLoading: customersLoading,
    error: customersError,
  } = useCustomers(1, 1); // Get total customer count

  // Calculate metrics
  const totalRevenue =
    completedOrdersData?.items.reduce(
      (sum, order) => sum + order.totalAmount,
      0
    ) || 0;
  const totalOrders = ordersData?.totalCount || 0;
  const totalCustomers = customersData?.totalCount || 0;
  const recentOrders = ordersData?.items || [];

  // Fetch fullName from localStorage
  useEffect(() => {
    const userData = localStorage.getItem(
      "oidc.user:https://localhost:5000:admin_client"
    );
    if (userData) {
      try {
        const parsedData = JSON.parse(userData);
        setFullName(parsedData.profile?.name || "Quản trị viên");
      } catch (error) {
        console.error("Failed to parse user data from localStorage", error);
        setFullName("Quản trị viên");
      }
    } else {
      setFullName("Quản trị viên");
    }
  }, []);

  // Handle errors
  useEffect(() => {
    if (ordersError) {
      message.error(`Không tải được đơn hàng: ${ordersError.message}`);
    }
    if (completedOrdersError) {
      message.error(
        `Không tải được doanh thu: ${completedOrdersError.message}`
      );
    }
    if (customersError) {
      message.error(`Không tải được khách hàng: ${customersError.message}`);
    }
  }, [ordersError, completedOrdersError, customersError]);

  // Loading state
  if (ordersLoading || completedOrdersLoading || customersLoading) {
    return (
      <div className="flex justify-center items-center h-64">
        <Spin tip="Đang tải dữ liệu..." />
      </div>
    );
  }

  return (
    <>
      <PageMeta
        title="Tổng Quan Doanh Thu | Admin - Tiệm Bánh Ngọt"
        description="Trang dashboard tổng hợp tình hình kinh doanh cho tiệm bánh ngọt."
      />
      <div className="space-y-6">
        {/* Header */}
        <div className="flex justify-between items-center">
          <h2 className="text-2xl font-bold text-gray-800 dark:text-white">
            Chào {fullName}, hôm nay là {dayjs().format("DD/MM/YYYY")}
          </h2>
        </div>
      </div>
    </>
  );
}
