import {
  Table,
  TableBody,
  TableCell,
  TableHeader,
  TableRow,
} from "@/components/ui/table";
import { IProduct } from "@/types/types";
import { Pagination } from "antd";
import dayjs from "dayjs";
import { memo } from "react";

interface ProductTableProps {
  data: IProduct[];
  onEdit: (product: IProduct) => void;
  onDelete: (product: IProduct) => void;
  onSetFeatured: (product: IProduct, isFeatured: boolean) => void;
  pagination: {
    totalCount: number;
    pageIndex: number;
    pageSize: number;
    onPageChange: (page: number, pageSize: number) => void;
  };
}

function ProductTable({
  data,
  onEdit,
  onDelete,
  onSetFeatured,
  pagination,
}: ProductTableProps) {
  const { totalCount, pageIndex, pageSize, onPageChange } = pagination;

  return (
    <div className="space-y-4">
      <div className="overflow-hidden rounded-xl border border-gray-200 bg-white dark:border-white/[0.05] dark:bg-white/[0.03]">
        <div className="max-w-full overflow-x-auto">
          <Table>
            <TableHeader className="border-b border-gray-100 dark:border-white/[0.05]">
              <TableRow>
                <TableCell
                  isHeader
                  className="px-5 py-3 font-medium text-gray-500 text-start text-theme-xs dark:text-gray-400"
                >
                  Hình ảnh
                </TableCell>
                <TableCell
                  isHeader
                  className="px-5 py-3 font-medium text-gray-500 text-start text-theme-xs dark:text-gray-400"
                >
                  Tên sản phẩm
                </TableCell>
                <TableCell
                  isHeader
                  className="px-5 py-3 font-medium text-gray-500 text-start text-theme-xs dark:text-gray-400"
                >
                  Mô tả
                </TableCell>
                <TableCell
                  isHeader
                  className="px-5 py-3 font-medium text-gray-500 text-start text-theme-xs dark:text-gray-400"
                >
                  Giá
                </TableCell>
                <TableCell
                  isHeader
                  className="px-5 py-3 font-medium text-gray-500 text-start text-theme-xs dark:text-gray-400"
                >
                  Ngày tạo
                </TableCell>
                <TableCell
                  isHeader
                  className="px-5 py-3 font-medium text-gray-500 text-start text-theme-xs dark:text-gray-400"
                >
                  Nổi bật
                </TableCell>
                <TableCell
                  isHeader
                  className="px-5 py-3 font-medium text-gray-500 text-start text-theme-xs dark:text-gray-400"
                >
                  Hành động
                </TableCell>
              </TableRow>
            </TableHeader>
            <TableBody className="divide-y divide-gray-100 dark:divide-white/[0.05]">
              {data.map((product) => (
                <TableRow key={product.id}>
                  <TableCell className="px-5 py-4 sm:px-6 text-start">
                    <div className="flex gap-2">
                      {product.imageUrls.length > 0 ? (
                        product.imageUrls
                          .slice(0, 2)
                          .map((url, index) => (
                            <img
                              key={index}
                              src={url}
                              alt={`${product.name} image ${index + 1}`}
                              className="h-12 w-12 object-cover rounded"
                            />
                          ))
                      ) : (
                        <img
                          src="/images/placeholder.jpg"
                          alt="Placeholder"
                          className="h-12 w-12 object-cover rounded"
                        />
                      )}
                      {product.imageUrls.length > 2 && (
                        <span className="text-gray-500">
                          +{product.imageUrls.length - 2}
                        </span>
                      )}
                    </div>
                  </TableCell>
                  <TableCell className="px-5 py-4 sm:px-6 text-start">
                    <span className="block font-medium text-gray-800 text-theme-sm dark:text-white/90">
                      {product.name}
                    </span>
                  </TableCell>
                  <TableCell className="px-4 py-3 text-gray-500 text-start text-theme-sm dark:text-gray-400">
                    {product.description}
                  </TableCell>
                  <TableCell className="px-4 py-3 text-gray-500 text-theme-sm dark:text-gray-400">
                    {product.price.toLocaleString()}đ
                  </TableCell>
                  <TableCell className="px-4 py-3 text-gray-500 text-theme-sm dark:text-gray-400">
                    {product.createdDate
                      ? dayjs(product.createdDate).format("DD/MM/YYYY")
                      : "N/A"}
                  </TableCell>
                  <TableCell className="px-4 py-3 text-theme-sm text  text-start">
                    <button
                      onClick={() =>
                        onSetFeatured(product, !product.isFeatured)
                      }
                      className={`flex items-center justify-center gap-2 rounded-full border px-4 py-3 text-sm font-medium shadow-theme-xs ${
                        product.isFeatured
                          ? "border-green-300 bg-green-50 text-green-600 hover:bg-green-100 dark:border-green-700 dark:bg-green-900 dark:text-green-400 dark:hover:bg-green-800"
                          : "border-gray-300 bg-white text-gray-700 hover:bg-gray-50 dark:border-gray-700 dark:bg-gray-800 dark:text-gray-400 dark:hover:bg-white/[0.03]"
                      }`}
                    >
                      {product.isFeatured ? (
                        <>
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
                              d="M12 2L15.09 8.26L22 9.27L17 14.14L18.18 21.02L12 17.77L5.82 21.02L7 14.14L2 9.27L8.91 8.26L12 2Z"
                            />
                          </svg>
                          Đang nổi bật
                        </>
                      ) : (
                        <>
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
                              d="M12 2L15.09 8.26L22 9.27L17 14.14L18.18 21.02L12 17.77L5.82 21.02L7 14.14L2 9.27L8.91 8.26L12 2Z"
                            />
                          </svg>
                          Đặt nổi bật
                        </>
                      )}
                    </button>
                  </TableCell>
                  <TableCell className="px-4 py-3 text-theme-sm text-start">
                    <div className="flex items-center gap-2">
                      <button
                        onClick={() => onEdit(product)}
                        className="flex items-center justify-center gap-2 rounded-full border border-gray-300 bg-white px-4 py-3 text-sm font-medium text-gray-700 shadow-theme-xs hover:bg-gray-50 hover:text-gray-800 dark:border-gray-700 dark:bg-gray-800 dark:text-gray-F400 dark:hover:bg-white/[0.03] dark:hover:text-gray-200"
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
                        Edit
                      </button>
                      <button
                        onClick={() => onDelete(product)}
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
      <div className="flex justify-end">
        <Pagination
          current={pageIndex}
          pageSize={pageSize}
          total={totalCount}
          onChange={onPageChange}
          showSizeChanger
          pageSizeOptions={["10", "20", "50"]}
          className="dark:text-gray-400"
        />
      </div>
    </div>
  );
}

export default memo(ProductTable);
