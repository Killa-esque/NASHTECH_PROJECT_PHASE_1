import PageMeta from "@/components/common/PageMeta";
import dayjs from "dayjs";
import { useEffect, useState } from "react";

export default function Home() {
  const [fullName, setFullName] = useState<string>("");

  // Fetch fullName from localStorage
  useEffect(() => {
    const userData = localStorage.getItem(
      "oidc.user:https://localhost:5000:admin_client"
    );
    if (userData) {
      try {
        const parsedData = JSON.parse(userData);
        setFullName(parsedData.profile?.name || "Quản trị viên");
      } catch (error) {
        console.error("Failed to parse user data from localStorage", error);
        setFullName("Quản trị viên");
      }
    } else {
      setFullName("Quản trị viên");
    }
  }, []);

  return (
    <>
      <PageMeta
        title="Tổng Quan Doanh Thu | Admin - Tiệm Bánh Ngọt"
        description="Trang dashboard tổng hợp tình hình kinh doanh cho tiệm bánh ngọt."
      />
      <div className="space-y-6">
        {/* Header */}
        <div className="flex justify-between items-center">
          <h2 className="text-2xl font-bold text-gray-800 dark:text-white">
            Chào {fullName}, hôm nay là {dayjs().format("DD/MM/YYYY")}
          </h2>
        </div>
      </div>
    </>
  );
}
