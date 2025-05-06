export const API_URLS = {
  // Category Endpoints
  CATEGORY: {
    BASE: "/api/admin/categories",
    BY_ID: (id: string) => `/api/admin/categories/${id}`,
  },

  // Order Endpoints
  ORDER: {
    BASE: "/api/admin/orders",
    BY_ID: (orderId: string) => `/api/admin/orders/${orderId}`,
    UPDATE_STATUS: (orderId: string) => `/api/admin/orders/${orderId}/status`,
  },

  // Product Endpoints
  PRODUCT: {
    BASE: "/api/admin/products",
    BY_ID: (id: string) => `/api/admin/products/${id}`,
    SET_FEATURED: (id: string) => `/api/admin/products/${id}/set-featured`,
  },

  //Customer Endpoints
  CUSTOMER: {
    BASE: "/api/users",
    BY_ID: (id: string) => `/api/users/${id}`,
  },
};
