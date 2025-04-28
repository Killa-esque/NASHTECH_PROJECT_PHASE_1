import axios, {
  AxiosInstance,
  AxiosResponse,
  InternalAxiosRequestConfig,
} from "axios";

// Axios instance with base configuration
const api: AxiosInstance = axios.create({
  baseURL: "https://localhost:5001/",
  headers: {
    "Content-Type": "application/json",
  },
});

// Interceptor for requests
api.interceptors.request.use(
  (config: InternalAxiosRequestConfig) => {
    const token = localStorage.getItem("token");
    if (token) {
      config.headers.Authorization = `Bearer ${token}`;
    }
    return config;
  },
  (error) => Promise.reject(error)
);

// Interceptor for responses
api.interceptors.response.use(
  (response: AxiosResponse) => response,
  (error) => {
    if (error.response?.status === 401) {
      console.error("Unauthorized access - redirecting to login");
    }
    return Promise.reject(error);
  }
);

export default api;
