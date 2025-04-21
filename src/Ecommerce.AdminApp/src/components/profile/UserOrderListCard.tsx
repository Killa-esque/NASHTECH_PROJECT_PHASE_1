interface UserOrderListCardProps {
  orders: {
    id: string;
    date: string;
    amount: number;
  }[];
}

export default function UserOrderListCard({ orders }: UserOrderListCardProps) {
  return (
    <div className="p-5 border border-gray-200 rounded-2xl bg-white dark:border-gray-800 dark:bg-gray-900">
      <h4 className="mb-4 text-lg font-semibold text-gray-800 dark:text-white/90">
        Đơn hàng gần đây
      </h4>
      <ul className="space-y-3 text-sm text-gray-600 dark:text-gray-300">
        {orders.length === 0 && (
          <li className="text-gray-400 italic">Chưa có đơn hàng</li>
        )}
        {orders.map((order) => (
          <li key={order.id} className="flex justify-between">
            <span>#{order.id}</span>
            <span>{order.date}</span>
            <span>{order.amount.toLocaleString()}đ</span>
          </li>
        ))}
      </ul>
    </div>
  );
}
