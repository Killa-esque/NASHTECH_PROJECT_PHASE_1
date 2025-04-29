import { getUserManager } from "@/api/auth/authService";
import { useEffect } from "react";

const SilentRenew: React.FC = () => {
  useEffect(() => {
    const handleSilentRenew = async () => {
      try {
        const userManager = getUserManager();
        await userManager.signinSilentCallback();
        console.log("[SILENT_RENEW] Silent renew callback successful");
      } catch (error) {
        console.error("[SILENT_RENEW] Silent renew callback failed:", error);
      }
    };

    handleSilentRenew();
  }, []);

  return null;
};

export default SilentRenew;
