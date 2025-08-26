using AutoMapper;
using Domain.Models.Entities;
using LMS.Shared.DTOs.ActivityDtos;
using LMS.Shared.DTOs.AuthDtos;
using LMS.Shared.DTOs.DocumentDtos;
using LMS.Shared.DTOs.ModuleDtos;
using LMS.Shared.DTOs.UserDtos;

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
        CreateMap<DocumentManipulationDto, Document>()
            .ForMember(target => target.UploadedAt, config => config.Ignore())
            .ForMember(target => target.UploadedByUserId, config => config.Ignore())
            .ReverseMap();
        #endregion

        #region Users
        CreateMap<ApplicationUser, UserBasicDto>()
            .ForMember(d => d.Id, opt => opt.MapFrom(s => s.Id))
            .ForMember(d => d.UserName, opt => opt.MapFrom(s => s.UserName))
            .ForMember(d => d.Email, opt => opt.MapFrom(s => s.Email));

        CreateMap<UserBasicDto, ApplicationUser>()
            .ForMember(d => d.Id, opt => opt.MapFrom(s => s.Id))
            .ForMember(d => d.UserName, opt => opt.MapFrom(s => s.UserName))
            .ForMember(d => d.Email, opt => opt.MapFrom(s => s.Email));
        #endregion
    }
}
