// src/hooks/useRouteCustom.tsx
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
const Videos = lazy(() => import("@/pages/UiElements/Videos"));
const Images = lazy(() => import("@/pages/UiElements/Images"));
const Alerts = lazy(() => import("@/pages/UiElements/Alerts"));
const Badges = lazy(() => import("@/pages/UiElements/Badges"));
const Avatars = lazy(() => import("@/pages/UiElements/Avatars"));
const Buttons = lazy(() => import("@/pages/UiElements/Buttons"));
const LineChart = lazy(() => import("@/pages/Charts/LineChart"));
const BarChart = lazy(() => import("@/pages/Charts/BarChart"));
const Calendar = lazy(() => import("@/pages/Calendar"));
const BasicTables = lazy(() => import("@/pages/Tables/BasicTables"));
const FormElements = lazy(() => import("@/pages/Forms/FormElements"));
const Blank = lazy(() => import("@/pages/Blank"));
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
        {
          path: "/calendar",
          element: (
            <ProtectedRoute>
              <Calendar />
            </ProtectedRoute>
          ),
        },
        {
          path: "/blank",
          element: (
            <ProtectedRoute>
              <Blank />
            </ProtectedRoute>
          ),
        },
        {
          path: "/form-elements",
          element: (
            <ProtectedRoute>
              <FormElements />
            </ProtectedRoute>
          ),
        },
        {
          path: "/basic-tables",
          element: (
            <ProtectedRoute>
              <BasicTables />
            </ProtectedRoute>
          ),
        },
        {
          path: "/alerts",
          element: (
            <ProtectedRoute>
              <Alerts />
            </ProtectedRoute>
          ),
        },
        {
          path: "/avatars",
          element: (
            <ProtectedRoute>
              <Avatars />
            </ProtectedRoute>
          ),
        },
        {
          path: "/badge",
          element: (
            <ProtectedRoute>
              <Badges />
            </ProtectedRoute>
          ),
        },
        {
          path: "/buttons",
          element: (
            <ProtectedRoute>
              <Buttons />
            </ProtectedRoute>
          ),
        },
        {
          path: "/images",
          element: (
            <ProtectedRoute>
              <Images />
            </ProtectedRoute>
          ),
        },
        {
          path: "/videos",
          element: (
            <ProtectedRoute>
              <Videos />
            </ProtectedRoute>
          ),
        },
        {
          path: "/line-chart",
          element: (
            <ProtectedRoute>
              <LineChart />
            </ProtectedRoute>
          ),
        },
        {
          path: "/bar-chart",
          element: (
            <ProtectedRoute>
              <BarChart />
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
