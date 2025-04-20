import AppLayout from "@/layout/AppLayout";
import { lazy } from "react";
import { useRoutes } from "react-router-dom";

const SignIn = lazy(() => import("@/pages/AuthPages/SignIn"));
const NotFound = lazy(() => import("@/pages/OtherPage/NotFound"));
const UserProfiles = lazy(() => import("@/pages/UserProfiles"));
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
        { path: "/", element: <Home /> },
        { path: "/profile", element: <UserProfiles /> },
        { path: "/calendar", element: <Calendar /> },
        { path: "/blank", element: <Blank /> },
        { path: "/form-elements", element: <FormElements /> },
        { path: "/basic-tables", element: <BasicTables /> },
        { path: "/alerts", element: <Alerts /> },
        { path: "/avatars", element: <Avatars /> },
        { path: "/badge", element: <Badges /> },
        { path: "/buttons", element: <Buttons /> },
        { path: "/images", element: <Images /> },
        { path: "/videos", element: <Videos /> },
        { path: "/line-chart", element: <LineChart /> },
        { path: "/bar-chart", element: <BarChart /> },
      ],
    },
    { path: "/signin", element: <SignIn /> },
    { path: "*", element: <NotFound /> },
  ]);
}

export default useRouteCustom;
