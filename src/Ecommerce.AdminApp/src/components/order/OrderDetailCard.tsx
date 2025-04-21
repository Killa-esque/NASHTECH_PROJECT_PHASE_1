import { IOrderProduct } from "@/types/order";

interface OrderDetailCardProps {
  id: string; // ✅ thay vì orderId
  date: string;
  customer: {
    name: string;
    phone: string;
    address: string;
  };
  products: IOrderProduct[];
  total: number;
  status: "pending" | "processing" | "completed" | "cancelled";
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
  return (
    <div className="space-y-6 rounded-2xl border border-gray-200 p-6 bg-white dark:border-gray-800 dark:bg-white/[0.03]">
      <div className="flex justify-between items-center">
        <h3 className="text-xl font-semibold text-gray-800 dark:text-white">
          Đơn hàng #{id}
        </h3>
        <span className="text-sm text-gray-500">{date}</span>
      </div>

      <div>
        <h4 className="text-lg font-medium mb-2">Khách hàng</h4>
        <p className="text-sm text-gray-700 dark:text-gray-300">
          {customer.name} — {customer.phone}
        </p>
        <p className="text-sm text-gray-500">{customer.address}</p>
      </div>

      <div>
        <h4 className="text-lg font-medium mb-2">Sản phẩm</h4>
        <ul className="space-y-2">
          {products.map((p, i) => (
            <li key={i} className="text-sm text-gray-700 dark:text-gray-300">
              {p.name} x{p.quantity} — {p.price.toLocaleString()}đ
            </li>
          ))}
        </ul>
      </div>

      <div className="text-right text-lg font-semibold text-gray-800 dark:text-white">
        Tổng: {total.toLocaleString()}đ
      </div>

      {note && (
        <div>
          <h4 className="text-sm font-medium text-gray-600 dark:text-gray-300">
            Ghi chú
          </h4>
          <p className="text-sm text-gray-500 italic">{note}</p>
        </div>
      )}
    </div>
  );
}
