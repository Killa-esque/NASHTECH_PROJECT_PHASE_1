
using AutoMapper;
using Ecommerce.Domain.Entities;
using Ecommerce.Shared.DTOs;
using Ecommerce.Shared.ViewModels;
using Ecommerce.Shared.Common;
using Ecommerce.Infrastructure.Entities;

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
    // CreateMap<CustomerDto, CustomerViewModel>().ReverseMap();

    // DTO ⇄ Entity
    CreateMap<CategoryDto, Category>().ReverseMap();
    CreateMap<ProductDto, Product>().ReverseMap();
    CreateMap<PaymentTransactionDto, PaymentTransaction>().ReverseMap();
    CreateMap<OrderDto, Order>().ReverseMap();
    CreateMap<OrderItemDto, OrderItem>().ReverseMap();
    // CreateMap<CartItemDto, CartItem>().ReverseMap();
    CreateMap<CartItem, CartItemDto>()
    .ForMember(dest => dest.ProductName, opt => opt.Ignore()) // Set in CartService
    .ForMember(dest => dest.Price, opt => opt.Ignore()) // Set in CartService
    .ForMember(dest => dest.ImageUrl, opt => opt.Ignore()); // Set in CartService
    CreateMap<RatingDto, Rating>().ReverseMap();
    CreateMap<DiscountCodeDto, DiscountCode>().ReverseMap();
    CreateMap<CustomerDto, ApplicationUser>()
             .ForMember(dest => dest.DefaultAddress, opt => opt.MapFrom(src => src.DefaultAddress))
             .ReverseMap();

    // Auto map PagedResult<TSource> → PagedResult<TDestination>
    CreateMap(typeof(PagedResult<>), typeof(PagedResult<>)).ConvertUsing(typeof(PagedResultConverter<,>));
  }
}
