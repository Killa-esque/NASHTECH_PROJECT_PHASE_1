
using AutoMapper;
using Ecommerce.Domain.Entities;
using Ecommerce.Application.DTOs;
using Ecommerce.Shared.ViewModels;
using Ecommerce.Application.Common;
using Ecommerce.Shared.Common;

public class MappingProfile : Profile
{
  public MappingProfile()
  {
    // DTO ⇄ ViewModel
    CreateMap<CategoryDto, CategoryViewModel>().ReverseMap();
    CreateMap<ProductDto, ProductViewModel>().ReverseMap();
    CreateMap<PaymentTransactionDto, PaymentTransactionViewModel>().ReverseMap();
    CreateMap<OrderDto, OrderViewModel>().ReverseMap();
    CreateMap<OrderItemDto, OrderItemViewModel>().ReverseMap();
    CreateMap<CartItemDto, CartItemViewModel>().ReverseMap();
    CreateMap<RatingDto, RatingViewModel>().ReverseMap();
    CreateMap<DiscountCodeDto, DiscountCodeViewModel>().ReverseMap();

    // DTO ⇄ Entity
    CreateMap<CategoryDto, Category>().ReverseMap();
    CreateMap<ProductDto, Product>().ReverseMap();
    CreateMap<PaymentTransactionDto, PaymentTransaction>().ReverseMap();
    CreateMap<OrderDto, Order>().ReverseMap();
    CreateMap<OrderItemDto, OrderItem>().ReverseMap();
    CreateMap<CartItemDto, CartItem>().ReverseMap();
    CreateMap<RatingDto, Rating>().ReverseMap();
    CreateMap<DiscountCodeDto, DiscountCode>().ReverseMap();

    // Auto map PagedResult<TSource> → PagedResult<TDestination>
    CreateMap(typeof(PagedResult<>), typeof(PagedResult<>)).ConvertUsing(typeof(PagedResultConverter<,>));
  }
}
