using FysioEnterprise.Presentation.Service.Helpers;

namespace FysioEnterprise.Presentation.Service
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddPresentationServices(
            this IServiceCollection services, IConfiguration config)
        {
            services.AddScoped<LogInContext>();
            services.AddScoped<NotificationHelper>();
            return services;
        }
    }
}
