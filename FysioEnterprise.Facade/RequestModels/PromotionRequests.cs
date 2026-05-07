using System;
using System.Collections.Generic;
using System.Text;

namespace FysioEnterprise.Facade.RequestModels
{
    public class PromotionRequests
    {
        public record CreatePromotionRequest(
            Guid PromotionID,
            string Name,
            decimal DiscountPercentage,
            DateTime StartDate,
            DateTime EndDate);
        public record UpdatePromotionRequest(
            Guid PromotionID,
            string Name,
            decimal DiscountPercentage,
            DateTime StartDate,
            DateTime EndDate);
        public record DeletePromotionRequest(
            Guid PromotionID);
    }
}