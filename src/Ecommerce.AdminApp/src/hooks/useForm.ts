import { useState } from "react";

type UseFormOptions<T> = {
  initialValues: T;
  onSubmit?: (values: T) => void | Promise<void>;
};

export function useForm<T extends Record<string, any>>({
  initialValues,
  onSubmit,
}: UseFormOptions<T>) {
  const [formData, setFormData] = useState<T>(initialValues);

  const handleChange = (path: string, value: any) => {
    setFormData((prev) => {
      const keys = path.split(".");
      const updated = structuredClone(prev) as any;

      let curr = updated;
      while (keys.length > 1) {
        const k = keys.shift()!;
        if (!curr[k]) curr[k] = {};
        curr = curr[k];
      }
      curr[keys[0]] = value;
      return updated;
    });
  };

  const setValues = (values: Partial<T>) => {
    setFormData((prev) => ({ ...prev, ...values }));
  };

  const reset = () => {
    setFormData(initialValues);
  };

  const handleSubmit = async () => {
    if (onSubmit) {
      await onSubmit(formData);
    }
  };

  return {
    formData,
    handleChange,
    setValues,
    reset,
    handleSubmit,
  };
}
