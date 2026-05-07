using FluentResults;
using FysioEnterprise.Domain.Service;
using System;
using System.Collections.Generic;
using System.Text;

namespace FysioEnterprise.Port.Driving.Commands.PromotionCommands
{
    public interface IUpdatePromotionCommand
    {
        Task<Result> UpdatePromotionAsync(UpdatePromotionCommand command);
        public record UpdatePromotionCommand(
            Guid PromotionID,
            string PromotionName,
            decimal PromotionDiscountPercentage,
            DateTime PromotionStartDate,
            DateTime PromotionEndDate,
            ITimeNow TimeNow,
            bool IsActive);
    }
}