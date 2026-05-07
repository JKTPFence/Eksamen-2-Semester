using Microsoft.Extensions.DependencyInjection;
using FysioEnterprise.Facade.UseCase.ClientUseCase;
using FysioEnterprise.UseCase.CommandHandler.ClientCommands;
using FysioEnterprise.Facade.UseCase.PromotionUseCase;
using FysioEnterprise.UseCase.CommandHandlers.PromotionCommands;
using FysioEnterprise.Facade.UseCase.SessionUseCase;
using FysioEnterprise.UseCase.CommandHandlers.SessionCommands;

namespace FysioEnterprise.UseCase.DependencyInjection
{
    public static class UseCasesServiceCollectionExtensions
    {
        public static IServiceCollection AddUseCases(this IServiceCollection services)
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
            services.AddScoped<IDeleteSessionUseCase, SessionCommandHandler>();

            //Need to implement EndSession og CancelSession
            services.AddScoped<ICreateSessionUseCase, SessionCommandHandler>();
            services.AddScoped<ICreateSessionUseCase, SessionCommandHandler>();

            return services;
        }
    }
}
