// src/components/common/ProtectedRoute.tsx
import { useAuth } from "@/hooks/useAuth";
import { Navigate } from "react-router-dom";

interface ProtectedRouteProps {
  children: JSX.Element;
}

export const ProtectedRoute: React.FC<ProtectedRouteProps> = ({ children }) => {
  const { isAuthenticated, isAdminUser, isLoading } = useAuth();

  if (isLoading) {
    return (
      <div className="flex items-center justify-center min-h-screen">
        {/* bạn có thể import Spinner riêng hoặc dùng div text cũng được */}
        <div className="text-xl">Loading...</div>
      </div>
    );
  }

  if (!isAuthenticated) {
    console.warn(
      "[PROTECTED_ROUTE] User not authenticated. Redirecting to /signin"
    );
    return <Navigate to="/signin" replace />;
  }

  if (!isAdminUser) {
    console.warn(
      "[PROTECTED_ROUTE] User not admin. Redirecting to /unauthorized"
    );
    return <Navigate to="/unauthorized" replace />;
  }

  return children;
};
