using AutoMapper;
using DemoWebApplication.Models;
using System.Diagnostics.CodeAnalysis;

namespace DemoWebApplication.Utils
{
    [ExcludeFromCodeCoverage]

    public class MappingProfile : Profile
    {
        public MappingProfile() {

            CreateMap<PersonDto,Person>().ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.Username))
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id));
            CreateMap<Person, PersonDto>().ForMember(dest => dest.Username, opt => opt.MapFrom(src => src.UserName))
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id));

            CreateMap<RegisterUser, Person>().ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.Username));
            CreateMap<FavouriteItemDto, FavouriteItem>().ForMember(dest => dest.ProductId, opt => opt.MapFrom(src => src.ProductId))
                .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.UserId));
        }
    }
}