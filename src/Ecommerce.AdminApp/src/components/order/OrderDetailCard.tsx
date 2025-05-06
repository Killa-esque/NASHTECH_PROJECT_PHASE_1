import Badge from "@/components/ui/badge/Badge";
import { IOrderItem } from "@/types/types";

interface OrderDetailCardProps {
  id: string;
  date: string;
  customer: {
    name: string;
    phone: string;
    address: string;
  };
  products: IOrderItem[];
  total: number;
  status: "Pending" | "Paid" | "Shipped" | "Completed" | "Cancelled";
  note?: string;
}

export default function OrderDetailCard({
  id,
  date,
  customer,
  products,
  total,
  status,
  note,
}: OrderDetailCardProps) {
  const getStatusColor = (
    status: OrderDetailCardProps["status"]
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
    <div className="rounded-2xl border border-gray-200 bg-white p-6 shadow-sm dark:border-gray-800 dark:bg-gray-900">
      {/* Header */}
      <div className="mb-6 flex items-center justify-between">
        <h3 className="text-xl font-bold text-gray-800 dark:text-white">
          $ơn hàng #{id}
        </h3>
        <span className="text-sm text-gray-500 dark:text-gray-400">{date}</span>
      </div>

      {/* Customer Info */}
      <div className="mb-6">
        <h4 className="mb-3 text-lg font-bold text-gray-800 dark:text-white">
          Khách hàng
        </h4>
        <div className="grid gap-2 sm:grid-cols-2">
          <div>
            <p className="text-sm font-medium text-gray-700 dark:text-gray-300">
              {customer.name}
            </p>
            <p className="text-sm text-gray-500 dark:text-gray-400">
              {customer.phone}
            </p>
          </div>
          <p className="text-sm text-gray-500 dark:text-gray-400">
            {customer.address}
          </p>
        </div>
      </div>

      {/* Products */}
      <div className="mb-6">
        <h4 className="mb-3 text-lg font-bold text-gray-800 dark:text-white">
          Sản phẩm
        </h4>
        <div className="divide-y divide-gray-200 dark:divide-gray-700">
          {products.length === 0 ? (
            <p className="py-4 text-sm text-gray-500 dark:text-gray-400">
              Không có sản phẩm.
            </p>
          ) : (
            products.map((p) => (
              <div
                key={p.id}
                className="flex justify-between py-3 transition-colors hover:bg-gray-50 dark:hover:bg-gray-800/50"
              >
                <div className="flex-1">
                  <p className="text-sm font-medium text-gray-700 dark:text-gray-300">
                    {p.productName}
                  </p>
                  <p className="text-sm text-gray-500 dark:text-gray-400">
                    Số lượng: {p.quantity}
                  </p>
                </div>
                <p className="text-sm font-medium text-gray-700 dark:text-gray-300">
                  {p.price.toLocaleString()}$
                </p>
              </div>
            ))
          )}
        </div>
      </div>

      {/* Total */}
      <div className="mb-6 text-right">
        <p className="text-lg font-bold text-gray-800 dark:text-white">
          Tổng: {total.toLocaleString()}$
        </p>
      </div>

      {/* Status */}
      <div className="mb-6">
        <h4 className="mb-3 text-lg font-bold text-gray-800 dark:text-white">
          Trạng thái
        </h4>
        <Badge variant="light" color={getStatusColor(status)} size="sm">
          {status}
        </Badge>
      </div>

      {/* Note */}
      {note && (
        <div>
          <h4 className="mb-3 text-lg font-bold text-gray-800 dark:text-white">
            Ghi chú
          </h4>
          <p className="text-sm italic text-gray-500 dark:text-gray-400">
            {note}
          </p>
        </div>
      )}
    </div>
  );
}
