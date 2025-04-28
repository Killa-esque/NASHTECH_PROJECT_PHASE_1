import {
  CreateProductDto,
  ProductResponseDto,
  UpdateProductDto,
} from "@/api/product/productTypes";
import FileInput from "@/components/form/input/FileInput";
import Input from "@/components/form/input/InputField";
import TextArea from "@/components/form/input/TextArea";
import Label from "@/components/form/Label";
import Select from "@/components/form/Select";
import Button from "@/components/ui/button/Button";
import { Modal } from "@/components/ui/modal";
import { useEffect, useState } from "react";

interface ProductModalProps {
  isOpen: boolean;
  onClose: () => void;
  isEdit?: boolean;
  initialData?: Partial<ProductResponseDto>;
  onCreate?: (data: CreateProductDto) => void; // For create mode
  onEdit?: (data: UpdateProductDto) => void; // For edit mode
}

const categoryOptions = [
  { value: "1", label: "Bánh kem" },
  { value: "2", label: "Bánh mặn" },
  { value: "3", label: "Bánh ngọt" },
];

export default function ProductModal({
  isOpen,
  onClose,
  isEdit = false,
  initialData,
  onCreate,
  onEdit,
}: ProductModalProps) {
  const [name, setName] = useState("");
  const [description, setDescription] = useState("");
  const [price, setPrice] = useState(0);
  const [categoryId, setCategoryId] = useState("1");
  const [imageUrl, setImageUrl] = useState("");

  useEffect(() => {
    if (isEdit && initialData) {
      setName(initialData.name || "");
      setDescription(initialData.description || "");
      setPrice(initialData.price || 0);
      setCategoryId(initialData.categoryId || "1");
      setImageUrl(initialData.imageUrl || "");
    } else {
      // Reset form when creating a new product
      setName("");
      setDescription("");
      setPrice(0);
      setCategoryId("1");
      setImageUrl("");
    }
  }, [isEdit, initialData]);

  const handleSubmit = () => {
    const data: CreateProductDto | UpdateProductDto = {
      name,
      description,
      price,
      categoryId,
      imageUrl,
      stock: isEdit ? initialData?.stock : 0,
    };

    if (isEdit && onEdit) {
      onEdit(data as UpdateProductDto);
    } else if (!isEdit && onCreate) {
      onCreate(data as CreateProductDto);
    }

    onClose();
  };

  const handleFileChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    const files = e.target.files;
    if (files && files.length > 0) {
      const fileUrl = URL.createObjectURL(files[0]);
      setImageUrl(fileUrl);
    }
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
          <FileInput onChange={handleFileChange} />
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
