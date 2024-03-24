using AutoMapper;
using Common.Domain;
using Users.Application.Command.CreateUser;
using Users.Application.Command.UpdateUser;
using Users.Application.Dtos;

namespace Users.Application.Mapping
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<CreateUserCommand, ApplicationUser>();
            CreateMap<UpdateUserCommand, ApplicationUser>();
            CreateMap<ApplicationUser, GetUserDto>();
        }

    }
}
