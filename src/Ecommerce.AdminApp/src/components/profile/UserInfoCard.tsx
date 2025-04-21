interface UserInfoCardProps {
  user?: {
    firstName?: string;
    lastName?: string;
    email?: string;
    phone?: string;
    bio?: string;
  };
}

export default function UserInfoCard({ user }: UserInfoCardProps) {
  return (
    <div className="p-5 border border-gray-200 rounded-2xl bg-white dark:border-gray-800 dark:bg-gray-900">
      <h4 className="mb-4 text-lg font-semibold text-gray-800 dark:text-white/90">
        Thông tin liên hệ
      </h4>
      <div className="grid grid-cols-1 sm:grid-cols-2 gap-4">
        <Info label="First Name" value={user?.firstName} />
        <Info label="Last Name" value={user?.lastName} />
        <Info label="Email" value={user?.email} />
        <Info label="Phone" value={user?.phone} />
        <Info label="Bio" value={user?.bio} className="sm:col-span-2" />
      </div>
    </div>
  );
}

export function Info({
  label,
  value,
  className = "",
}: {
  label: string;
  value?: string;
  className?: string;
}) {
  return (
    <div className={className}>
      <p className="text-xs text-gray-500 dark:text-gray-400">{label}</p>
      <p className="text-sm font-medium text-gray-800 dark:text-white/90">
        {value || "-"}
      </p>
    </div>
  );
}
