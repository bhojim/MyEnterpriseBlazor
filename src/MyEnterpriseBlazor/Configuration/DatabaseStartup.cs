using System;
using MyEnterpriseBlazor.Infrastructure.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Hosting;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace MyEnterpriseBlazor.Configuration
{
    public static class DatabaseConfiguration
    {
        public static IServiceCollection AddDatabaseModule(this IServiceCollection services, IConfiguration configuration)
        {
            var entityFrameworkConfiguration = configuration.GetSection("EntityFramework");

            var connection = configuration.GetConnectionString("AppDbContext");

            services.AddDbContext<ApplicationDatabaseContext>(options => options.UseSqlServer(connection));

            services.AddScoped<DbContext>(provider => provider.GetService<ApplicationDatabaseContext>());

            return services;
        }

        public static IApplicationBuilder UseApplicationDatabase(this IApplicationBuilder app,
            IServiceProvider serviceProvider, IHostEnvironment environment)
        {
            if (environment.IsDevelopment() || environment.IsProduction())
            {
                using (var scope = serviceProvider.CreateScope())
                {
                    var context = scope.ServiceProvider.GetRequiredService<ApplicationDatabaseContext>();
                    context.Database.OpenConnection();
                    context.Database.EnsureCreated();
                }
            }

            return app;
        }
    }
}
