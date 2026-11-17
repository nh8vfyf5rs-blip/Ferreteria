using AutoMapper;
using Ferreteria.DTOS;
using Ferreteria.Models.Segurudad;

namespace Ferreteria.Mapping
{
    public class UserProfile : Profile
    {
  public UserProfile()
        {
            CreateMap<Usuario, LoginResponseDto>()
                       .ForMember(d => d.AccessToken, opt => opt.Ignore())
                       .ForMember(d => d.RefreshToken, opt => opt.Ignore())
                       .ForMember(d => d.ExpiresAt, opt => opt.Ignore());

            CreateMap<RegisterRequestDto, Usuario>()
                        .ForMember(d => d.Password, opt => opt.Ignore())
                        .ForMember(d => d.EmailConfirmed, opt => opt.MapFrom(src => false))
                        .ForMember(d => d.EmailConfirmToken, opt => opt.Ignore())
                        .ForMember(d => d.RefreshToken, opt => opt.Ignore())
                        .ForMember(d => d.RefreshToken, opt => opt.Ignore())
                        .ForMember(d => d.RefreshTokenExpiryTime, opt => opt.Ignore())
                        .ForMember(d => d.PasswordResedToken, opt => opt.Ignore())
                        .ForMember(d => d.Rol, opt => opt.Ignore());

        }

}

}
