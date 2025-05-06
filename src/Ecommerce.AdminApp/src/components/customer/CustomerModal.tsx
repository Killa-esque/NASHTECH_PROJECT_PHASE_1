import DatePicker from "@/components/form/date-picker";
import Input from "@/components/form/input/InputField";
import Label from "@/components/form/Label";
import Select from "@/components/form/Select";
import Button from "@/components/ui/button/Button";
import { Modal } from "@/components/ui/modal";
import { ICreateCustomer, ICustomer, IUpdateCustomer } from "@/types/types";
import { message } from "antd";
import dayjs from "dayjs";
import { useState } from "react";
import { z } from "zod";

interface Props {
  isOpen: boolean;
  onClose: () => void;
  initialData: ICustomer;
  onSubmit: (data: ICreateCustomer | IUpdateCustomer) => void;
  isCreate?: boolean;
  isLoading?: boolean;
}

// Validate dateOfBirth in dd-MM-yyyy format
const dateOfBirthSchema = z
  .string()
  .optional()
  .refine(
    (val) => !val || dayjs(val, "DD-MM-YYYY", true).isValid(),
    "Ngày sinh phải có định dạng dd-MM-yyyy (ví dụ: 31-12-1990)"
  );

const genderSchema = z.enum(["Male", "Female", "Other"], {
  errorMap: () => ({ message: "Giới tính phải là Nam, Nữ hoặc Khác" }),
});

const createCustomerSchema = z.object({
  email: z.string().email("Email không hợp lệ"),
  fullName: z.string().min(1, "Họ tên là bắt buộc"),
  dateOfBirth: dateOfBirthSchema,
  phoneNumber: z.string().min(1, "Số điện thoại là bắt buộc"),
  password: z.string().min(6, "Mật khẩu phải có ít nhất 6 ký tự"),
  gender: genderSchema,
  defaultAddress: z.string().min(1, "Địa chỉ là bắt buộc"),
  allergyNotes: z.string().optional(),
});

const updateCustomerSchema = z.object({
  fullName: z.string().min(1, "Họ tên là bắt buộc"),
  dateOfBirth: dateOfBirthSchema,
  phoneNumber: z.string().min(1, "Số điện thoại là bắt buộc"),
  gender: genderSchema,
  defaultAddress: z.string().min(1, "Địa chỉ là bắt buộc"),
  allergyNotes: z.string().optional(),
});

export default function CustomerModal({
  isOpen,
  onClose,
  initialData,
  onSubmit,
  isCreate = false,
  isLoading = false,
}: Props) {
  console.log("CustomerEditModal - initialData:", initialData);

  const initialDateOfBirth = initialData.dateOfBirth
    ? dayjs(initialData.dateOfBirth).format("YYYY-MM-DD")
    : "";
  const [formData, setFormData] = useState<ICustomer>({
    ...initialData,
    dateOfBirth: initialDateOfBirth,
    email: initialData.email || (isCreate ? "" : initialData.email),
    password: isCreate ? "" : undefined,
  });
  const [errors, setErrors] = useState<
    Partial<Record<keyof ICreateCustomer, string>>
  >({});

  // Debug logging to check isCreate and formData.email
  console.log(
    "CustomerEditModal - isCreate:",
    isCreate,
    "formData.email:",
    formData.email,
    "formData.password:",
    formData.password,
    "isLoading:",
    isLoading
  );

  const handleChange = (key: string, value: string) => {
    setFormData((prev: ICustomer) => ({ ...prev, [key]: value }));
    setErrors((prev) => ({ ...prev, [key]: undefined }));
  };

  const validateForm = () => {
    try {
      // Convert dateOfBirth to dd-MM-yyyy for validation
      const dataToValidate = {
        ...formData,
        dateOfBirth: formData.dateOfBirth
          ? dayjs(formData.dateOfBirth, "YYYY-MM-DD").format("DD-MM-YYYY")
          : undefined,
      };
      if (isCreate) {
        createCustomerSchema.parse(dataToValidate);
      } else {
        updateCustomerSchema.parse(dataToValidate);
      }
      return true;
    } catch (err) {
      if (err instanceof z.ZodError) {
        const fieldErrors: Partial<Record<keyof ICreateCustomer, string>> = {};
        err.errors.forEach((error) => {
          if (error.path[0]) {
            fieldErrors[error.path[0] as keyof ICreateCustomer] = error.message;
          }
        });
        setErrors(fieldErrors);
      }
      return false;
    }
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    if (!validateForm()) return;

    try {
      // Convert dateOfBirth to dd-MM-yyyy for backend
      const submitData = {
        ...formData,
        dateOfBirth: formData.dateOfBirth
          ? dayjs(formData.dateOfBirth, "YYYY-MM-DD").format("DD-MM-YYYY")
          : undefined,
      };
      await onSubmit(isCreate ? submitData : submitData);
    } catch (err) {
      message.error("Failed to save customer");
    }
  };

  return (
    <Modal isOpen={isOpen} onClose={onClose} className="max-w-3xl m-4">
      <div className="p-6 space-y-6 bg-white rounded-2xl dark:bg-gray-900">
        <h3 className="text-xl font-semibold text-gray-800 dark:text-white">
          {isCreate ? "Thêm khách hàng mới" : "Chỉnh sửa khách hàng"}
        </h3>
        {isLoading && !isCreate ? (
          <div className="text-center text-gray-500">Đang tải dữ liệu...</div>
        ) : (
          <form className="space-y-6" onSubmit={handleSubmit}>
            <div className="grid grid-cols-2 gap-4">
              <div>
                <Label>Họ và tên</Label>
                <Input
                  type="text"
                  value={formData.fullName}
                  onChange={(e) => handleChange("fullName", e.target.value)}
                  error={!!errors.fullName}
                  disabled={isLoading}
                />
              </div>
              <div>
                <Label>Giới tính</Label>
                <Select
                  options={[
                    { value: "Male", label: "Nam" },
                    { value: "Female", label: "Nữ" },
                    { value: "Other", label: "Khác" },
                  ]}
                  defaultValue={formData.gender}
                  onChange={(value) => handleChange("gender", value)}
                  placeholder="Chọn giới tính"
                  disabled={isLoading}
                  className="w-full"
                />
                {errors.gender && (
                  <p className="mt-1 text-sm text-error-500">{errors.gender}</p>
                )}
              </div>
              <div className="col-span-2">
                <Label>Địa chỉ</Label>
                <Input
                  type="text"
                  value={formData.defaultAddress}
                  onChange={(e) =>
                    handleChange("defaultAddress", e.target.value)
                  }
                  error={!!errors.defaultAddress}
                  disabled={isLoading}
                />
              </div>
              {isCreate && (
                <>
                  <div>
                    <Label>Email</Label>
                    <Input
                      type="email"
                      value={formData.email || ""}
                      onChange={(e) => handleChange("email", e.target.value)}
                      error={!!errors.email}
                      disabled={isLoading}
                    />
                  </div>
                  <div>
                    <Label>Mật khẩu</Label>
                    <Input
                      type="password"
                      value={formData.password || ""}
                      onChange={(e) => handleChange("password", e.target.value)}
                      error={!!errors.password}
                      disabled={isLoading}
                    />
                  </div>
                </>
              )}
              <div>
                <Label>Số điện thoại</Label>
                <Input
                  type="text"
                  value={formData.phoneNumber}
                  onChange={(e) => handleChange("phoneNumber", e.target.value)}
                  error={!!errors.phoneNumber}
                  disabled={isLoading}
                />
              </div>
              <div>
                <DatePicker
                  id="dateOfBirth"
                  label="Ngày sinh"
                  defaultDate={
                    formData.dateOfBirth
                      ? dayjs(formData.dateOfBirth, "YYYY-MM-DD").toDate()
                      : undefined
                  }
                  onChange={(selectedDates) => {
                    const date = selectedDates[0]
                      ? dayjs(selectedDates[0]).format("YYYY-MM-DD")
                      : "";
                    handleChange("dateOfBirth", date);
                  }}
                  placeholder="Chọn ngày sinh"
                />
              </div>
              <div className="col-span-2">
                <Label>Ghi chú dị ứng</Label>
                <Input
                  type="text"
                  value={formData.allergyNotes || ""}
                  onChange={(e) => handleChange("allergyNotes", e.target.value)}
                  error={!!errors.allergyNotes}
                  disabled={isLoading}
                />
              </div>
            </div>

            <div className="flex justify-end gap-3 pt-4">
              <Button
                size="sm"
                variant="outline"
                onClick={onClose}
                disabled={isLoading}
              >
                Hủy
              </Button>
              <Button size="sm" type="submit" disabled={isLoading}>
                {isCreate ? "Tạo mới" : "Lưu thay đổi"}
              </Button>
            </div>
          </form>
        )}
      </div>
    </Modal>
  );
}
