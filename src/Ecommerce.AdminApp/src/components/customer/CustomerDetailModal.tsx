import UserAddressCard from "@/components/profile/UserAddressCard";
import UserInfoCard from "@/components/profile/UserInfoCard";
import UserMetaCard from "@/components/profile/UserMetaCard";
import UserOrderListCard from "@/components/profile/UserOrderListCard";
import UserOrderSummaryCard from "@/components/profile/UserOrderSummaryCard";
import Button from "@/components/ui/button/Button";
import { Modal } from "@/components/ui/modal";
import { useModal } from "@/hooks/useModal";
import { ICustomer } from "@/types/customer";
import { useState } from "react";
import CustomerEditModal from "./CustomerEditModal";

type Props = {
  isOpen: boolean;
  onClose: () => void;
  initialData: ICustomer;
};

export default function CustomerDetailModal({
  isOpen,
  onClose,
  initialData,
}: Props) {
  const [customerData, setCustomerData] = useState<ICustomer>(initialData);
  const editModal = useModal();
  if (!customerData) return null;

  return (
    <>
      <Modal isOpen={isOpen} onClose={onClose} className="max-w-4xl w-full p-6">
        <div className="space-y-6">
          <UserMetaCard {...customerData} />
          <UserInfoCard user={customerData.userInfo} />
          <UserAddressCard address={customerData.address} />
          <UserOrderSummaryCard
            totalOrders={customerData.totalOrders ?? 0}
            totalSpent={customerData.totalSpent ?? 0}
          />
          <UserOrderListCard orders={customerData.orders ?? []} />

          <div className="flex justify-end">
            <Button onClick={editModal.openModal}>Chỉnh sửa</Button>
          </div>
        </div>
      </Modal>

      <CustomerEditModal
        isOpen={editModal.isOpen}
        onClose={editModal.closeModal}
        initialData={customerData}
        onSubmit={(updated) => {
          setCustomerData(updated);
          editModal.closeModal();
        }}
      />
    </>
  );
}
