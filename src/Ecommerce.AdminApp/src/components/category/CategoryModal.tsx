import Input from "@/components/form/input/InputField";
import TextArea from "@/components/form/input/TextArea";
import Label from "@/components/form/Label";
import Button from "@/components/ui/button/Button";
import { Modal } from "@/components/ui/modal";
import { ICategory, ICreateCategory, IUpdateCategory } from "@/types/types";
import { message } from "antd";
import { useEffect, useState } from "react";
import { z } from "zod";

interface CategoryModalProps {
  isOpen: boolean;
  onClose: () => void;
  isEdit?: boolean;
  initialData?: Partial<ICategory>;
  onCreate?: (data: ICreateCategory) => void;
  onEdit?: (data: IUpdateCategory) => void;
  isLoading?: boolean;
}

// Schema validation for ICreateCategory and IUpdateCategory
const CategoryFormSchema = z.object({
  name: z.string().min(1, "Tên danh mục không được để trống"),
  description: z.string().min(1, "Mô tả không được để trống"),
});

export default function CategoryModal({
  isOpen,
  onClose,
  isEdit = false,
  initialData,
  onCreate,
  onEdit,
  isLoading = false,
}: CategoryModalProps) {
  const [name, setName] = useState("");
  const [description, setDescription] = useState("");
  const [errors, setErrors] = useState<Record<string, string>>({});

  useEffect(() => {
    if (isEdit && initialData) {
      setName(initialData.name || "");
      setDescription(initialData.description || "");
    } else {
      setName("");
      setDescription("");
    }
    setErrors({});
  }, [isEdit, initialData]);

  const handleSubmit = () => {
    const data: ICreateCategory | IUpdateCategory = {
      name,
      description,
    };

    // Validate data
    const result = CategoryFormSchema.safeParse(data);
    if (!result.success) {
      const fieldErrors: Record<string, string> = {};
      result.error.errors.forEach((err) => {
        if (err.path[0]) {
          fieldErrors[err.path[0]] = err.message;
        }
      });
      setErrors(fieldErrors);
      message.error("Vui lòng kiểm tra lại thông tin");
      return;
    }

    if (isEdit && onEdit) {
      onEdit(data as IUpdateCategory);
    } else if (!isEdit && onCreate) {
      onCreate(data as ICreateCategory);
    }

    onClose();
  };

  if (isLoading) {
    return (
      <Modal isOpen={isOpen} onClose={onClose} className="max-w-xl w-full p-6">
        <div className="flex justify-center items-center h-40">
          <p>Loading category data...</p>
        </div>
      </Modal>
    );
  }

  return (
    <Modal isOpen={isOpen} onClose={onClose} className="max-w-xl w-full p-6">
      <div className="px-2 pr-14">
        <h4 className="mb-2 text-2xl font-semibold text-gray-800 dark:text-white/90">
          {isEdit ? "Chỉnh sửa danh mục" : "Thêm danh mục mới"}
        </h4>
        <p className="mb-6 text-sm text-gray-500 dark:text-gray-400">
          {isEdit
            ? "Cập nhật thông tin danh mục"
            : "Điền thông tin để tạo danh mục mới."}
        </p>
      </div>

      <div className="space-y-4">
        <div>
          <Label htmlFor="name">Tên danh mục</Label>
          <Input
            id="name"
            value={name}
            onChange={(e) => setName(e.target.value)}
            placeholder="Nhập tên danh mục"
            error={!!errors.name}
          />
          {errors.name && (
            <p className="mt-1 text-sm text-red-500">{errors.name}</p>
          )}
        </div>

        <div>
          <Label htmlFor="description">Mô tả</Label>
          <TextArea
            placeholder="Nhập mô tả danh mục"
            value={description}
            onChange={(val) => setDescription(val)}
            error={!!errors.description}
          />
          {errors.description && (
            <p className="mt-1 text-sm text-red-500">{errors.description}</p>
          )}
        </div>
      </div>

      <div className="flex items-center justify-end gap-3 mt-4">
        <Button size="sm" variant="outline" onClick={onClose}>
          Hủy
        </Button>
        <Button size="sm" onClick={handleSubmit}>
          {isEdit ? "Lưu thay đổi" : "Tạo mới"}
        </Button>
      </div>
    </Modal>
  );
}
