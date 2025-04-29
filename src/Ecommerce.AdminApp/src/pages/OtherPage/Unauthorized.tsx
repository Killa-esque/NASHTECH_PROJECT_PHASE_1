import PageMeta from "@/components/common/PageMeta";
import AuthLayout from "@/layout/AuthPageLayout";
import { Link } from "react-router-dom";

const Unauthorized: React.FC = () => {
  return (
    <>
      <PageMeta
        title="Unauthorized | TailAdmin - Next.js Admin Dashboard Template"
        description="You do not have permission to access this page."
      />
      <AuthLayout>
        <div className="flex flex-col flex-1">
          <div className="flex flex-col justify-center flex-1 w-full max-w-md mx-auto">
            <div>
              <div className="mb-5 sm:mb-8">
                <h1 className="mb-2 font-semibold text-gray-800 text-title-sm dark:text-white/90 sm:text-title-md">
                  Unauthorized Access
                </h1>
                <p className="text-sm text-gray-500 dark:text-gray-400">
                  You do not have permission to access this page. Please{" "}
                  <Link to="/signin" className="text-blue-500 hover:underline">
                    sign in
                  </Link>{" "}
                  with an admin account.
                </p>
              </div>
            </div>
          </div>
        </div>
      </AuthLayout>
    </>
  );
};

export default Unauthorized;
