import { AppWrapper } from "@/components/common/PageMeta.tsx";
import { ThemeProvider } from "@/contexts/ThemeContext.tsx";
import "@/index.css";
import { QueryClient, QueryClientProvider } from "@tanstack/react-query";
import { StrictMode } from "react";
import { createRoot } from "react-dom/client";
import { BrowserRouter as Router } from "react-router-dom";
import App from "./App.tsx";

const queryClient = new QueryClient({
  defaultOptions: {
    queries: {
      retry: false, // Disable retries for faster debugging
      staleTime: 0, // Force refetch on mount for testing
    },
  },
});

createRoot(document.getElementById("root")!).render(
  <StrictMode>
    <ThemeProvider>
      <AppWrapper>
        <QueryClientProvider client={queryClient}>
          <Router>
            <App />
          </Router>
        </QueryClientProvider>
      </AppWrapper>
    </ThemeProvider>
  </StrictMode>
);
