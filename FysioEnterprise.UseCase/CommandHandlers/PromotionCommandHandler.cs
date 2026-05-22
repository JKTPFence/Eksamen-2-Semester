using FysioEnterprise.UseCase.IRepositories;
using FysioEnterprise.Domain.Entities;
using FluentResults;
using FysioEnterprise.Facade.UseCase.PromotionUseCase;
using static FysioEnterprise.Facade.RequestModels.PromotionRequests;
using FysioEnterprise.Domain.Service;
using FysioEnterprise.Domain.Exceptions;
namespace FysioEnterprise.UseCase.CommandHandlers.PromotionCommands
{
    public class PromotionCommandHandler : ICreatePromotionUseCase, IUpdatePromotionUseCase, IDeletePromotionUseCase
    {
        private readonly IPromotionRepository _promotionRepository;
        private readonly ITimeNow _timeNow;
        public PromotionCommandHandler(IPromotionRepository promotionRepository, ITimeNow timeNow)
        {
            _promotionRepository = promotionRepository;
            _timeNow = timeNow;
        }
        public async Task<Result> CreatePromotionAsync(CreatePromotionRequest request)
        {
            if (request == null)
                return Result.Fail("Request cannot be null.");
            
            var promotion = await _promotionRepository.GetPromotionAsync(request.PromotionID);
            if (promotion != null)
                return Result.Fail("Promotion with the same ID already exists.");
            
            var validationResult = TimeValidationService.ValidateTime(
                request.Name,
                request.StartDate,
                request.EndDate,
                DateTime.Now);
            if (validationResult.IsFailed)
                return validationResult;

            try
            {
                promotion = Promotion.Create(
                        request.Name,
                        request.DiscountPercentage,
                        request.StartDate,
                        request.EndDate);

                return await _promotionRepository.CreatePromotionAsync(promotion.Value);
            }
            catch (DomainException ex)
            {
                return Result.Fail(ex.Message);
            }
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
           
            var validationResult = TimeValidationService.ValidateTime(
                           request.Name,
                           request.StartDate,
                           request.EndDate,
                           DateTime.Now);
            if (validationResult.IsFailed)
                return validationResult;

            var promotion = await _promotionRepository.GetPromotionAsync(request.PromotionID);
            if (promotion == null)
                return Result.Fail("Promotion not found.");
                
            promotion.Value.UpdatePromotion(
                    request.Name,
                    request.DiscountPercentage,
                    request.StartDate,
                    request.EndDate);
                
            await _promotionRepository.UpdatePromotionAsync(promotion.Value);
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
