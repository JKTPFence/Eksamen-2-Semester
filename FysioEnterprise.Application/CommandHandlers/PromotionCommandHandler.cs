using FysioEnterprise.UseCase.IRepositories;
using FysioEnterprise.Domain.Entities;
using FluentResults;
using FysioEnterprise.Facade.UseCase.PromotionUseCase;
using static FysioEnterprise.Facade.RequestModels.PromotionRequests;
using FysioEnterprise.Domain.Service;
namespace FysioEnterprise.UseCase.CommandHandlers.PromotionCommands
{
    public class PromotionCommandHandler : ICreatePromotionUseCase, IUpdatePromotionUseCase, IDeletePromotionUseCase
    {
        private readonly IPromotionRepository _promotionRepository;
        public PromotionCommandHandler(IPromotionRepository promotionRepository)
        {
            _promotionRepository = promotionRepository;
        }
        public async Task<Result> CreatePromotionAsync(CreatePromotionRequest request)
        {
            if (request == null)
                return Result.Fail("Request cannot be null.");
            if (request.Name == null)
                return Result.Fail("Promotion name cannot be null.");
            if (request.DiscountPercentage <= 0)
                return Result.Fail("Discount percentage must be greater than zero.");
            if (request.StartDate == default)
                return Result.Fail("Start date must be a valid date.");
            if (request.EndDate == default)
                return Result.Fail("End date must be a valid date.");
            if (request.EndDate <= request.StartDate)
                return Result.Fail("End date must be after start date.");
            
            var promotion = await _promotionRepository.GetPromotionAsync(request.PromotionID);
            if (promotion != null)
                return Result.Fail("Promotion with the same ID already exists.");

            promotion = new Promotion(
                    request.Name,
                    request.DiscountPercentage,
                    request.StartDate,
                    request.EndDate,
                    new CurrentDateTime());

            return await _promotionRepository.CreatePromotionAsync(promotion);
        }
        public async Task<Result> UpdatePromotionAsync(UpdatePromotionRequest request)
        {
            if (request == null)
                return Result.Fail("Request cannot be null.");
            if (request.PromotionID == Guid.Empty)
                return Result.Fail("Promotion ID cannot be empty.");
            if (request.Name == null)
                return Result.Fail("Promotion name cannot be null.");
            if (request.DiscountPercentage <= 0)
                return Result.Fail("Discount percentage must be greater than zero.");
            if (request.StartDate == default)
                return Result.Fail("Start date must be a valid date.");
            if (request.EndDate == default)
                return Result.Fail("End date must be a valid date.");
            if (request.EndDate <= request.StartDate)
                return Result.Fail("End date must be after start date.");

            var promotion = await _promotionRepository.GetPromotionAsync(request.PromotionID);
            if (promotion == null)
                return Result.Fail("Promotion not found.");
                
            promotion.UpdatePromotion(
                    request.Name,
                    request.DiscountPercentage,
                    request.StartDate,
                    request.EndDate);
                
            await _promotionRepository.UpdatePromotionAsync(promotion);
            return Result.Ok();
        }
        public async Task<Result> DeletePromotionAsync(DeletePromotionRequest request)
        {
            if (request == null)
                return Result.Fail("Command cannot be null.");
            if (request.PromotionID == Guid.Empty)
                return Result.Fail("Promotion ID cannot be empty.");
                
            var promotion = await _promotionRepository.GetPromotionAsync(request.PromotionID);
            if(promotion == null)
                return Result.Fail("Promotion not found.");

            await _promotionRepository.DeletePromotionAsync(request.PromotionID);
            return Result.Ok();
        }
    }
}
