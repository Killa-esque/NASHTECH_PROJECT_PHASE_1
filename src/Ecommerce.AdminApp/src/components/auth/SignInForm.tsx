import Button from "@/components/ui/button/Button";

export default function SignInForm() {
  return (
    <div className="flex flex-col flex-1">
      <div className="flex flex-col justify-center flex-1 w-full max-w-md mx-auto">
        <div>
          <div className="mb-5 sm:mb-8">
            <h1 className="mb-2 font-semibold text-gray-800 text-title-sm dark:text-white/90 sm:text-title-md">
              Sign In
            </h1>
            <p className="text-sm text-gray-500 dark:text-gray-400">
              Click to sign in!
            </p>
          </div>
          <Button className="w-full" size="sm">
            Sign in
          </Button>
        </div>
      </div>
    </div>
  );
}
