using AutoMapper;
using Domain.Models.Entities;
using LMS.Shared.DTOs.ActivityDtos;
using LMS.Shared.DTOs.AuthDtos;
using LMS.Shared.DTOs.ModuleDtos;

namespace LMS.Infrastructure.Data;

public class MapperProfile : Profile
{
    public MapperProfile()
    {
        CreateMap<UserRegistrationDto, ApplicationUser>();

        #region Activities
        CreateMap<ActivityTypeDto, ActivityType>()
            .ReverseMap();
        CreateMap<ActivityDto, Activity>()
            .ReverseMap();
        CreateMap<ActivityEditDto, Activity>()
            //.ForMember(target => target.Documents, config => config.AllowNull())
            .ForMember(target => target.StartsAt,
                        opt => opt.MapFrom((src, destination) =>
                                             src.StartsAt == default
                                             ? destination.StartsAt
                                             : src.StartsAt))
            .ReverseMap();
        CreateMap<ActivityCreateDto, Activity>()
            //.ForMember(target => target.Documents, config => config.AllowNull())
            .ForMember(target => target.StartsAt,
                        opt => opt.MapFrom((src, destination) =>
                                             src.StartsAt == default
                                             ? destination.StartsAt
                                             : src.StartsAt))
            .ReverseMap();
        #endregion
        #region Modules
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
        #endregion
    }
}
