using Common.Application.Behaviors;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace Common.Application
{
    public static class AddCommonApplicationExtension
    {
        public static IServiceCollection AddCommonApplication(this IServiceCollection services)
        {
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ContextTransactionBehavior<,>));

            return services;
        }
    }
}
