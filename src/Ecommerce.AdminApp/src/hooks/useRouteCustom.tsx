import { ProtectedRoute } from "@/components/auth/ProtectedRoute";
import AppLayout from "@/layout/AppLayout";
import { lazy } from "react";
import { useRoutes } from "react-router-dom";

const Callback = lazy(() => import("@/pages/AuthPage/Callback"));
const SignIn = lazy(() => import("@/pages/AuthPage/SignIn"));
const SilentRenew = lazy(() => import("@/pages/AuthPage/SilentRenew"));
const Unauthorized = lazy(() => import("@/pages/OtherPage/Unauthorized"));
const NotFound = lazy(() => import("@/pages/OtherPage/NotFound"));
const UserProfiles = lazy(() => import("@/pages/UserProfiles"));
const Category = lazy(() => import("@/pages/CategoryPage"));
const Customer = lazy(() => import("@/pages/CustomerPage"));
const Product = lazy(() => import("@/pages/ProductPage"));
const OrderList = lazy(() => import("@/pages/OrderPage"));
const OrderDetail = lazy(() => import("@/pages/OrderPage/OrderDetail"));
const Home = lazy(() => import("@/pages/Dashboard/Home"));

function useRouteCustom() {
  return useRoutes([
    {
      element: <AppLayout />,
      children: [
        {
          path: "/",
          element: (
            <ProtectedRoute>
              <Home />
            </ProtectedRoute>
          ),
        },
        {
          path: "/profile",
          element: (
            <ProtectedRoute>
              <UserProfiles />
            </ProtectedRoute>
          ),
        },
        {
          path: "/categories",
          element: (
            <ProtectedRoute>
              <Category />
            </ProtectedRoute>
          ),
        },
        {
          path: "/products",
          element: (
            <ProtectedRoute>
              <Product />
            </ProtectedRoute>
          ),
        },
        {
          path: "/customers",
          element: (
            <ProtectedRoute>
              <Customer />
            </ProtectedRoute>
          ),
        },
        {
          path: "/admin/orders",
          element: (
            <ProtectedRoute>
              <OrderList />
            </ProtectedRoute>
          ),
        },
        {
          path: "/admin/orders/:orderId",
          element: (
            <ProtectedRoute>
              <OrderDetail />
            </ProtectedRoute>
          ),
        },
      ],
    },
    { path: "/signin", element: <SignIn /> },
    { path: "/silent-renew", element: <SilentRenew /> },
    { path: "/oauth/callback", element: <Callback /> },
    { path: "/unauthorized", element: <Unauthorized /> },
    { path: "*", element: <NotFound /> },
  ]);
}

export default useRouteCustom;
