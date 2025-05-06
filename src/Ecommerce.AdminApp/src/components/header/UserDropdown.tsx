// src/components/user/UserDropdown.tsx
import { useEffect, useState } from "react";

export default function UserDropdown() {
  const [isOpen, setIsOpen] = useState(false);
  const [userName, setUserName] = useState("Musharof Chowdhury");
  const [userEmail, setUserEmail] = useState("randomuser@pimjo.com");
  const [avatarUrl, setAvatarUrl] = useState("/images/user/owner.jpg");

  useEffect(() => {
    const userData = localStorage.getItem(
      "oidc.user:https://localhost:5000:admin_client"
    );
    if (userData) {
      try {
        const parsedData = JSON.parse(userData);
        setUserName(parsedData.profile?.name || "Musharof Chowdhury");
        setUserEmail(parsedData.profile?.email || "randomuser@pimjo.com");
        setAvatarUrl(parsedData.profile?.avatarUrl || "/images/user/owner.jpg");
      } catch (error) {
        console.error("Failed to parse user data from localStorage", error);
      }
    }
  }, []);

  function toggleDropdown() {
    setIsOpen(!isOpen);
  }

  function closeDropdown() {
    setIsOpen(false);
  }

  const handleLogout = async () => {
    try {
      // Add your logout logic here
      console.log("Logged out");
      closeDropdown();
    } catch (error) {
      console.error("Logout failed", error);
    }
  };

  return (
    <div className="relative">
      <button
        onClick={toggleDropdown}
        className="flex items-center text-gray-700 dropdown-toggle dark:text-gray-400"
      >
        <span className="mr-3 overflow-hidden rounded-full h-11 w-11">
          <img src={avatarUrl} alt={userName} />
        </span>
        <span className="block mr-1 font-medium text-theme-sm">{userName}</span>
        <svg
          className={`stroke-gray-500 dark:stroke-gray-400 transition-transform duration-200 ${
            isOpen ? "rotate-180" : ""
          }`}
          width="18"
          height="20"
          viewBox="0 0 18 20"
          fill="none"
          xmlns="http://www.w3.org/2000/svg"
        >
          <path
            d="M4.3125 8.65625L9 13.3437L13.6875 8.65625"
            stroke="currentColor"
            strokeWidth="1.5"
            strokeLinecap="round"
            strokeLinejoin="round"
          />
        </svg>
      </button>

      {isOpen && (
        <div className="absolute right-0 mt-2 w-48 bg-white rounded-md shadow-lg">
          <div className="px-4 py-2 text-sm text-gray-700">
            <p>{userName}</p>
            <p>{userEmail}</p>
          </div>
          <div className="border-t border-gray-200">
            <button
              onClick={handleLogout}
              className="block w-full px-4 py-2 text-left text-sm text-gray-700 hover:bg-gray-100"
            >
              Logout
            </button>
          </div>
        </div>
      )}
    </div>
  );
}
