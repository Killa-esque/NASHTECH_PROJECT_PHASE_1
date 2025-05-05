using Ecommerce.Shared.DTOs;
using Ecommerce.Shared.Common;

namespace Ecommerce.Application.Interfaces.Services;

public interface IRatingService
{
  Task<Result<Guid>> CreateRatingAsync(string userId, CreateRatingDto request);
  Task<Result<PagedResult<RatingDto>>> GetRatingsByProductAsync(Guid productId, int pageIndex = 1, int pageSize = 10);

}
