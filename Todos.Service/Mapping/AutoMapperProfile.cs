using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common.Domain;
using Todos.Service.Dto;

namespace Todos.Service.Mapping
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile() 
        {
            CreateMap<CreateToDoDto, ToDo>();
            CreateMap<UpdateToDoDto, ToDo>();
            CreateMap<ToDo, GetToDoDto>();
        }

    }
}
