using Common.Domain;
using Common.Repositories;
using Microsoft.Extensions.DependencyInjection;
using Todos.Domain;
using Todos.Service.Mapping;

namespace Todos.Service
{
    public static class ToDoServicesDI
    {
        public static IServiceCollection AddToDoServices(this IServiceCollection services)
        {
            services.AddAutoMapper(typeof(AutoMapperProfile));
            services.AddSingleton<IToDoService, ToDoService>();
            services.AddTransient<IRepository<ToDo>, BaseRepository<ToDo>>();
            services.AddTransient<IRepository<User>, BaseRepository<User>>();

            return services;
        }
    }
}
