import Button from "@/components/ui/button/Button";
import { Modal } from "@/components/ui/modal";

interface ConfirmDeleteModalProps {
  isOpen: boolean;
  onClose: () => void;
  onConfirm: () => void;
  title?: string; // Mặc định: "Xác nhận xoá"
  description?: string; // Nếu muốn custom đoạn mô tả
  targetLabel?: string; // Tên đối tượng cần xoá (hiển thị đậm)
  objectType?: string; // Loại đối tượng, ví dụ: "sản phẩm", "khách hàng"
}

export default function ConfirmDeleteModal({
  isOpen,
  onClose,
  onConfirm,
  title = "Xác nhận xoá",
  description,
  targetLabel,
  objectType = "mục này",
}: ConfirmDeleteModalProps) {
  return (
    <Modal isOpen={isOpen} onClose={onClose} className="max-w-md w-full p-6">
      <div className="px-2 text-center">
        <h4 className="mb-3 text-xl font-semibold text-gray-800 dark:text-white/90">
          {title}
        </h4>
        <p className="text-sm text-gray-600 dark:text-gray-400">
          {description || (
            <>
              Bạn có chắc chắn muốn xoá {objectType}{" "}
              <span className="font-semibold text-red-500">{targetLabel}</span>?
            </>
          )}
        </p>
        <div className="flex items-center justify-center gap-3 mt-6">
          <Button variant="outline" size="sm" onClick={onClose}>
            Hủy
          </Button>
          <Button variant="primary" size="sm" onClick={onConfirm}>
            Xác nhận xoá
          </Button>
        </div>
      </div>
    </Modal>
  );
}
