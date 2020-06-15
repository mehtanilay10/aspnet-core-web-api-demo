using System;
using AutoMapper;
using DemoApp.EntityFramework.IdentityModels;
using DemoApp.Models.ApiModels.Auth;
using DemoApp.Models.Enums;

namespace DemoApp.AutoMapper
{
    public class AuthProfile : Profile
    {
        public AuthProfile()
        {
            CreateMap<SecurityQuestionDto, UpdateSecurityQuestionDto>();

            CreateMap<AppUser, UserDetailsDto>()
                .ForMember(dest => dest.UserId, options => options.MapFrom(src => src.Id))
                .ForMember(dest => dest.Culture, options => options.MapFrom(src => (SupportedCulture)src.Culture));

            CreateMap<RegisterUserDto, AppUser>()
                .BeforeMap((src, dest) =>
                {
                    dest.IsActive = true;
                    dest.JoinedOn = DateTime.Now;
                    dest.UserName = src.Email;
                    dest.Culture = (int)SupportedCulture.en;
                });
        }
    }
}
