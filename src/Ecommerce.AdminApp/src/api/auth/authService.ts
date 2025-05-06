// src/api/auth/authService.ts
import {
  User,
  UserManager,
  UserManagerSettings,
  WebStorageStateStore,
} from "oidc-client-ts";

let userManager: UserManager;

const config: UserManagerSettings = {
  authority: import.meta.env.VITE_AUTH_AUTHORITY,
  client_id: import.meta.env.VITE_AUTH_CLIENT_ID,
  // client_secret: import.meta.env.VITE_AUTH_CLIENT_SECRET, // không cần thiết nếu PKCE, nhưng bạn giữ cũng OK
  redirect_uri: import.meta.env.VITE_AUTH_REDIRECT_URI,
  post_logout_redirect_uri: import.meta.env.VITE_AUTH_POST_LOGOUT_REDIRECT_URI,
  response_type: "code", // Authorization Code Flow
  scope: import.meta.env.VITE_AUTH_SCOPE,
  automaticSilentRenew: true,
  silent_redirect_uri: import.meta.env.VITE_AUTH_SILENT_REDIRECT_URI,
  accessTokenExpiringNotificationTimeInSeconds: 60,
  userStore: new WebStorageStateStore({ store: window.localStorage }),
};

export const getUserManager = (): UserManager => {
  if (!userManager) {
    userManager = new UserManager(config);

    // Listen sự kiện tiện lợi
    userManager.events.addUserLoaded((user) => {
      console.log("[AUTH] User loaded:", user);
    });

    userManager.events.addUserUnloaded(() => {
      console.log("[AUTH] User unloaded");
    });

    userManager.events.addAccessTokenExpiring(() => {
      console.log("[AUTH] Access token expiring...");
    });

    userManager.events.addAccessTokenExpired(() => {
      console.log("[AUTH] Access token expired");
    });

    userManager.events.addSilentRenewError((error) => {
      console.error("[AUTH] Silent renew error:", error);
    });
  }
  return userManager;
};

// Login: Redirect qua Authorization Server
export const login = async (): Promise<void> => {
  const manager = getUserManager();
  console.log("[AUTH] Starting signinRedirect...");
  await manager.signinRedirect();
};

// Callback: Đổi Authorization Code lấy token
export const signinCallback = async (): Promise<User> => {
  const manager = getUserManager();
  console.log("[AUTH] Handling signinCallback...");
  const user = await manager.signinRedirectCallback();
  console.log("[AUTH] Signin callback completed. User:", user);
  return user;
};

// Logout: Redirect logout
export const logout = async (): Promise<void> => {
  const manager = getUserManager();
  console.log("[AUTH] Starting signoutRedirect...");
  await manager.signoutRedirect();
};

// Get current user
export const getUser = async (): Promise<User | null> => {
  const manager = getUserManager();
  try {
    const user = await manager.getUser();
    if (!user || user.expired) {
      console.log("[AUTH] No user or token expired");
      return null;
    }
    return user;
  } catch (error) {
    console.error("[AUTH] Failed to get user:", error);
    return null;
  }
};

export const isAdmin = async (user: User | null): Promise<boolean> => {
  if (!user || !user.profile) {
    return false;
  }

  const role = (user.profile as any).role || (user.profile as any).roles;

  if (!role) {
    console.log("[AUTH] No role found in user profile");
    return false;
  }

  // Nếu role là array hoặc string
  const roles = Array.isArray(role) ? role : [role];

  const isAdminUser = roles.includes("admin");

  console.log("[AUTH] isAdminUser:", isAdminUser);
  return isAdminUser;
};

// Manual Refresh Token (optional gọi tay)
export const refreshTokenManually = async (): Promise<User> => {
  const manager = getUserManager();
  console.log("[AUTH] Manually refreshing token...");
  const user = await manager.signinSilent();
  if (!user) {
    throw new Error("[AUTH] Failed to refresh token: No user returned");
  }
  console.log("[AUTH] Token refreshed successfully:", user);
  return user;
};
