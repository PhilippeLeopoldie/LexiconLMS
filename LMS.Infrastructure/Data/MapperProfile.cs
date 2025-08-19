using AutoMapper;
using Domain.Models.Entities;
using LMS.Shared.DTOs.AuthDtos;
using LMS.Shared.DTOs.ModuleDtos;

namespace LMS.Infrastructure.Data;

public class MapperProfile : Profile
{
    public MapperProfile()
    {
        CreateMap<UserRegistrationDto, ApplicationUser>();
        CreateMap<Module, ModuleDto>().ReverseMap();
        CreateMap<ModuleUpdateDto, Module>()
            .ForMember(target => target.StartsAt,
                        options => options.MapFrom((src, destination) =>
                                             src.StartsAt == default
                                             ? destination.StartsAt
                                             : src.StartsAt))
            .ReverseMap();
        CreateMap<ModuleCreateDto,Module>()
            .ForMember(target => target.StartsAt,
                        options => options.MapFrom((src, destination) =>
                                             src.StartsAt == default
                                             ? destination.StartsAt
                                             : src.StartsAt))
            .ReverseMap();
    }
}
