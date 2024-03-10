using Serilog;
using Serilog.Events;
using SharpGrip.FluentValidation.AutoValidation.Mvc.Extensions;
using Todos.Service;
using Common.Api;
using Common.Repositories;
using System.Text.Json.Serialization;

namespace API
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

                // Add services to the container.

                builder.Services.AddControllers().AddJsonOptions(x => x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);
                // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
                builder.Services.AddEndpointsApiExplorer();
                builder.Services.AddToDoServices();
                builder.Services.AddTodosDatabase(builder.Configuration);
                builder.Services.AddSwaggerGen();

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
