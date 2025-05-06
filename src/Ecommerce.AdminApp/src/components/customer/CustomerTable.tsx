import {
  Table,
  TableBody,
  TableCell,
  TableHeader,
  TableRow,
} from "@/components/ui/table";
import { ICustomer } from "@/types/types";

interface CustomerTableProps {
  data: ICustomer[];
  onEdit: (customer: ICustomer) => void;
  onDelete: (customer: ICustomer) => void;
  onView: (customer: ICustomer) => void;
  onViewOrderDetail: (customer: ICustomer) => void;
}

export default function CustomerTable({
  data,
  onEdit,
  onDelete,
  onView,
  onViewOrderDetail,
}: CustomerTableProps) {
  return (
    <div className="overflow-hidden rounded-xl border border-gray-200 bg-white dark:border-white/[0.05] dark:bg-white/[0.03]">
      <div className="max-w-full overflow-x-auto">
        <Table>
          <TableHeader className="border-b border-gray-100 dark:border-white/[0.05]">
            <TableRow>
              <TableCell
                isHeader
                className="px-4 py-2 font-medium text-gray-500 text-start text-theme-xs dark:text-gray-400"
              >
                Tên
              </TableCell>
              <TableCell
                isHeader
                className="px-4 py-2 font-medium text-gray-500 text-start text-theme-xs dark:text-gray-400"
              >
                Email
              </TableCell>
              <TableCell
                isHeader
                className="px-4 py-2 font-medium text-gray-500 text-start text-theme-xs dark:text-gray-400"
              >
                SĐT
              </TableCell>
              <TableCell
                isHeader
                className="px-4 py-2 font-medium text-gray-500 text-start text-theme-xs dark:text-gray-400"
              >
                Địa chỉ
              </TableCell>
              <TableCell
                isHeader
                className="px-4 py-2 font-medium text-gray-500 text-start text-theme-xs dark:text-gray-400"
              >
                Hành động
              </TableCell>
            </TableRow>
          </TableHeader>
          <TableBody className="divide-y divide-gray-100 dark:divide-white/[0.05]">
            {data.map((customer) => (
              <TableRow key={customer.id}>
                <TableCell className="px-5 py-4 sm:px-6 text-start">
                  <span className="block font-medium text-gray-800 text-theme-sm dark:text-white/90">
                    {customer.fullName}
                  </span>
                </TableCell>
                <TableCell className="px-4 py-3 text-gray-500 text-start text-theme-sm dark:text-gray-400">
                  {customer.email || "-"}
                </TableCell>
                <TableCell className="px-4 py-3 text-gray-500 text-start text-theme-sm dark:text-gray-400">
                  {customer.phoneNumber || "-"}
                </TableCell>
                <TableCell className="px-4 py-3 text-gray-500 text-start text-theme-sm dark:text-gray-400">
                  {customer.defaultAddress || "-"}
                </TableCell>
                <TableCell className="px-4 py-3 text-theme-sm text-start">
                  <div className="flex items-center gap-2">
                    <button
                      onClick={() => onView(customer)}
                      className="flex items-center justify-center gap-2 rounded-full border border-gray-300 bg-white px-4 py-3 text-sm font-medium text-blue-500 shadow-theme-xs hover:bg-blue-50 hover:text-blue-600 dark:border-gray-700 dark:bg-gray-800 dark:text-blue-400 dark:hover:bg-white/[0.03] dark:hover:text-blue-300"
                    >
                      <svg
                        className="fill-current"
                        width="18"
                        height="18"
                        viewBox="0 0 24 24"
                        xmlns="http://www.w3.org/2000/svg"
                      >
                        <path
                          fillRule="evenodd"
                          clipRule="evenodd"
                          d="M12 4.5C7.30558 4.5 3.40258 7.221 1.5 11.25C3.40258 15.279 7.30558 18 12 18C16.6944 18 20.5974 15.279 22.5 11.25C20.5974 7.221 16.6944 4.5 12 4.5ZM12 15.75C9.65279 15.75 7.75 13.8472 7.75 11.5C7.75 9.15279 9.65279 7.25 12 7.25C14.3472 7.25 16.25 9.15279 16.25 11.5C16.25 13.8472 14.3472 15.75 12 15.75ZM12 9.25C10.7574 9.25 9.75 10.2574 9.75 11.5C9.75 12.7426 10.7574 13.75 12 13.75C13.2426 13.75 14.25 12.7426 14.25 11.5C14.25 10.2574 13.2426 9.25 12 9.25Z"
                        />
                      </svg>
                      Xem
                    </button>
                    <button
                      onClick={() => onViewOrderDetail(customer)}
                      className="flex items-center justify-center gap-2 rounded-full border border-gray-300 bg-white px-4 py-3 text-sm font-medium text-green-500 shadow-theme-xs hover:bg-green-50 hover:text-green-600 dark:border-gray-700 dark:bg-gray-800 dark:text-green-400 dark:hover:bg-white/[0.03] dark:hover:text-green-300"
                    >
                      <svg
                        className="fill-current"
                        width="18"
                        height="18"
                        viewBox="0 0 24 24"
                        xmlns="http://www.w3.org/2000/svg"
                      >
                        <path
                          fillRule="evenodd"
                          clipRule="evenodd"
                          d="M12 4.5C7.30558 4.5 3.40258 7.221 1.5 11.25C3.40258 15.279 7.30558 18 12 18C16.6944 18 20.5974 15.279 22.5 11.25C20.5974 7.221 16.6944 4.5 12 4.5ZM12 15.75C9.65279 15.75 7.75 13.8472 7.75 11.5C7.75 9.15279 9.65279 7.25 12 7.25C14.3472 7.25 16.25 9.15279 16.25 11.5C16.25 13.8472 14.3472 15.75 12 15.75ZM12 9.25C10.7574 9.25 9.75 10.2574 9.75 11.5C9.75 12.7426 10.7574 13.75 12 13.75C13.2426 13.75 14.25 12.7426 14.25 11.5C14.25 10.2574 13.2426 9.25 12 9.25Z"
                        />
                      </svg>
                      Đơn hàng
                    </button>
                    <button
                      onClick={() => onEdit(customer)}
                      className="flex items-center justify-center gap-2 rounded-full border border-gray-300 bg-white px-4 py-3 text-sm font-medium text-gray-700 shadow-theme-xs hover:bg-gray-50 hover:text-gray-800 dark:border-gray-700 dark:bg-gray-800 dark:text-gray-400 dark:hover:bg-white/[0.03] dark:hover:text-gray-200"
                    >
                      <svg
                        className="fill-current"
                        width="18"
                        height="18"
                        viewBox="0 0 18 18"
                        xmlns="http://www.w3.org/2000/svg"
                      >
                        <path
                          fillRule="evenodd"
                          clipRule="evenodd"
                          d="M15.0911 2.78206C14.2125 1.90338 12.7878 1.90338 11.9092 2.78206L4.57524 10.116C4.26682 10.4244 4.0547 10.8158 3.96468 11.2426L3.31231 14.3352C3.25997 14.5833 3.33653 14.841 3.51583 15.0203C3.69512 15.1996 3.95286 15.2761 4.20096 15.2238L7.29355 14.5714C7.72031 14.4814 8.11172 14.2693 8.42013 13.9609L15.7541 6.62695C16.6327 5.74827 16.6327 4.32365 15.7541 3.44497L15.0911 2.78206ZM12.9698 3.84272C13.2627 3.54982 13.7376 3.54982 14.0305 3.84272L14.6934 4.50563C14.9863 4.79852 14.9863 5.2734 14.6934 5.56629L14.044 6.21573L12.3204 4.49215L12.9698 3.84272ZM11.2597 5.55281L5.6359 11.1766C5.53309 11.2794 5.46238 11.4099 5.43238 11.5522L5.01758 13.5185L6.98394 13.1037C7.1262 13.0737 7.25666 13.003 7.35947 12.9002L12.9833 7.27639L11.2597 5.55281Z"
                        />
                      </svg>
                      Sửa
                    </button>
                    <button
                      onClick={() => onDelete(customer)}
                      className="flex items-center justify-center gap-2 rounded-full border border-gray-300 bg-white px-4 py-3 text-sm font-medium text-red-500 shadow-theme-xs hover:bg-red-50 hover:text-red-600 dark:border-gray-700 dark:bg-gray-800 dark:text-red-400 dark:hover:bg-white/[0.03] dark:hover:text-red-300"
                    >
                      <svg
                        className="fill-current"
                        width="18"
                        height="18"
                        viewBox="0 0 24 24"
                        xmlns="http://www.w3.org/2000/svg"
                      >
                        <path
                          fillRule="evenodd"
                          clipRule="evenodd"
                          d="M6.04289 16.5413C5.65237 16.9318 5.65237 17.565 6.04289 17.9555C6.43342 18.346 7.06658 18.346 7.45711 17.9555L11.9987 13.4139L16.5408 17.956C16.9313 18.3466 17.5645 18.3466 17.955 17.956C18.3455 17.5655 18.3455 16.9323 17.955 16.5418L13.4129 11.9997L17.955 7.4576C18.3455 7.06707 18.3455 6.43391 17.955 6.04338C17.5645 5.65286 16.9313 5.65286 16.5408 6.04338L11.9987 10.5855L7.45711 6.0439C7.06658 5.65338 6.43342 5.65338 6.04289 6.0439C5.65237 6.43442 5.65237 7.06759 6.04289 7.45811L10.5845 11.9997L6.04289 16.5413Z"
                        />
                      </svg>
                      Xoá
                    </button>
                  </div>
                </TableCell>
              </TableRow>
            ))}
          </TableBody>
        </Table>
      </div>
    </div>
  );
}
