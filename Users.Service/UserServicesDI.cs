using Microsoft.Extensions.DependencyInjection;
using Common.Domain;
using Common.Repositories;
using Users.Service.Mapping;
using FluentValidation;
using System.Reflection;

namespace Users.Service
{
    public static class UserServicesDI
    {
        public static IServiceCollection AddUserServices(this IServiceCollection services)
        {
            services.AddAutoMapper(typeof(AutoMapperProfile));
            services.AddTransient<IUserService, UserService>();
            services.AddTransient<IRepository<User>, BaseRepository<User>>();
            services.AddValidatorsFromAssemblies(new[] { Assembly.GetExecutingAssembly() }, includeInternalTypes: true);

            return services;
        }
    }
}
