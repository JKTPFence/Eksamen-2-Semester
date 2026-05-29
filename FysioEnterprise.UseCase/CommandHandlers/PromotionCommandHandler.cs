using System.Diagnostics;
using FluentResults;
using FysioEnterprise.Domain.Entities;
using FysioEnterprise.Domain.Exceptions;
using FysioEnterprise.Domain.Service;
using FysioEnterprise.Facade.UseCase.PromotionUseCase;
using FysioEnterprise.UseCase.IRepositories;
using static FysioEnterprise.Facade.RequestModels.PromotionRequests;
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
                return Result.Fail("For at lave en kampagne skal der være indhold");
            
            var promotion = await _promotionRepository.GetPromotionAsync(request.PromotionID);
            if (promotion.IsSuccess)
                return Result.Fail("Der findes en anden kampagne med det samme ID");
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
                return ex switch
                {
                    UserInvalidInputException => Result.Fail("Et input var ikke korrekt" + ex.Message),
                    ValidationException => Result.Fail("Der er sket en valideringsfejl" + ex.Message),
                    _ => Result.Fail("Der er sket en uforventet fejl " + ex.Message) // Fallback catch-all for base DomainException
                };
            }
            catch (Exception ex) // Catch-all for any other unexpected exceptions (typically SQL or Infrastructure exceptions)
            {
                System.Diagnostics.Debug.WriteLine($"Infrastructure Failure: {ex.Message}");
                return Result.Fail("An unexpected system error occurred." + ex.Message);
            }
        }
        public async Task<Result> UpdatePromotionAsync(UpdatePromotionRequest request)
        {
            if (request == null)
                return Result.Fail("For at lave en kampagne skal der være indhold");
            if (request.PromotionID == Guid.Empty)
                return Result.Fail("Ingen kampagne er fundet med dette ID");
            if (request.Name == null)
                return Result.Fail("En kampagne skal have et navn");
            if (request.DiscountPercentage <= 0)
                return Result.Fail("En kampagne skal have en rabatprocent");
           
            var validationResult = TimeValidationService.ValidateTime(
                           request.Name,
                           request.StartDate,
                           request.EndDate,
                           _timeNow.Now());
            if (validationResult.IsFailed)
                return validationResult;

            var promotion = await _promotionRepository.GetPromotionAsync(request.PromotionID);
            if (promotion.IsFailed)
                return Result.Fail("Ingen kampagne er blevet fundet");

            try
            {
                promotion.Value.UpdatePromotion(
                    request.Name,
                    request.DiscountPercentage,
                    request.StartDate,
                    request.EndDate);
                await _promotionRepository.UpdatePromotionAsync(promotion.Value);

            }
            catch (DomainException ex)
            {
                return ex switch
                {
                    UserInvalidInputException => Result.Fail("Et input var ikke korrekt" + ex.Message),
                    ValidationException => Result.Fail("Der er sket en valideringsfejl" + ex.Message),
                    _ => Result.Fail("Der er sket en uforventet fejl " + ex.Message) // Fallback catch-all for base DomainException
                };
            }
            catch (Exception ex) // Catch-all for any other unexpected exceptions (typically SQL or Infrastructure exceptions)
            {
                System.Diagnostics.Debug.WriteLine($"Infrastructure Failure: {ex.Message}");
                return Result.Fail("An unexpected system error occurred." + ex.Message);
            }

            return Result.Ok();
        }
        public async Task<Result> DeletePromotionAsync(DeletePromotionRequest request)
        {
            if (request == null)
                return Result.Fail("For at slette en kampagne, skal vi have oplysningerne på hvilken der skal slettes");
            if (request.PromotionID == Guid.Empty)
                return Result.Fail("Fejl i inputtet af kampagneoplysningerne");
                
            var promotion = await _promotionRepository.GetPromotionAsync(request.PromotionID);
            if(promotion.IsFailed)
            {
                var result = Result.Fail("Ingen kampagne er blevet fundet");
                Debug.WriteLine($"Returning: {result.Errors[0].Message}");
                return result;
            }

            await _promotionRepository.DeletePromotionAsync(request.PromotionID);
            return Result.Ok();
        }
    }
}
