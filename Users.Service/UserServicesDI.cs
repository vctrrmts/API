using Microsoft.Extensions.DependencyInjection;
using Common.Domain;
using Common.Repositories;
using Users.Service.Mapping;

namespace Users.Service
{
    public static class UserServicesDI
    {
        public static IServiceCollection AddUserServices(this IServiceCollection services)
        {
            services.AddAutoMapper(typeof(AutoMapperProfile));
            services.AddSingleton<IUserService, UserService>();
            services.AddTransient<IRepository<User>, BaseRepository<User>>();

            return services;
        }
    }
}
