using FysioEnterprise.Facade.Queries;
using FysioEnterprise.Infrastructure.Database;
using FysioEnterprise.Infrastructure.Database.Repository;
using FysioEnterprise.Infrastructure.QueryHandlers;
using FysioEnterprise.UseCase.IRepositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace FysioEnterprise.Infrastructure
{
    public static class InfrastructureServiceCollectionExtensions
    {
        //ConnectionString brug af blazor fra appsettings.json
        public static IServiceCollection AddInfrastructure(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            services.AddDbContext<AppDBContext>(options => options.UseSqlServer(
                configuration.GetConnectionString("YEPPPPPPPOOOOO")));

            RegisterRepositoriesAndQueries(services);
            return services;
        }

        //Til brug til unittests
        public static IServiceCollection AddInfrastructure(
            this IServiceCollection services,
            Action<DbContextOptionsBuilder> configureDb)
        {
            services.AddDbContext<AppDBContext>(options =>
            {
                options.LogTo(Console.WriteLine);
                options.EnableSensitiveDataLogging();
                options.EnableDetailedErrors();
                configureDb(options);
            });

            RegisterRepositoriesAndQueries(services);
            return services;
        }

        private static void RegisterRepositoriesAndQueries(IServiceCollection services)
        {
            //Repos
            services.AddScoped<ISessionRepository, SessionRepository>();
            services.AddScoped<IClientRepository, ClientRepository>();
            services.AddScoped<IPromotionRepository, PromotionRepository>();
            services.AddScoped<IStaffRepository, StaffRepository>();
            services.AddScoped<IClinicRepository, ClinicRepository>();

            //Query Handlers
            services.AddScoped<ISessionQueries, SessionQueriesImpl>();
            services.AddScoped<IClientQueries, ClientQueriesImpl>();
            services.AddScoped<IPromotionQueries, PromotionQueriesImpl>();
            services.AddScoped<ISimpleQueries, SimpleQueriesImpl>();
        }
    }
}
