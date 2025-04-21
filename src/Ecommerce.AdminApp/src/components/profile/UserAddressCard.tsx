import { Info } from "./UserInfoCard";

interface UserAddressCardProps {
  address?: {
    country?: string;
    cityState?: string;
    postalCode?: string;
    taxId?: string;
  };
}

export default function UserAddressCard({ address }: UserAddressCardProps) {
  return (
    <div className="p-5 border border-gray-200 rounded-2xl bg-white dark:border-gray-800 dark:bg-gray-900">
      <h4 className="mb-4 text-lg font-semibold text-gray-800 dark:text-white/90">
        Địa chỉ
      </h4>
      <div className="grid grid-cols-1 sm:grid-cols-2 gap-4">
        <Info label="Quốc gia" value={address?.country} />
        <Info label="Thành phố/Tỉnh" value={address?.cityState} />
        <Info label="Mã bưu điện" value={address?.postalCode} />
        <Info label="Mã số thuế" value={address?.taxId} />
      </div>
    </div>
  );
}
