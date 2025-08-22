using AutoMapper;
using Domain.Models.Entities;
using LMS.Shared.DTOs.ActivityDtos;
using LMS.Shared.DTOs.AuthDtos;
using LMS.Shared.DTOs.DocumentDtos;
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
            .ForMember(target => target.Type, config => config.MapFrom(src => src.ActivityType))
            .ForMember(target => target.Documents, config => config.MapFrom(src => src.Documents))
            .ReverseMap()
            .ForMember(target => target.ActivityType, config => config.MapFrom(src => src.Type))
            .ForMember(target => target.Documents, config => config.MapFrom(src => src.Documents));
        CreateMap<ActivityEditDto, Activity>()
            .ForMember(target => target.Documents, config => config.AllowNull())
            .ForMember(target => target.StartsAt,
                        opt => opt.MapFrom((src, destination) =>
                                             src.StartsAt == default
                                             ? destination.StartsAt
                                             : src.StartsAt))
            .ReverseMap();
        CreateMap<ActivityCreateDto, Activity>()
            .ForMember(target => target.Documents, config => config.AllowNull())
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
        CreateMap<ModuleCreateDto, Module>()
            .ForMember(target => target.StartsAt,
                        options => options.MapFrom((src, destination) =>
                                             src.StartsAt == default
                                             ? destination.StartsAt
                                             : src.StartsAt))
            .ReverseMap();
        #endregion

        #region Documents
        CreateMap<Document, DocumentDto>()
            .ReverseMap();
        CreateMap<DocumentUpdateDto, Document>()
            .ForMember(target => target.UploadedAt, config => config.Ignore())
            .ForMember(target => target.UploadedByUserId, config => config.Ignore())
            .ReverseMap();
        #endregion
    }
}
