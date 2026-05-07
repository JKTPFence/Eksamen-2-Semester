using FluentResults;
using FysioEnterprise.Domain.Service;
using System;
using System.Collections.Generic;
using System.Text;

namespace FysioEnterprise.Port.Driving.Commands.PromotionCommands
{
    public interface ICreatePromotionCommand
    {
        Task<Result> CreatePromotionAsync(CreatePromotionCommand command);
        public record CreatePromotionCommand(
            Guid PromotionID,
            string PromotionName,
            int PromotionDiscountPercentage,
            DateTime PromotionStartDate,
            DateTime PromotionEndDate,
            ITimeNow TimeNow,
            bool IsActive);
    }
}
