using AutoMapper;
using AutoPost_Bot.Mappers;
using AutoPost_Bot.Models;

namespace AutoPost_Bot.MappingProfiles
{
    public class PostProfile : Profile
    {
        public PostProfile()
        {
            CreateMap<PostModel, PostEntity>();
        }
    }
}
