using AutoMapper;
using Common.Domain;
using Todos.Application.Command.CreateTodo;
using Todos.Application.Command.UpdateTodo;
using Todos.Application.Dtos;

namespace Todos.Application.Mapping
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<CreateTodoCommand, ToDo>();
            CreateMap<UpdateTodoCommand, ToDo>();
            CreateMap<ToDo, GetTodoDto>();
        }

    }
}
