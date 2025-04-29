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
import { message } from "antd";
import { useEffect, useState } from "react";
import { z } from "zod";

interface ProductModalProps {
  isOpen: boolean;
  onClose: () => void;
  isEdit?: boolean;
  initialData?: Partial<ProductResponseDto>;
  onCreate?: (data: CreateProductDto) => void;
  onEdit?: (data: UpdateProductDto) => void;
}

const categoryOptions = [
  { value: "1", label: "Bánh kem" },
  { value: "2", label: "Bánh mặn" },
  { value: "3", label: "Bánh ngọt" },
];

// Schema validation cho CreateProductDto và UpdateProductDto
const ProductFormSchema = z.object({
  name: z.string().min(1, "Tên sản phẩm không được để trống"),
  description: z.string().min(1, "Mô tả không được để trống"),
  price: z.number().min(0, "Giá phải lớn hơn hoặc bằng 0"),
  categoryId: z.string().min(1, "Vui lòng chọn danh mục"),
  imageUrl: z.string().url("Hình ảnh phải là một URL hợp lệ").optional(),
  stock: z
    .number()
    .min(0, "Số lượng tồn kho phải lớn hơn hoặc bằng 0")
    .optional(),
});

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
  const [errors, setErrors] = useState<Record<string, string>>({});

  useEffect(() => {
    if (isEdit && initialData) {
      setName(initialData.name || "");
      setDescription(initialData.description || "");
      setPrice(initialData.price || 0);
      setCategoryId(initialData.categoryId || "1");
      setImageUrl(initialData.imageUrl || "");
    } else {
      setName("");
      setDescription("");
      setPrice(0);
      setCategoryId("1");
      setImageUrl("");
    }
    setErrors({});
  }, [isEdit, initialData]);

  const handleSubmit = () => {
    const data = {
      name,
      description,
      price,
      categoryId,
      imageUrl: imageUrl || undefined,
      stock: isEdit ? initialData?.stock : 0,
    };

    // Validate dữ liệu
    const result = ProductFormSchema.safeParse(data);
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
            error={!!errors.name}
          />
        </div>

        <div>
          <Label htmlFor="description">Mô tả</Label>
          <TextArea
            placeholder="Nhập mô tả sản phẩm"
            value={description}
            onChange={(val) => setDescription(val)}
            error={!!errors.description}
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
            error={!!errors.price}
          />
        </div>

        <div>
          <Label htmlFor="category">Danh mục</Label>
          <Select
            options={categoryOptions}
            defaultValue={categoryId}
            onChange={(val) => setCategoryId(val)}
            className={!!errors.categoryId ? "border-red-500" : ""}
          />
        </div>

        <div>
          <Label htmlFor="image">Hình ảnh</Label>
          <FileInput onChange={handleFileChange} />
          {errors.imageUrl && (
            <p className="mt-1 text-sm text-red-500">{errors.imageUrl}</p>
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
