using AutoMapper;
using Ecommerce.Application.DTOs;
using Ecommerce.Application.Interfaces.Repositories;
using Ecommerce.Application.Interfaces.Services;
using Ecommerce.Domain.Entities;
using Ecommerce.Shared.Common;

namespace Ecommerce.Application.Services;

public class RatingService : IRatingService
{
  private readonly IMapper _mapper;
  private readonly IProductRepository _productRepository;
  private readonly IRatingRepository _ratingRepository;

  public RatingService(IMapper mapper, IProductRepository productRepository, IRatingRepository ratingRepository)
  {
    _mapper = mapper;
    _productRepository = productRepository;
    _ratingRepository = ratingRepository;
  }

  public async Task<Result<Guid>> CreateRatingAsync(string userId, CreateRatingDto request)
  {
    var product = await _productRepository.GetByIdAsync(request.ProductId);
    if (product == null) return Result.Failure<Guid>("Product not found.");

    var rating = new Rating
    {
      ProductId = request.ProductId,
      UserId = userId,
      RatingValue = request.RatingValue,
      Comment = request.Comment
    };

    await _ratingRepository.AddAsync(rating);
    await _ratingRepository.SaveChangesAsync();

    return Result.Success(rating.Id, "Rating created successfully.");
  }

  public async Task<Result<PagedResult<RatingDto>>> GetRatingsByProductAsync(Guid productId, int pageIndex = 1, int pageSize = 10)
  {
    var ratings = await _ratingRepository.GetRatingsByProductIdAsync(productId, pageIndex, pageSize);
    if (ratings == null)
      return Result.Failure<PagedResult<RatingDto>>("Ratings not found.");

    var ratingsDtos = _mapper.Map<IEnumerable<RatingDto>>(ratings);
    var totalCount = ratingsDtos.Count();
    var pagedResult = PagedResult<RatingDto>.Create(ratingsDtos, totalCount, pageIndex, pageSize);

    return Result.Success(pagedResult, "Ratings retrieved successfully.");
  }

}
