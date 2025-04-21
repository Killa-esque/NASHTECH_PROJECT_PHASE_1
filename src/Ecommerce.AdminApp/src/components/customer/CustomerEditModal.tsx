import Input from "@/components/form/input/InputField";
import Label from "@/components/form/Label";
import Button from "@/components/ui/button/Button";
import { Modal } from "@/components/ui/modal";
import { useForm } from "@/hooks/useForm";
import { ICustomer } from "@/types/customer";

interface Props {
  isOpen: boolean;
  onClose: () => void;
  initialData: ICustomer;
  onSubmit: (data: ICustomer) => void;
}

export default function CustomerEditModal({
  isOpen,
  onClose,
  initialData,
  onSubmit,
}: Props) {
  const { formData, handleChange, handleSubmit } = useForm<ICustomer>({
    initialValues: initialData,
    onSubmit: (data) => {
      onSubmit(data);
    },
  });

  return (
    <Modal isOpen={isOpen} onClose={onClose} className="max-w-3xl m-4">
      <div className="p-6 space-y-6 bg-white rounded-2xl dark:bg-gray-900">
        <h3 className="text-xl font-semibold text-gray-800 dark:text-white">
          Chỉnh sửa khách hàng
        </h3>
        <form
          className="space-y-6"
          onSubmit={(e) => {
            e.preventDefault();
            handleSubmit();
          }}
        >
          <div className="grid grid-cols-2 gap-4">
            <div>
              <Label>Họ và tên</Label>
              <Input
                type="text"
                value={formData.fullName ?? ""}
                onChange={(e) => handleChange("fullName", e.target.value)}
              />
            </div>
            <div>
              <Label>Vai trò</Label>
              <Input
                type="text"
                value={formData.role ?? ""}
                onChange={(e) => handleChange("role", e.target.value)}
              />
            </div>
            <div className="col-span-2">
              <Label>Vị trí</Label>
              <Input
                type="text"
                value={formData.location ?? ""}
                onChange={(e) => handleChange("location", e.target.value)}
              />
            </div>
          </div>

          <div className="grid grid-cols-2 gap-4">
            <div>
              <Label>First Name</Label>
              <Input
                type="text"
                value={formData.userInfo?.firstName ?? ""}
                onChange={(e) =>
                  handleChange("userInfo.firstName", e.target.value)
                }
              />
            </div>
            <div>
              <Label>Last Name</Label>
              <Input
                type="text"
                value={formData.userInfo?.lastName ?? ""}
                onChange={(e) =>
                  handleChange("userInfo.lastName", e.target.value)
                }
              />
            </div>
            <div>
              <Label>Email</Label>
              <Input
                type="email"
                value={formData.userInfo?.email ?? ""}
                onChange={(e) => handleChange("userInfo.email", e.target.value)}
              />
            </div>
            <div>
              <Label>Phone</Label>
              <Input
                type="text"
                value={formData.userInfo?.phone ?? ""}
                onChange={(e) => handleChange("userInfo.phone", e.target.value)}
              />
            </div>
          </div>

          <div className="grid grid-cols-2 gap-4">
            <div>
              <Label>Quốc gia</Label>
              <Input
                type="text"
                value={formData.address?.country ?? ""}
                onChange={(e) =>
                  handleChange("address.country", e.target.value)
                }
              />
            </div>
            <div>
              <Label>Thành phố/Tỉnh</Label>
              <Input
                type="text"
                value={formData.address?.cityState ?? ""}
                onChange={(e) =>
                  handleChange("address.cityState", e.target.value)
                }
              />
            </div>
            <div>
              <Label>Mã bưu điện</Label>
              <Input
                type="text"
                value={formData.address?.postalCode ?? ""}
                onChange={(e) =>
                  handleChange("address.postalCode", e.target.value)
                }
              />
            </div>
            <div>
              <Label>Mã số thuế</Label>
              <Input
                type="text"
                value={formData.address?.taxId ?? ""}
                onChange={(e) => handleChange("address.taxId", e.target.value)}
              />
            </div>
          </div>

          <div className="flex justify-end gap-3 pt-4">
            <Button size="sm" variant="outline" onClick={onClose}>
              Hủy
            </Button>
            <Button size="sm">Lưu thay đổi</Button>
          </div>
        </form>
      </div>
    </Modal>
  );
}
