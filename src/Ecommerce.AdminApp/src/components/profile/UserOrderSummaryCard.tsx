interface UserOrderSummaryCardProps {
  totalOrders: number;
  totalSpent: number;
}

export default function UserOrderSummaryCard({
  totalOrders,
  totalSpent,
}: UserOrderSummaryCardProps) {
  return (
    <div className="p-5 border border-gray-200 rounded-2xl bg-white dark:border-gray-800 dark:bg-gray-900">
      <h4 className="mb-4 text-lg font-semibold text-gray-800 dark:text-white/90">
        Tổng quan đơn hàng
      </h4>
      <div className="grid grid-cols-2 gap-6 text-center">
        <div>
          <p className="text-sm text-gray-500 dark:text-gray-400">Tổng đơn</p>
          <p className="mt-1 text-2xl font-bold text-gray-800 dark:text-white">
            {totalOrders}
          </p>
        </div>
        <div>
          <p className="text-sm text-gray-500 dark:text-gray-400">
            Tổng chi tiêu
          </p>
          <p className="mt-1 text-2xl font-bold text-gray-800 dark:text-white">
            {totalSpent.toLocaleString()}đ
          </p>
        </div>
      </div>
    </div>
  );
}
