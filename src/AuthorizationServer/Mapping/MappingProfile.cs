using AutoMapper;
using Ecommerce.Infrastructure.Entities;
using Ecommerce.Shared.DTOs;

namespace AuthorizationServer.Mapping;

public class MappingProfile : Profile
{
  public MappingProfile()
  {
    CreateMap<ApplicationUser, CustomerDto>()
        .ForMember(dest => dest.DateOfBirth, opt => opt.MapFrom(src =>
            src.DateOfBirth.HasValue ? src.DateOfBirth.Value : (DateTime?)null));

    CreateMap<CreateCustomerDto, ApplicationUser>()
        .ForMember(dest => dest.Id, opt => opt.Ignore())
        .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.Email))
        .ForMember(dest => dest.NormalizedUserName, opt => opt.Ignore())
        .ForMember(dest => dest.NormalizedEmail, opt => opt.Ignore())
        .ForMember(dest => dest.PasswordHash, opt => opt.Ignore())
        .ForMember(dest => dest.SecurityStamp, opt => opt.Ignore())
        .ForMember(dest => dest.ConcurrencyStamp, opt => opt.Ignore())
        .ForMember(dest => dest.DateOfBirth, opt => opt.Ignore()); // Xử lý trong service

    CreateMap<UpdateCustomerDto, ApplicationUser>()
        .ForMember(dest => dest.Id, opt => opt.Ignore())
        .ForMember(dest => dest.UserName, opt => opt.Ignore())
        .ForMember(dest => dest.Email, opt => opt.Ignore())
        .ForMember(dest => dest.NormalizedUserName, opt => opt.Ignore())
        .ForMember(dest => dest.NormalizedEmail, opt => opt.Ignore())
        .ForMember(dest => dest.PasswordHash, opt => opt.Ignore())
        .ForMember(dest => dest.SecurityStamp, opt => opt.Ignore())
        .ForMember(dest => dest.ConcurrencyStamp, opt => opt.Ignore())
        .ForMember(dest => dest.DateOfBirth, opt => opt.Ignore()); // Xử lý trong service
  }
}
