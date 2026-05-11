using FysioEnterprise.Facade.DTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace FysioEnterprise.Facade.Queries
{
    public interface IPromotionQueries
    {
        /// <summary>
        ///     Som bruger ønsker jeg at systemet kan håndtere kampagner og fødselsdagstilbud for at spare penge
        /// </summary>
        /// <returns></returns>
        Task<List<PromotionDTO>> GetAllPromotionsAsync();

        /// <summary>
        ///     Som bruger ønsker jeg at systemet kan håndtere kampagner og fødselsdagstilbud for at spare penge
        /// </summary>
        /// <returns></returns>
        Task<List<PromotionDTO>> GetAllActivePromotionsAsync();

        /// <summary>
        ///     Som bruger ønsker jeg at systemet kan håndtere kampagner og fødselsdagstilbud for at spare penge
        /// </summary>
        /// <returns></returns>
        Task<List<PromotionDTO>> GetAllInActivePromotionsAsync();

        /// <summary>
        ///     Som receptionist vil jeg kunne se alle sessioner for en given kunde, så jeg kan få et overblik over kundens bookinger.
        /// </summary>
        /// <param name="promotionId"></param>
        /// <returns></returns>
        Task<PromotionDTO?> GetPromotionByIdAsync(Guid promotionId);
    }
}
