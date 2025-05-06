export {};

declare global {
  interface HttpResponse<T> {
    status: boolean;
    data?: T;
    message?: string;
    error?: string;
  }

  interface PagedResult<T> {
    items: T[];
    totalCount: number;
    pageIndex: number;
    pageSize: number;
  }
}
