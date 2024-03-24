using Common.Application.Abstractions.Persistence;
using Common.Domain;
using Common.Persistence;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Auth.Application
{
    public static class AddAuthApplicationExtension
    {
        public static IServiceCollection AddAuthApplication(this IServiceCollection services)
        {
            services.AddValidatorsFromAssemblies(new[] { Assembly.GetExecutingAssembly() }, includeInternalTypes: true);
            services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));
            services.AddTransient<IContextTransactionCreator, ContextTransactionCreator>();

            services.AddTransient<IRepository<ApplicationUser>, BaseRepository<ApplicationUser>>();
            services.AddTransient<IRepository<RefreshToken>, BaseRepository<RefreshToken>>();

            return services;
        }
    }
}
