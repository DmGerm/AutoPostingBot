using AutoMapper;
using AutoPost_Bot.Models;

namespace AutoPost_Bot.MappingProfiles
{
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            CreateMap<UserModel, UserEditModel>()
                .ForMember(dest => dest.Password, opt => opt.MapFrom(src => "*******"));

            CreateMap<UserEditModel, UserModel>()
                .ForMember(dest => dest.PasswordHash, opt => opt.MapFrom<PasswordHashResolver>())
                .ForMember(dest => dest.PasswordSalt, opt => opt.Ignore());
        }
    }
}