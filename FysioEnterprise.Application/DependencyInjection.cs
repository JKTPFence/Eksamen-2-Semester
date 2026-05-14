using FysioEnterprise.Domain.Service;
using FysioEnterprise.Domain.Service.PricingService;
using FysioEnterprise.Facade.UseCase.ClientUseCase;
using FysioEnterprise.Facade.UseCase.PromotionUseCase;
using FysioEnterprise.Facade.UseCase.SessionUseCase;
using FysioEnterprise.UseCase.CommandHandler.ClientCommands;
using FysioEnterprise.UseCase.CommandHandlers.PromotionCommands;
using FysioEnterprise.UseCase.CommandHandlers.SessionCommands;
using FysioEnterprise.UseCase.Service;
using Microsoft.Extensions.DependencyInjection;

namespace FysioEnterprise.UseCase.DependencyInjection
{
    public static class UseCasesServiceCollectionExtensions
    {
        public static IServiceCollection AddUseCaseServices(this IServiceCollection services)
        {
            //Client Use Case
            services.AddScoped<ICreateClientUseCase, ClientCommandHandler>();
            services.AddScoped<IDeleteClientUseCase, ClientCommandHandler>();
            services.AddScoped<IUpdateClientUseCase, ClientCommandHandler>();
            services.AddScoped<IUpdateClientPrefferedStaffUseCase, ClientCommandHandler>();
            services.AddScoped<IUpdateClientNoteUseCase, ClientCommandHandler>();

            //Promotion Use Case
            services.AddScoped<ICreatePromotionUseCase, PromotionCommandHandler>();
            services.AddScoped<IUpdatePromotionUseCase, PromotionCommandHandler>();
            services.AddScoped<IDeletePromotionUseCase, PromotionCommandHandler>();

            //Session Use Case
            services.AddScoped<ICreateSessionUseCase, SessionCommandHandler>();
            services.AddScoped<IUpdateSessionUseCase, SessionCommandHandler>();
            services.AddScoped<IMarkSessionAsNoShowUseCase, SessionCommandHandler>();
            services.AddScoped<ICreateSessionUseCase, SessionCommandHandler>();
            services.AddScoped<ICreateSessionUseCase, SessionCommandHandler>();
            services.AddScoped<IPricingStrategyFactory, PricingStrategyFactoryService>();
            services.AddScoped<PriceCalculator>();
            services.AddScoped<ITimeNow, CurrentDateTime>();

            return services;
        }
    }
}
