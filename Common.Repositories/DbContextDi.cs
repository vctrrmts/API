using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Common.Repositories;

public static class DbContextDi
{
    public static IServiceCollection AddTodosDatabase(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<DbContext, ApplicationDbContext>(
            options =>
            {
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"));
            }
        );
        return services;
    }
}