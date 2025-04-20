import { AppWrapper } from "@/components/common/PageMeta.tsx";
import { ThemeProvider } from "@/contexts/ThemeContext.tsx";
import "@/index.css";
import { StrictMode } from "react";
import { createRoot } from "react-dom/client";
import { BrowserRouter as Router } from "react-router-dom";
import App from "./App.tsx";

createRoot(document.getElementById("root")!).render(
  <StrictMode>
    <ThemeProvider>
      <AppWrapper>
        <Router>
          <App />
        </Router>
      </AppWrapper>
    </ThemeProvider>
  </StrictMode>
);
