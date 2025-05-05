using Ecommerce.CustomerApp.Services.ApiClients.Interfaces;
using Ecommerce.CustomerApp.Services.Interfaces;
using Ecommerce.Shared.Common;
using Ecommerce.Shared.DTOs;
using Ecommerce.Shared.ViewModels;

namespace Ecommerce.CustomerApp.Services;
public class RatingService : IRatingService
{
  private readonly IRatingApiClient _ratingApiClient;
  private readonly ILogger<RatingService> _logger;

  public RatingService(IRatingApiClient ratingApiClient, ILogger<RatingService> logger)
  {
    _ratingApiClient = ratingApiClient;
    _logger = logger;
  }

  public async Task<Guid> CreateRatingAsync(CreateRatingDto request)
  {
    try
    {
      return await _ratingApiClient.CreateRatingAsync(request);
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, "Lỗi khi gửi đánh giá sản phẩm");
      throw;
    }
  }

  public async Task<PagedResult<RatingViewModel>> GetRatingsByProductAsync(Guid productId)
  {
    var pagedDto = await _ratingApiClient.GetRatingsByProductAsync(productId);
    var items = pagedDto.Items.Select(dto => new RatingViewModel
    {
      Id = dto.Id,
      UserName = dto.UserName ?? "Ẩn danh",
      RatingValue = dto.RatingValue,
      Comment = dto.Comment,
      CreatedDate = dto.CreatedDate
    }).ToList();

    return PagedResult<RatingViewModel>.Create(items, pagedDto.TotalCount, pagedDto.PageIndex, pagedDto.PageSize);
  }
}
