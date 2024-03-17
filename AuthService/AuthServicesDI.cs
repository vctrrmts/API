using Common.Domain;
using Common.Repositories;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace AuthService
{
    public static class AuthServicesDI
    {
        public static IServiceCollection AddAuthServices(this IServiceCollection services)
        {
            services.AddTransient<IAuthService, AuthService>();
            services.AddTransient<IRepository<ApplicationUser>, SqlServerBaseRepository<ApplicationUser>>();
            services.AddTransient<IRepository<ApplicationUserRole>, SqlServerBaseRepository<ApplicationUserRole>>();
            services.AddTransient<IRepository<RefreshToken>, SqlServerBaseRepository<RefreshToken>>();
            services.AddValidatorsFromAssemblies(new[] { Assembly.GetExecutingAssembly() }, includeInternalTypes: true);
            return services;
        }
    }
}
