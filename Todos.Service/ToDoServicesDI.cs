using Common.Domain;
using Common.Repositories;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using Todos.Domain;
using Todos.Service.Mapping;

namespace Todos.Service
{
    public static class ToDoServicesDI
    {
        public static IServiceCollection AddToDoServices(this IServiceCollection services)
        {
            services.AddAutoMapper(typeof(AutoMapperProfile));
            services.AddTransient<IToDoService, ToDoService>();
            services.AddTransient<IRepository<ToDo>, BaseRepository<ToDo>>();
            services.AddTransient<IRepository<User>, BaseRepository<User>>();
            services.AddValidatorsFromAssemblies(new[] { Assembly.GetExecutingAssembly() }, includeInternalTypes: true);

            return services;
        }
    }
}
