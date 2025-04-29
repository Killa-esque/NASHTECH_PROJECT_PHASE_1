import basicSsl from "@vitejs/plugin-basic-ssl";
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
        exportType: "named",
        namedExport: "ReactComponent",
      },
    }),
    basicSsl(),
  ],
  server: {
    port: Number(process.env.VITE_PORT) || 3000,
  },
  preview: {
    port: 8080,
  },
});
