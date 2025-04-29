import { signinCallback } from "@/api/auth/authService";
import PageMeta from "@/components/common/PageMeta";
import AuthLayout from "@/layout/AuthPageLayout";
import { useEffect } from "react";
import { useNavigate } from "react-router-dom";

const Callback: React.FC = () => {
  const navigate = useNavigate();

  useEffect(() => {
    const handleCallback = async () => {
      try {
        console.log("[CALLBACK] Starting callback...");
        await signinCallback();
        console.log("[CALLBACK] Callback successful, navigating to dashboard");
        navigate("/");
      } catch (error) {
        console.error("[CALLBACK] Callback failed:", error);
        navigate("/signin");
      }
    };

    handleCallback();
  }, [navigate]);
  return (
    <>
      <PageMeta
        title="React.js SignIn Dashboard | TailAdmin - Next.js Admin Dashboard Template"
        description="This is React.js SignIn Tables Dashboard page for TailAdmin - React.js Tailwind CSS Admin Dashboard Template"
      />
      <AuthLayout>
        <div className="flex flex-col flex-1">
          <div className="flex flex-col justify-center flex-1 w-full max-w-md mx-auto">
            <div>
              <div className="mb-5 sm:mb-8">
                <h1 className="mb-2 font-semibold text-gray-800 text-title-sm dark:text-white/90 sm:text-title-md">
                  Processing login...
                </h1>
              </div>
            </div>
          </div>
        </div>
      </AuthLayout>
    </>
  );
};

export default Callback;
