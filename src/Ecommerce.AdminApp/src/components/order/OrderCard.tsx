interface OrderCardProps {
  id: string;
  date: string;
  customerName: string;
  total: number;
  status: "pending" | "processing" | "completed" | "cancelled";
  onClick?: () => void;
}

export default function OrderCard({
  id,
  date,
  customerName,
  total,
  status,
  onClick,
}: OrderCardProps) {
  return (
    <div
      onClick={onClick}
      className="cursor-pointer border border-gray-200 rounded-xl p-4 hover:shadow-md dark:border-white/[0.05] bg-white dark:bg-white/[0.03]"
    >
      <div className="flex justify-between items-center mb-2">
        <h4 className="font-semibold text-gray-800 dark:text-white">#{id}</h4>
        <span className="text-xs text-gray-500 dark:text-gray-400">{date}</span>
      </div>
      <p className="text-sm text-gray-600 dark:text-gray-300">
        Khách hàng: <span className="font-medium">{customerName}</span>
      </p>
      <p className="text-sm text-gray-600 dark:text-gray-300">
        Tổng: <span className="font-semibold">{total.toLocaleString()}đ</span>
      </p>
      <span
        className={`inline-block mt-2 text-xs font-medium px-2 py-1 rounded bg-opacity-10 ${
          status === "completed"
            ? "text-green-600 bg-green-500"
            : status === "pending"
            ? "text-yellow-600 bg-yellow-500"
            : status === "cancelled"
            ? "text-red-600 bg-red-500"
            : "text-blue-600 bg-blue-500"
        }`}
      >
        {status}
      </span>
    </div>
  );
}
