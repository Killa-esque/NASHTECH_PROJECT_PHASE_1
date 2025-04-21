import Label from "@/components/form/Label";
import Input from "@/components/form/input/InputField";
import TextArea from "@/components/form/input/TextArea";
import Button from "@/components/ui/button/Button";
import { Modal } from "@/components/ui/modal";
import { useForm } from "@/hooks/useForm";
import { FullUserProfile } from "@/types/user";

interface UserProfileModalProps {
  isOpen: boolean;
  onClose: () => void;
  initialData: FullUserProfile;
  onSubmit: (data: FullUserProfile) => void;
}

export default function UserProfileModal({
  isOpen,
  onClose,
  initialData,
  onSubmit,
}: UserProfileModalProps) {
  const { formData, handleChange, handleSubmit } = useForm<FullUserProfile>({
    initialValues: initialData,
    onSubmit: (values) => {
      onSubmit(values);
      onClose();
    },
  });

  return (
    <Modal isOpen={isOpen} onClose={onClose} className="max-w-3xl m-4">
      <div className="p-6 space-y-6 bg-white rounded-2xl dark:bg-gray-900">
        <h3 className="text-xl font-semibold text-gray-800 dark:text-white">
          Chỉnh sửa hồ sơ
        </h3>

        <form className="space-y-6" onSubmit={(e) => e.preventDefault()}>
          {/* Section 1: Meta */}
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

          {/* Section 2: Info */}
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
                type="text"
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
            <div className="col-span-2">
              <Label>Bio</Label>
              <TextArea
                value={formData.userInfo?.bio ?? ""}
                onChange={(value) => handleChange("userInfo.bio", value)}
              />
            </div>
          </div>

          {/* Section 3: Address */}
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

          {/* Footer buttons */}
          <div className="flex justify-end gap-3 pt-4">
            <Button size="sm" variant="outline" onClick={onClose}>
              Hủy
            </Button>
            <Button size="sm" onClick={handleSubmit}>
              Lưu thay đổi
            </Button>
          </div>
        </form>
      </div>
    </Modal>
  );
}
