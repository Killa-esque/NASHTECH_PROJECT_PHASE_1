// src/contexts/AuthContext.tsx
import { getUser, getUserManager, isAdmin } from "@/api/auth/authService";
import { User } from "oidc-client-ts";
import React, { createContext, useCallback, useEffect, useState } from "react";

interface AuthContextType {
  user: User | null;
  isAuthenticated: boolean;
  isAdminUser: boolean;
  isLoading: boolean;
  logout: () => Promise<void>;
}

export const AuthContext = createContext<AuthContextType | undefined>(
  undefined
);

export const AuthProvider: React.FC<{ children: React.ReactNode }> = ({
  children,
}) => {
  const [user, setUser] = useState<User | null>(null);
  const [isAdminUser, setIsAdminUser] = useState(false);
  const [isLoading, setIsLoading] = useState(true);

  const loadUser = useCallback(async () => {
    try {
      console.log("[AUTH] Loading user...");
      setIsLoading(true);

      const userData = await getUser();
      setUser(userData);

      if (userData) {
        const adminStatus = await isAdmin(userData);
        setIsAdminUser(adminStatus);
      } else {
        setIsAdminUser(false);
      }
    } catch (error) {
      console.error("[AUTH] Failed to load user:", error);
    } finally {
      setIsLoading(false);
    }
  }, []);

  useEffect(() => {
    loadUser();

    const userManager = getUserManager();

    const onUserLoaded = (loadedUser: User) => {
      console.log("[AUTH] User loaded event");
      setUser(loadedUser);
      isAdmin(loadedUser).then(setIsAdminUser);
    };

    const onUserUnloaded = () => {
      console.log("[AUTH] User unloaded event");
      setUser(null);
      setIsAdminUser(false);
    };

    userManager.events.addUserLoaded(onUserLoaded);
    userManager.events.addUserUnloaded(onUserUnloaded);

    return () => {
      userManager.events.removeUserLoaded(onUserLoaded);
      userManager.events.removeUserUnloaded(onUserUnloaded);
    };
  }, [loadUser]);

  const logout = async () => {
    try {
      console.log("[AUTH] Starting logout...");
      const userManager = getUserManager();
      await userManager.signoutRedirect();
    } catch (error) {
      console.error("[AUTH] Logout failed:", error);
      throw error;
    }
  };

  return (
    <AuthContext.Provider
      value={{
        user,
        isAuthenticated: !!user && !user.expired,
        isAdminUser,
        isLoading,
        logout,
      }}
    >
      {children}
    </AuthContext.Provider>
  );
};
