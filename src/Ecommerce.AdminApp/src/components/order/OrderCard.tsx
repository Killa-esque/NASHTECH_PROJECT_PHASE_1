import Badge from "@/components/ui/badge/Badge";
import { IOrder } from "@/types/types";
import dayjs from "dayjs";

interface OrderCardProps {
  order: IOrder;
  customerName: string;
  isLoading?: boolean;
  onClick?: () => void;
}

export default function OrderCard({
  order,
  customerName,
  onClick,
  isLoading = false,
}: OrderCardProps) {
  const getStatusColor = (
    status: IOrder["status"]
  ): "warning" | "primary" | "info" | "success" | "error" => {
    switch (status) {
      case "Pending":
        return "warning";
      case "Paid":
        return "primary";
      case "Shipped":
        return "info";
      case "Completed":
        return "success";
      case "Cancelled":
        return "error";
    }
  };

  return (
    <div
      onClick={!isLoading ? onClick : undefined} // Disable click when loading
      className={`cursor-pointer border border-gray-200 rounded-xl p-4 hover:shadow-md dark:border-white/[0.05] bg-white dark:bg-white/[0.03] ${
        isLoading ? "opacity-50 pointer-events-none" : ""
      }`}
    >
      {isLoading ? (
        <div className="flex justify-center items-center h-full">
          <span className="loader" /> {/* Replace with your spinner */}
        </div>
      ) : (
        <div className="space-y-4">
          <h4 className="font-semibold text-gray-800 dark:text-white">
            #{order.orderCode}
          </h4>
          <p className="text-sm text-gray-600 dark:text-gray-300">
            Khách hàng: <span className="font-medium">{customerName}</span>
          </p>
          <p className="text-sm text-gray-600 dark:text-gray-300">
            Địa chỉ giao hàng:{" "}
            <span className="font-medium">
              {order.shippingAddress || "N/A"}
            </span>
          </p>
          <p className="text-sm text-gray-600 dark:text-gray-300">
            Phương thức thanh toán:{" "}
            <span className="font-medium">{order.paymentMethod}</span>
          </p>
          <p className="text-sm text-gray-600 dark:text-gray-300">
            Ghi chú:{" "}
            <span className="font-medium">
              {order.note || "Không có ghi chú"}
            </span>
          </p>
          <p className="text-sm text-gray-600 dark:text-gray-300">
            Ngày giao hàng:{" "}
            <span className="font-medium">
              {order.deliveryDate
                ? dayjs(order.deliveryDate).format("DD/MM/YYYY")
                : "Chưa xác định"}
            </span>
          </p>
          <p className="text-sm text-gray-600 dark:text-gray-300">
            Tổng: <span className="font-semibold">{order.totalAmount}$</span>
          </p>
          <Badge variant="light" color={getStatusColor(order.status)} size="sm">
            {order.status}
          </Badge>
        </div>
      )}
    </div>
  );
}
