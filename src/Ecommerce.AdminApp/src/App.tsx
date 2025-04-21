import { ScrollToTop } from "@/components/common/ScrollToTop";
import useRouteCustom from "@/hooks/useRouteCustom";
import { Suspense } from "react";

function App() {
  return (
    <>
      <ScrollToTop />
      <Suspense fallback={<div>Loading...</div>}>{useRouteCustom()}</Suspense>
    </>
  );
}

export default App;
