namespace Ecommerce.API.Mappings;

using AutoMapper;
using Ecommerce.Domain.Entities;
using Ecommerce.Application.DTOs;

// using Ecommerce.Domain.Entities;
// using Ecommerce.Shared.DTOs;

public class MappingProfile : Profile
{
  public MappingProfile()
  {
    // Request → DTO
    // CreateMap<CreateProductRequest, ProductDTO>();

    // DTO ⇄ ViewModel
    // CreateMap<ProductDTO, ProductViewModel>();
    // CreateMap<ProductViewModel, ProductDTO>();

    // DTO ⇄ Entity
    // Product
    // CreateMap<Product, ProductDTO>()
    //     .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category.Name));
    // CreateMap<ProductDTO, Product>();

    // Category
    CreateMap<Category, CategoryDTO>();
    CreateMap<CategoryDTO, Category>();

    // Rating
    CreateMap<Rating, RatingDTO>();
    CreateMap<RatingDTO, Rating>();

    // User
    CreateMap<AspNetUser, UserDTO>();
    CreateMap<UserDTO, AspNetUser>();

    // Role
    CreateMap<AspNetRole, RoleDTO>();
    CreateMap<RoleDTO, AspNetRole>();
  }
}
