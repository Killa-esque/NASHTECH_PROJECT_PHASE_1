import { z } from "zod";

// Generic HTTP Response Schema
const HttpResponseSchema = <T extends z.ZodTypeAny>(dataSchema: T) =>
  z.object({
    status: z.boolean(),
    message: z.string().nullable(), // Allow null for message
    data: dataSchema,
  });

// Paginated Response Schema (for APIs returning lists with pagination)
const PaginatedResponseSchema = <T extends z.ZodTypeAny>(itemSchema: T) =>
  z.object({
    items: z.array(itemSchema),
    totalCount: z.number(),
    pageIndex: z.number(),
    pageSize: z.number(),
  });

// Generic Types
export type HttpResponse<T> = {
  status: boolean;
  message: string | null; // Update type to allow null
  data: T;
};

export type PaginatedResponse<T> = {
  items: T[];
  totalCount: number;
  pageIndex: number;
  pageSize: number;
};

// Export schemas for validation
export { HttpResponseSchema, PaginatedResponseSchema };
