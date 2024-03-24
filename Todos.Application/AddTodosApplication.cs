using Common.Application.Abstractions.Persistence;
using Common.Domain;
using Common.Persistence;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using Todos.Application.Mapping;

namespace Todos.Application
{
    public static class AddTodosApplicationExtension
    {
        public static IServiceCollection AddTodosApplication(this IServiceCollection services)
        {
            services.AddAutoMapper(typeof(AutoMapperProfile));
            services.AddSingleton<TodosMemoryCache>();
            services.AddValidatorsFromAssemblies(new[] { Assembly.GetExecutingAssembly() }, includeInternalTypes: true);
            services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));
            services.AddTransient<IContextTransactionCreator, ContextTransactionCreator>();

            services.AddTransient<IRepository<ToDo>, BaseRepository<ToDo>>();
            return services;
        }
    }
}
