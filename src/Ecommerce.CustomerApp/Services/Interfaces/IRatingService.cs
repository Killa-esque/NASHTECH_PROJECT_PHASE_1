using Ecommerce.Shared.Common;
using Ecommerce.Shared.DTOs;
using Ecommerce.Shared.ViewModels;

namespace Ecommerce.CustomerApp.Services.Interfaces;

public interface IRatingService
{
  Task<Guid> CreateRatingAsync(CreateRatingDto request);
  Task<PagedResult<RatingViewModel>> GetRatingsByProductAsync(Guid productId);
}
