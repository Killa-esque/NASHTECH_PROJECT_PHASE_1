import reactRefresh from "@vitejs/plugin-react-refresh";
import { defineConfig } from "vite";
import svgr from "vite-plugin-svgr";
import tsconfigPaths from "vite-tsconfig-paths";

export default defineConfig({
  plugins: [
    reactRefresh(),
    tsconfigPaths(),
    svgr({
      svgrOptions: {
        icon: true,
        // This will transform your SVG to a React component
        exportType: "named",
        namedExport: "ReactComponent",
      },
    }),
  ],
  define: {
    "process.env": process.env,
  },
  server: {
    port: Number(process.env.VITE_PORT) || 3000,
  },
  preview: {
    port: 8080,
  },
});
