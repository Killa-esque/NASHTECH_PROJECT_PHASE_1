import Dropzone from "@/components/form/form-elements/DropZone";
import Input from "@/components/form/input/InputField";
import TextArea from "@/components/form/input/TextArea";
import Label from "@/components/form/Label";
import Select from "@/components/form/Select";
import Button from "@/components/ui/button/Button";
import { Modal } from "@/components/ui/modal";
import { useCategory } from "@/hooks/useCategory";
import { useProduct } from "@/hooks/useProduct";
import {
  ICategory,
  ICreateProduct,
  IProduct,
  IUpdateProduct,
} from "@/types/types";
import { message } from "antd";
import { memo, useCallback, useEffect, useState } from "react";
import { z } from "zod";

interface ProductModalProps {
  isOpen: boolean;
  onClose: () => void;
  isEdit?: boolean;
  initialData?: Partial<IProduct>;
  onCreate?: (data: ICreateProduct) => Promise<string | void>;
  onEdit?: (data: IUpdateProduct) => void;
  isLoading?: boolean;
}

// Schema validation for ICreateProduct and IUpdateProduct
const ProductFormSchema = z.object({
  name: z.string().min(1, "Tên sản phẩm không được để trống"),
  description: z.string().min(1, "Mô tả không được để trống"),
  price: z.number().min(0, "Giá phải lớn hơn hoặc bằng 0"),
  categoryId: z.string().min(1, "Vui lòng chọn danh mục"),
  stock: z
    .number()
    .min(0, "Số lượng tồn kho phải lớn hơn hoặc bằng 0")
    .optional(),
  weight: z.string().optional(),
  ingredients: z.string().optional(),
  expirationDate: z.string().optional(),
  storageInstructions: z.string().optional(),
  allergens: z.string().optional(),
});

function ProductModal({
  isOpen,
  onClose,
  isEdit = false,
  initialData,
  onCreate,
  onEdit,
  isLoading = false,
}: ProductModalProps) {
  const { useCategories } = useCategory();
  const { useUploadProductImages, useDeleteProductImage } = useProduct();
  const { data: categories = { items: [] }, isLoading: categoriesLoading } =
    useCategories();
  const uploadImagesMutation = useUploadProductImages();
  const deleteImageMutation = useDeleteProductImage();

  const [activeTab, setActiveTab] = useState("basic");
  const [name, setName] = useState("");
  const [description, setDescription] = useState("");
  const [price, setPrice] = useState(0);
  const [categoryId, setCategoryId] = useState("");
  const [stock, setStock] = useState(0);
  const [weight, setWeight] = useState("");
  const [ingredients, setIngredients] = useState("");
  const [expirationDate, setExpirationDate] = useState("");
  const [storageInstructions, setStorageInstructions] = useState("");
  const [allergens, setAllergens] = useState("");
  const [newFiles, setNewFiles] = useState<File[]>([]);
  const [existingImages, setExistingImages] = useState<string[]>([]);
  const [errors, setErrors] = useState<Record<string, string>>({});

  useEffect(() => {
    if (isEdit && initialData) {
      setName(initialData.name || "");
      setDescription(initialData.description || "");
      setPrice(initialData.price || 0);
      setCategoryId(initialData.categoryId || "");
      setStock(initialData.stock || 0);
      setWeight(initialData.weight || "");
      setIngredients(initialData.ingredients || "");
      setExpirationDate(initialData.expirationDate || "");
      setStorageInstructions(initialData.storageInstructions || "");
      setAllergens(initialData.allergens || "");
      setExistingImages(initialData.imageUrls || []);
    } else {
      setName("");
      setDescription("");
      setPrice(0);
      setCategoryId(categories.items[0]?.id || "");
      setStock(0);
      setWeight("");
      setIngredients("");
      setExpirationDate("");
      setStorageInstructions("");
      setAllergens("");
      setExistingImages([]);
    }
    setNewFiles([]);
    setErrors({});
  }, [isEdit, initialData, categories]);

  const handleFilesChange = useCallback((files: File[]) => {
    setNewFiles((prev) => [...prev, ...files]);
  }, []);

  const handleRemoveNewFile = useCallback((index: number) => {
    setNewFiles((prev) => prev.filter((_, i) => i !== index));
  }, []);

  const handleRemoveExistingImage = useCallback(
    async (imageUrl: string) => {
      if (isEdit && initialData?.id) {
        try {
          await deleteImageMutation.mutateAsync({
            id: initialData.id,
            imageUrl,
          });
          message.success("Image deleted successfully");
        } catch (err) {
          message.error("Failed to delete image");
        }
      }
    },
    [isEdit, initialData?.id, deleteImageMutation]
  );

  const handleSubmit = useCallback(async () => {
    const data: ICreateProduct | IUpdateProduct = {
      name,
      description,
      price,
      categoryId,
      stock,
      weight: weight || undefined,
      ingredients: ingredients || undefined,
      expirationDate: expirationDate || undefined,
      storageInstructions: storageInstructions || undefined,
      allergens: allergens || undefined,
    };

    // Validate data
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

    try {
      let productId: string | undefined;

      if (isEdit && onEdit && initialData?.id) {
        await onEdit(data as IUpdateProduct);
        productId = initialData.id;
      } else if (!isEdit && onCreate) {
        const result = await onCreate(data as ICreateProduct);
        productId = result as string; // Assuming onCreate returns the new product ID
      }

      // Upload new images if any
      if (newFiles.length > 0 && productId) {
        await uploadImagesMutation.mutateAsync({
          id: productId,
          files: newFiles,
        });
        message.success("Images uploaded successfully");
      }

      onClose();
    } catch (err) {
      message.error("Failed to save product");
    }
  }, [
    name,
    description,
    price,
    categoryId,
    stock,
    weight,
    ingredients,
    expirationDate,
    storageInstructions,
    allergens,
    newFiles,
    isEdit,
    initialData?.id,
    onEdit,
    onCreate,
    uploadImagesMutation,
    onClose,
  ]);

  const categoryOptions = categories.items.map((category: ICategory) => ({
    value: category.id,
    label: category.name,
  }));

  if (isLoading) {
    return (
      <Modal isOpen={isOpen} onClose={onClose} className="max-w-2xl w-full p-6">
        <div className="flex justify-center items-center h-40">
          <p>Loading product data...</p>
        </div>
      </Modal>
    );
  }

  return (
    <Modal isOpen={isOpen} onClose={onClose} className="max-w-2xl w-full p-6">
      <div className="flex flex-col h-[80vh]">
        {/* Header */}
        <div className="px-2 mb-4">
          <h4 className="text-2xl font-semibold text-gray-800 dark:text-white/90">
            {isEdit ? "Chỉnh sửa sản phẩm" : "Thêm sản phẩm mới"}
          </h4>
          <p className="mt-1 text-sm text-gray-500 dark:text-gray-400">
            {isEdit
              ? "Cập nhật thông tin sản phẩm"
              : "Điền thông tin để tạo sản phẩm mới."}
          </p>
        </div>

        {/* Tabs */}
        <div className="flex border-b border-gray-200 dark:border-gray-700">
          <button
            className={`px-4 py-2 text-sm font-medium ${
              activeTab === "basic"
                ? "border-b-2 border-brand-500 text-brand-500"
                : "text-gray-500 hover:text-gray-700 dark:text-gray-400 dark:hover:text-gray-200"
            }`}
            onClick={() => setActiveTab("basic")}
          >
            Thông tin cơ bản
          </button>
          <button
            className={`px-4 py-2 text-sm font-medium ${
              activeTab === "details"
                ? "border-b-2 border-brand-500 text-brand-500"
                : "text-gray-500 hover:text-gray-700 dark:text-gray-400 dark:hover:text-gray-200"
            }`}
            onClick={() => setActiveTab("details")}
          >
            Chi tiết
          </button>
          <button
            className={`px-4 py-2 text-sm font-medium ${
              activeTab === "images"
                ? "border-b-2 border-brand-500 text-brand-500"
                : "text-gray-500 hover:text-gray-700 dark:text-gray-400 dark:hover:text-gray-200"
            }`}
            onClick={() => setActiveTab("images")}
          >
            Hình ảnh
          </button>
        </div>

        {/* Scrollable Content */}
        <div className="flex-1 overflow-y-auto p-4 space-y-4">
          {activeTab === "basic" && (
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
                {errors.name && (
                  <p className="mt-1 text-sm text-red-500">{errors.name}</p>
                )}
              </div>

              <div>
                <Label htmlFor="description">Mô tả</Label>
                <TextArea
                  placeholder="Nhập mô tả sản phẩm"
                  value={description}
                  onChange={(val) => setDescription(val)}
                  error={!!errors.description}
                />
                {errors.description && (
                  <p className="mt-1 text-sm text-red-500">
                    {errors.description}
                  </p>
                )}
              </div>

              <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
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
                  {errors.price && (
                    <p className="mt-1 text-sm text-red-500">{errors.price}</p>
                  )}
                </div>

                <div>
                  <Label htmlFor="stock">Số lượng tồn kho</Label>
                  <Input
                    id="stock"
                    type="number"
                    value={stock}
                    onChange={(e) => setStock(Number(e.target.value))}
                    placeholder="Nhập số lượng tồn kho"
                    error={!!errors.stock}
                  />
                  {errors.stock && (
                    <p className="mt-1 text-sm text-red-500">{errors.stock}</p>
                  )}
                </div>
              </div>

              <div>
                <Label htmlFor="category">Danh mục</Label>
                {categoriesLoading ? (
                  <p>Loading categories...</p>
                ) : (
                  <Select
                    options={categoryOptions}
                    defaultValue={categoryId}
                    onChange={(val) => setCategoryId(val)}
                    className={!!errors.categoryId ? "border-red-500" : ""}
                  />
                )}
                {errors.categoryId && (
                  <p className="mt-1 text-sm text-red-500">
                    {errors.categoryId}
                  </p>
                )}
              </div>
            </div>
          )}

          {activeTab === "details" && (
            <div className="space-y-4">
              <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
                <div>
                  <Label htmlFor="weight">Khối lượng</Label>
                  <Input
                    id="weight"
                    value={weight}
                    onChange={(e) => setWeight(e.target.value)}
                    placeholder="Nhập khối lượng (ví dụ: 500g)"
                    error={!!errors.weight}
                  />
                  {errors.weight && (
                    <p className="mt-1 text-sm text-red-500">{errors.weight}</p>
                  )}
                </div>

                <div>
                  <Label htmlFor="expirationDate">Hạn sử dụng</Label>
                  <Input
                    id="expirationDate"
                    value={expirationDate}
                    onChange={(e) => setExpirationDate(e.target.value)}
                    placeholder="Nhập hạn sử dụng (ví dụ: 30/12/2025)"
                    error={!!errors.expirationDate}
                  />
                  {errors.expirationDate && (
                    <p className="mt-1 text-sm text-red-500">
                      {errors.expirationDate}
                    </p>
                  )}
                </div>
              </div>

              <div>
                <Label htmlFor="ingredients">Thành phần</Label>
                <TextArea
                  placeholder="Nhập thành phần sản phẩm"
                  value={ingredients}
                  onChange={(val) => setIngredients(val)}
                  error={!!errors.ingredients}
                />
                {errors.ingredients && (
                  <p className="mt-1 text-sm text-red-500">
                    {errors.ingredients}
                  </p>
                )}
              </div>

              <div>
                <Label htmlFor="storageInstructions">Hướng dẫn bảo quản</Label>
                <TextArea
                  placeholder="Nhập hướng dẫn bảo quản"
                  value={storageInstructions}
                  onChange={(val) => setStorageInstructions(val)}
                  error={!!errors.storageInstructions}
                />
                {errors.storageInstructions && (
                  <p className="mt-1 text-sm text-red-500">
                    {errors.storageInstructions}
                  </p>
                )}
              </div>

              <div>
                <Label htmlFor="allergens">Chất gây dị ứng</Label>
                <TextArea
                  placeholder="Nhập chất gây dị ứng (nếu có)"
                  value={allergens}
                  onChange={(val) => setAllergens(val)}
                  error={!!errors.allergens}
                />
                {errors.allergens && (
                  <p className="mt-1 text-sm text-red-500">
                    {errors.allergens}
                  </p>
                )}
              </div>
            </div>
          )}

          {activeTab === "images" && (
            <div className="space-y-4">
              <div>
                <Label>Hình ảnh</Label>
                <Dropzone
                  onFilesChange={handleFilesChange}
                  disabled={
                    uploadImagesMutation.isPending ||
                    deleteImageMutation.isPending
                  }
                />
                <div className="mt-2 flex flex-wrap gap-2">
                  {existingImages.map((url, index) => (
                    <div key={index} className="relative">
                      <img
                        src={url}
                        alt={`Existing image ${index + 1}`}
                        className="h-16 w-16 object-cover rounded"
                      />
                      <button
                        onClick={() => handleRemoveExistingImage(url)}
                        className="absolute top-0 right-0 bg-red-500 text-white rounded-full p-1"
                        disabled={deleteImageMutation.isPending}
                      >
                        X
                      </button>
                    </div>
                  ))}
                  {newFiles.map((file, index) => (
                    <div key={index} className="relative">
                      <img
                        src={URL.createObjectURL(file)}
                        alt={`New image ${index + 1}`}
                        className="h-16 w-16 object-cover rounded"
                      />
                      <button
                        onClick={() => handleRemoveNewFile(index)}
                        className="absolute top-0 right-0 bg-red-500 text-white rounded-full p-1"
                      >
                        X
                      </button>
                    </div>
                  ))}
                </div>
              </div>
            </div>
          )}
        </div>

        {/* Sticky Footer */}
        <div className="sticky bottom-0 bg-white dark:bg-gray-900 p-4 border-t border-gray-200 dark:border-gray-700 flex justify-end gap-3">
          <Button size="sm" variant="outline" onClick={onClose}>
            Hủy
          </Button>
          <Button
            size="sm"
            onClick={handleSubmit}
            disabled={uploadImagesMutation.isPending}
          >
            {isEdit ? "Lưu thay đổi" : "Tạo mới"}
          </Button>
        </div>
      </div>
    </Modal>
  );
}

export default memo(ProductModal);
