﻿using Common.Domain;
using Common.Repositories;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace AuthService
{
    public static class AuthServicesDI
    {
        public static IServiceCollection AddAuthServices(this IServiceCollection services)
        {
            services.AddTransient<IAuthService, AuthService>();
            services.AddTransient<IRepository<User>, SqlServerBaseRepository<User>>();
            services.AddTransient<IRepository<UserRole>, SqlServerBaseRepository<UserRole>>();
            services.AddTransient<IRepository<RefreshToken>, SqlServerBaseRepository<RefreshToken>>();
            return services;
        }
    }
}