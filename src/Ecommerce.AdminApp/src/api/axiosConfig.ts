// src/api/axiosConfig.ts
import { getUserManager } from "@/api/auth/authService";
import axios, {
  AxiosInstance,
  AxiosResponse,
  InternalAxiosRequestConfig,
} from "axios";

// Create Axios instance
const api: AxiosInstance = axios.create({
  baseURL: "https://localhost:5001", // ✅ base URL của Resource Server
  headers: {
    "Content-Type": "application/json",
  },
});

// Request Interceptor: Đính Access Token vào Header
api.interceptors.request.use(
  async (config: InternalAxiosRequestConfig) => {
    try {
      const userManager = getUserManager();
      const user = await userManager.getUser();
      if (user && !user.expired) {
        config.headers.Authorization = `Bearer ${user.access_token}`;
      }
    } catch (error) {
      console.error("[AXIOS] Failed to attach token:", error);
    }
    return config;
  },
  (error) => Promise.reject(error)
);

// Response Interceptor: Handle 401 Unauthorized
api.interceptors.response.use(
  (response: AxiosResponse) => response,
  async (error) => {
    if (error.response?.status === 401) {
      console.warn("[AXIOS] 401 Unauthorized - attempting silent renew...");

      try {
        const userManager = getUserManager();
        await userManager.signinSilent();
        console.log("[AXIOS] Silent renew success. Reloading page...");
        window.location.reload(); // 🔄 reload để load lại token mới
      } catch (renewError) {
        console.error(
          "[AXIOS] Silent renew failed. Redirecting to signin...",
          renewError
        );
        window.location.href = "/signin"; // ❌ nếu silent renew fail thì đưa về signin
      }
    }
    return Promise.reject(error);
  }
);

export default api;
