namespace Ecommerce.Shared.Common;
public class PagedResult<T>
{
  public IEnumerable<T> Items { get; set; }
  public int TotalCount { get; set; }
  public int PageIndex { get; set; }
  public int PageSize { get; set; }

  private PagedResult(IEnumerable<T> items, int count, int pageIndex, int pageSize)
  {
    Items = items;
    TotalCount = count;
    PageIndex = pageIndex;
    PageSize = pageSize;
  }

  public PagedResult()
  {
    Items = Enumerable.Empty<T>(); // Khởi tạo Items để tránh null
  }

  // Factory Method không cần <T>
  public static PagedResult<T> Create(IEnumerable<T> items, int count, int pageIndex, int pageSize)
  {
    return new PagedResult<T>(items, count, pageIndex, pageSize);
  }
}

