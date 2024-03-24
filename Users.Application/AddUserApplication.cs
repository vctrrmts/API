using Common.Application.Abstractions.Persistence;
using Common.Domain;
using Common.Persistence;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using Users.Application.Mapping;

namespace Users.Application
{
    public static class AddUserApplicationExtension
    {
        public static IServiceCollection AddUserApplication(this IServiceCollection services)
        {
            services.AddAutoMapper(typeof(AutoMapperProfile));
            services.AddSingleton<UsersMemoryCache>();
            services.AddValidatorsFromAssemblies(new[] { Assembly.GetExecutingAssembly() }, includeInternalTypes: true);
            services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));
            services.AddTransient<IContextTransactionCreator, ContextTransactionCreator>();

            services.AddTransient<IRepository<ApplicationUser>, BaseRepository<ApplicationUser>>();
            services.AddTransient<IRepository<ApplicationUserRole>, BaseRepository<ApplicationUserRole>>();
            services.AddTransient<IRepository<ApplicationUserApplicationRole>, BaseRepository<ApplicationUserApplicationRole>>();

            return services;
        }
    }
}
