import Input from "@/components/form/input/InputField";
import Label from "@/components/form/Label";
import Button from "@/components/ui/button/Button";
import { Modal } from "@/components/ui/modal";
import { useState } from "react";

interface CategoryModalProps {
  isOpen: boolean;
  onClose: () => void;
  onSubmit: (data: { name: string; description: string }) => void;
  isEdit?: boolean;
  initialData?: { name: string; description: string };
}

export default function CategoryModal({
  isOpen,
  onClose,
  onSubmit,
  isEdit = false,
  initialData,
}: CategoryModalProps) {
  const [name, setName] = useState(initialData?.name || "");
  const [description, setDescription] = useState(
    initialData?.description || ""
  );

  const handleSubmit = () => {
    onSubmit({ name, description });
    onClose();
  };

  return (
    <Modal isOpen={isOpen} onClose={onClose} className="max-w-xl w-full p-6">
      <div className="px-2 pr-14">
        <h4 className="mb-2 text-2xl font-semibold text-gray-800 dark:text-white/90">
          {isEdit ? "Chỉnh sửa danh mục" : "Thêm danh mục mới"}
        </h4>
        <p className="mb-6 text-sm text-gray-500 dark:text-gray-400">
          {isEdit
            ? "Cập nhật thông tin danh mục sản phẩm"
            : "Điền thông tin để tạo danh mục mới."}
        </p>
      </div>
      <form className="flex flex-col gap-5">
        <div>
          <Label>Tên danh mục</Label>
          <Input
            type="text"
            value={name}
            onChange={(e) => setName(e.target.value)}
            placeholder="Nhập tên danh mục"
          />
        </div>
        <div>
          <Label>Mô tả</Label>
          <Input
            type="text"
            value={description}
            onChange={(e) => setDescription(e.target.value)}
            placeholder="Nhập mô tả"
          />
        </div>
        <div className="flex items-center justify-end gap-3 mt-4">
          <Button size="sm" variant="outline" onClick={onClose}>
            Hủy
          </Button>
          <Button size="sm" onClick={handleSubmit}>
            {isEdit ? "Lưu thay đổi" : "Tạo mới"}
          </Button>
        </div>
      </form>
    </Modal>
  );
}
