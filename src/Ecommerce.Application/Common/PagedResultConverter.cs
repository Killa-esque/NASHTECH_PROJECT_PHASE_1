using AutoMapper;
using Ecommerce.Shared.Common;
using System.Collections.Generic;

namespace Ecommerce.Application.Common;

public class PagedResultConverter<TSource, TDestination> : ITypeConverter<PagedResult<TSource>, PagedResult<TDestination>>
{
  public PagedResult<TDestination> Convert(PagedResult<TSource> source, PagedResult<TDestination> destination, ResolutionContext context)
  {
    var mappedItems = context.Mapper.Map<IEnumerable<TDestination>>(source.Items);
    return PagedResult<TDestination>.Create(mappedItems, source.TotalCount, source.PageIndex, source.PageSize);
  }
}

