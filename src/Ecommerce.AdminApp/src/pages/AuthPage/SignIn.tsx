// src/pages/SignIn.tsx
import { login } from "@/api/auth/authService";
import PageMeta from "@/components/common/PageMeta";
import Button from "@/components/ui/button/Button";
import { useAuth } from "@/hooks/useAuth";
import AuthLayout from "@/layout/AuthPageLayout";
import { useEffect } from "react";
import { useNavigate } from "react-router-dom";

export default function SignIn() {
  const { isAuthenticated, isAdminUser, isLoading } = useAuth();
  const navigate = useNavigate();

  useEffect(() => {
    if (isLoading) return;
    if (isAuthenticated && isAdminUser) {
      console.log(
        "[SIGNIN] User is authenticated and admin, navigating to dashboard"
      );
      navigate("/");
    } else if (isAuthenticated && !isAdminUser) {
      console.log(
        "[SIGNIN] User is authenticated but not admin, navigating to unauthorized"
      );
      navigate("/unauthorized");
    }
  }, [isAuthenticated, isAdminUser, isLoading, navigate]);

  const handleLogin = async () => {
    try {
      await login();
    } catch (error) {
      console.error("[SIGNIN] Login failed:", error);
    }
  };

  if (isLoading) {
    return <div>Loading...</div>;
  }

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
                  Sign In
                </h1>
                <p className="text-sm text-gray-500 dark:text-gray-400">
                  Click to sign in!
                </p>
              </div>
              <Button className="w-full" size="sm" onClick={handleLogin}>
                Sign in
              </Button>
            </div>
          </div>
        </div>
      </AuthLayout>
    </>
  );
}
