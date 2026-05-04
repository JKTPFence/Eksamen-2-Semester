using FysioEnterprise.UseCase.Repository.Interfaces;
using FysioEnterprise.Domain.Entities;
using FysioEnterprise.Port.Driving.Commands.PromotionCommands;
namespace FysioEnterprise.UseCase.CommandHandlers.PromotionCommands
{
    public class PromotionCommandHandler : ICreatePromotionCommand, IUpdatePromotionCommand, IDeletePromotionCommand
    {
        private readonly IPromotionRepository _promotionRepository;
        public PromotionCommandHandler(IPromotionRepository promotionRepository)
        {
            _promotionRepository = promotionRepository;
        }
        public async Task CreatePromotionAsync(ICreatePromotionCommand.CreatePromotionCommand command)
        {
            try
            {
                if (command.PromotionName == null)
                    throw new ArgumentNullException(nameof(command), "Command cannot be null.");
                if (command.PromotionDiscountPercentage <= 0)
                    throw new ArgumentException("Discount percentage must be greater than zero.", nameof(command));
                if (command.PromotionStartDate == default)
                    throw new ArgumentException("Start date must be a valid date.", nameof(command));
                if (command.PromotionEndDate == default)
                    throw new ArgumentException("End date must be a valid date.", nameof(command));
                if (command.PromotionEndDate <= command.PromotionStartDate)
                    throw new ArgumentException("End date must be after start date.", nameof(command));
                var promotion = await _promotionRepository.GetPromotionAsync(command.PromotionID);
                promotion = new Promotion(
                    command.PromotionName,
                    command.PromotionDiscountPercentage,
                    command.PromotionStartDate,
                    command.PromotionEndDate,
                    command.TimeNow);
                await _promotionRepository.CreatePromotionAsync(promotion);
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while creating the promotion.", ex);
            }
        }
        public async Task UpdatePromotionAsync(IUpdatePromotionCommand.UpdatePromotionCommand command)
        {
            try
            {
                if (command.PromotionID == Guid.Empty)
                    throw new ArgumentException("Promotion ID cannot be empty.", nameof(command));
                if (command.PromotionName == null)
                    throw new ArgumentNullException(nameof(command), "Command cannot be null.");
                if (command.PromotionDiscountPercentage <= 0)
                    throw new ArgumentException("Discount percentage must be greater than zero.", nameof(command));
                if (command.PromotionStartDate == default)
                    throw new ArgumentException("Start date must be a valid date.", nameof(command));
                if (command.PromotionEndDate == default)
                    throw new ArgumentException("End date must be a valid date.", nameof(command));
                if (command.PromotionEndDate <= command.PromotionStartDate)
                    throw new ArgumentException("End date must be after start date.", nameof(command));
                var promotion = await _promotionRepository.GetPromotionAsync(command.PromotionID) ?? throw new Exception("Promotion not found.");
                promotion.UpdatePromotion(
                    command.PromotionName,
                    command.PromotionDiscountPercentage,
                    command.PromotionStartDate,
                    command.PromotionEndDate);
                await _promotionRepository.UpdatePromotionAsync(promotion);
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while updating the promotion.", ex);
            }
        }
        public async Task DeletePromotionAsync(IDeletePromotionCommand.DeletePromotionCommand command)
        {
            try
            {
                if (command.PromotionID == Guid.Empty)
                    throw new ArgumentException("Promotion ID cannot be empty.", nameof(command));
                var promotion = await _promotionRepository.GetPromotionAsync(command.PromotionID) ?? throw new Exception("Promotion not found.");
                await _promotionRepository.DeletePromotionAsync(command.PromotionID);
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while deleting the promotion.", ex);
            }
        }
    }
}
