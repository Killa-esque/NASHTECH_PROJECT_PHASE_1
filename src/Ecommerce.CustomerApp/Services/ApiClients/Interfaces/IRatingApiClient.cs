using Ecommerce.Shared.Common;
using Ecommerce.Shared.DTOs;

namespace Ecommerce.CustomerApp.Services.ApiClients.Interfaces;

public interface IRatingApiClient
{
  Task<Guid> CreateRatingAsync(CreateRatingDto request);
  Task<PagedResult<RatingDto>> GetRatingsByProductAsync(Guid productId);
}
