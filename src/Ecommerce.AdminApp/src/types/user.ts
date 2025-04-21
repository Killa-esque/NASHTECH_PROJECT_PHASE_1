export interface FullUserProfile {
  avatarUrl?: string;
  fullName?: string;
  role?: string;
  location?: string;
  socialLinks?: {
    facebook?: string;
    instagram?: string;
    linkedin?: string;
    x?: string;
  };
  userInfo?: {
    firstName?: string;
    lastName?: string;
    email?: string;
    phone?: string;
    bio?: string;
  };
  address?: {
    country?: string;
    cityState?: string;
    postalCode?: string;
    taxId?: string;
  };
}
