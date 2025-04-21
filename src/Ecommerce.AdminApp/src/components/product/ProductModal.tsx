import FileInput from "@/components/form/input/FileInput";
import Input from "@/components/form/input/InputField";
import TextArea from "@/components/form/input/TextArea";
import Label from "@/components/form/Label";
import Select from "@/components/form/Select";
import Button from "@/components/ui/button/Button";
import { Modal } from "@/components/ui/modal";
import { IProduct } from "@/types/product";
import { useEffect, useState } from "react";

interface ProductModalProps {
  isOpen: boolean;
  onClose: () => void;
  onSubmit: (data: Omit<IProduct, "id" | "createdAt" | "updatedAt">) => void;
  isEdit?: boolean;
  initialData?: Partial<IProduct>;
}

const categoryOptions = [
  { value: "1", label: "Bánh kem" },
  { value: "2", label: "Bánh mặn" },
  { value: "3", label: "Bánh ngọt" },
];

export default function ProductModal({
  isOpen,
  onClose,
  onSubmit,
  isEdit = false,
  initialData,
}: ProductModalProps) {
  const [name, setName] = useState("");
  const [description, setDescription] = useState("");
  const [price, setPrice] = useState(0);
  const [categoryId, setCategoryId] = useState("1");

  useEffect(() => {
    if (isEdit && initialData) {
      setName(initialData.name || "");
      setDescription(initialData.description || "");
      setPrice(initialData.price || 0);
      setCategoryId(initialData.categoryId?.toString() || "1");
    }
  }, [isEdit, initialData]);

  const handleSubmit = () => {
    onSubmit({
      name,
      description,
      price,
      categoryId: parseInt(categoryId, 10),
      images: [],
    });
    onClose();
  };

  return (
    <Modal isOpen={isOpen} onClose={onClose} className="max-w-xl w-full p-6">
      <div className="px-2 pr-14">
        <h4 className="mb-2 text-2xl font-semibold text-gray-800 dark:text-white/90">
          {isEdit ? "Chỉnh sửa sản phẩm" : "Thêm sản phẩm mới"}
        </h4>
        <p className="mb-6 text-sm text-gray-500 dark:text-gray-400">
          {isEdit
            ? "Cập nhật thông tin sản phẩm"
            : "Điền thông tin để tạo sản phẩm mới."}
        </p>
      </div>

      <div className="space-y-4">
        <div>
          <Label htmlFor="name">Tên sản phẩm</Label>
          <Input
            id="name"
            value={name}
            onChange={(e) => setName(e.target.value)}
            placeholder="Nhập tên sản phẩm"
          />
        </div>

        <div>
          <Label htmlFor="description">Mô tả</Label>
          <TextArea
            placeholder="Nhập mô tả sản phẩm"
            value={description}
            onChange={(val) => setDescription(val)}
          />
        </div>

        <div>
          <Label htmlFor="price">Giá</Label>
          <Input
            id="price"
            type="number"
            value={price}
            onChange={(e) => setPrice(Number(e.target.value))}
            placeholder="Nhập giá sản phẩm"
          />
        </div>

        <div>
          <Label htmlFor="category">Danh mục</Label>
          <Select
            options={categoryOptions}
            defaultValue={categoryId}
            onChange={(val) => setCategoryId(val)}
          />
        </div>

        <div>
          <Label htmlFor="image">Hình ảnh</Label>
          <FileInput onChange={(e) => console.log(e.target.files)} />
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
