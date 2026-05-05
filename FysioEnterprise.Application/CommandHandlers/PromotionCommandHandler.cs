using FysioEnterprise.UseCase.Repository.Interfaces;
using FysioEnterprise.Domain.Entities;
using FysioEnterprise.Port.Driving.Commands.PromotionCommands;
using FluentResults;
namespace FysioEnterprise.UseCase.CommandHandlers.PromotionCommands
{
    public class PromotionCommandHandler : ICreatePromotionCommand, IUpdatePromotionCommand, IDeletePromotionCommand
    {
        private readonly IPromotionRepository _promotionRepository;
        public PromotionCommandHandler(IPromotionRepository promotionRepository)
        {
            _promotionRepository = promotionRepository;
        }
        public async Task<Result> CreatePromotionAsync(ICreatePromotionCommand.CreatePromotionCommand command)
        {
            if (command == null)
                return Result.Fail("Command cannot be null.");
            if (command.PromotionName == null)
                return Result.Fail("Promotion name cannot be null.");
            if (command.PromotionDiscountPercentage <= 0)
                return Result.Fail("Discount percentage must be greater than zero.");
            if (command.PromotionStartDate == default)
                return Result.Fail("Start date must be a valid date.");
            if (command.PromotionEndDate == default)
                return Result.Fail("End date must be a valid date.");
            if (command.PromotionEndDate <= command.PromotionStartDate)
                return Result.Fail("End date must be after start date.");
            
            var promotion = await _promotionRepository.GetPromotionAsync(command.PromotionID);
            if (promotion != null)
                return Result.Fail("Promotion with the same ID already exists.");

            promotion = new Promotion(
                    command.PromotionName,
                    command.PromotionDiscountPercentage,
                    command.PromotionStartDate,
                    command.PromotionEndDate,
                    command.TimeNow);

            return await _promotionRepository.CreatePromotionAsync(promotion);
        }
        public async Task<Result> UpdatePromotionAsync(IUpdatePromotionCommand.UpdatePromotionCommand command)
        {
            if (command == null)
                return Result.Fail("Command cannot be null.");
            if (command.PromotionID == Guid.Empty)
                return Result.Fail("Promotion ID cannot be empty.");
            if (command.PromotionName == null)
                return Result.Fail("Promotion name cannot be null.");
            if (command.PromotionDiscountPercentage <= 0)
                return Result.Fail("Discount percentage must be greater than zero.");
            if (command.PromotionStartDate == default)
                return Result.Fail("Start date must be a valid date.");
            if (command.PromotionEndDate == default)
                return Result.Fail("End date must be a valid date.");
            if (command.PromotionEndDate <= command.PromotionStartDate)
                return Result.Fail("End date must be after start date.");

            var promotion = await _promotionRepository.GetPromotionAsync(command.PromotionID);
            if (promotion == null)
                return Result.Fail("Promotion not found.");
                
            promotion.UpdatePromotion(
                    command.PromotionName,
                    command.PromotionDiscountPercentage,
                    command.PromotionStartDate,
                    command.PromotionEndDate);
                
            await _promotionRepository.UpdatePromotionAsync(promotion);
            return Result.Ok();
        }
        public async Task<Result> DeletePromotionAsync(IDeletePromotionCommand.DeletePromotionCommand command)
        {
            if (command == null)
                return Result.Fail("Command cannot be null.");
            if (command.PromotionID == Guid.Empty)
                return Result.Fail("Promotion ID cannot be empty.");
                
            var promotion = await _promotionRepository.GetPromotionAsync(command.PromotionID);
            if(promotion == null)
                return Result.Fail("Promotion not found.");

            await _promotionRepository.DeletePromotionAsync(command.PromotionID);
            return Result.Ok();
        }
    }
}
