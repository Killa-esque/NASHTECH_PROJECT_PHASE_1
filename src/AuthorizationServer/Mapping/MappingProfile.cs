using AutoMapper;
using Ecommerce.Shared.DTOs;
using Ecommerce.Infrastructure.Entities;

namespace AuthorizationServer.Mapping;

public class MappingProfile : Profile
{
  public MappingProfile()
  {
    CreateMap<ApplicationUser, CustomerDto>()
        .ForMember(dest => dest.DateOfBirth, opt => opt.MapFrom(src => src.DateOfBirth.HasValue ? src.DateOfBirth.Value.ToString("dd-MM-yyyy") : null));
    CreateMap<UpdateCustomerDto, ApplicationUser>()
        .ForMember(dest => dest.Id, opt => opt.Ignore())
        .ForMember(dest => dest.UserName, opt => opt.Ignore())
        .ForMember(dest => dest.Email, opt => opt.Ignore())
        .ForMember(dest => dest.NormalizedUserName, opt => opt.Ignore())
        .ForMember(dest => dest.NormalizedEmail, opt => opt.Ignore())
        .ForMember(dest => dest.PasswordHash, opt => opt.Ignore())
        .ForMember(dest => dest.SecurityStamp, opt => opt.Ignore())
        .ForMember(dest => dest.ConcurrencyStamp, opt => opt.Ignore())
        .ForMember(dest => dest.DateOfBirth, opt => opt.Ignore()); // Ignore vì đã xử lý thủ công
  }
}
