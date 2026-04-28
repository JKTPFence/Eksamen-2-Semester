using FysioEnterprise.Application.Repository.Interfaces;
using FysioEnterprise.Infrastructure.Database;
using FysioEnterprise.Port.Driving.Commands.SessionComands;
using FysioEnterprise.UseCase.Commands;
using Microsoft.EntityFrameworkCore;

namespace FysioEnterprise.Presentation.Service
{
    // Infrastructure/DependencyInjection.cs
    public static class DependencyInjection
    {

        public static IServiceCollection AddInfrastructureServices(
            this IServiceCollection services, IConfiguration config)
        {
            services.AddDbContext<AppDBContext>(options =>
                options.UseSqlServer(config.GetConnectionString("Default")));

           // services.AddScoped<IClientRepository, ClientRepository>();
           // services.AddScoped<ISessionRepository, SessionRepository>();
            return services;
        }
    }
}
