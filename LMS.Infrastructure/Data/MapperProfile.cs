using AutoMapper;
using Domain.Models.Entities;
using LMS.Shared.DTOs.AuthDtos;

namespace LMS.Infrastructure.Data;

public class MapperProfile : Profile
{
    public MapperProfile()
    {
        CreateMap<UserRegistrationDto, ApplicationUser>();
    }
}
