using AuthService;
using Common.Repositories;
using Common.Api;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;
using Serilog.Events;
using System.Text;
using System.Text.Json.Serialization;
using SharpGrip.FluentValidation.AutoValidation.Mvc.Extensions;

namespace Auth.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
                .WriteTo.File("Logs/Information-.txt", LogEventLevel.Information, rollingInterval: RollingInterval.Day)
                .WriteTo.File("Logs/Error-.txt", LogEventLevel.Error, rollingInterval: RollingInterval.Day)
                .CreateLogger();

            try
            {
                var builder = WebApplication.CreateBuilder(args);

                builder.Services.AddControllers().AddJsonOptions(x => x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);

                builder.Services.AddEndpointsApiExplorer();
                builder.Services.AddAuthServices();
                builder.Services.AddTodosDatabase(builder.Configuration);

                builder.Services.AddAuthorization();
                builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                    .AddJwtBearer(options =>
                    {
                        options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
                        {
                            ValidateIssuer = true,
                            ValidateAudience = true,
                            ValidateLifetime = true,
                            ValidateIssuerSigningKey = true,
                            ValidIssuer = builder.Configuration["Jwt:Issuer"],
                            ValidAudience = builder.Configuration["Jwt:Audience"],
                            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!))
                        };
                    });

                builder.Services.AddSwaggerGen(options =>
                {
                    options.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
                    {
                        Name = "Authorization",
                        Type = SecuritySchemeType.ApiKey,
                        Scheme = "Bearer",
                        BearerFormat = "JWT",
                        In = ParameterLocation.Header,
                        Description = "JWT Authorization header using the Bearer scheme. " +
                        "\r\n\r\n Enter 'Bearer' [space] and then your token in the text input below." +
                        "\r\n\r\nExample: \"Bearer 1safsfsdfdfd\"",
                    });

                    options.AddSecurityRequirement(new OpenApiSecurityRequirement
                    {
                        {
                            new OpenApiSecurityScheme
                            {
                                Reference = new OpenApiReference
                                {
                                    Type = ReferenceType.SecurityScheme,
                                    Id = "Bearer"
                                },
                                Scheme = "oauth2",
                                Name = "Bearer",
                                In = ParameterLocation.Header
                            },
                            new List<string>()
                        }
                    });
                });

                builder.Services.AddFluentValidationAutoValidation();

                builder.Host.UseSerilog();
                var app = builder.Build();

                app.UseExceptionsHandler();

                // Configure the HTTP request pipeline.
                if (app.Environment.IsDevelopment())
                {
                    app.UseSwagger();
                    app.UseSwaggerUI();
                }

                app.UseHttpsRedirection();

                app.UseAuthentication();
                app.UseAuthorization();

                app.MapControllers();

                app.Run();
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Run Error");
                throw;
            }
        }
    }
}
