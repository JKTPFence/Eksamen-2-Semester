using FysioEnterprise.Domain.Entities;
using FysioEnterprise.Domain.Enums;
using FysioEnterprise.Domain.Service.PricingService;
using FysioEnterprise.Facade.DTOs;
using FysioEnterprise.Facade.Queries;
using FysioEnterprise.Facade.RequestModels;
using FysioEnterprise.UseCase.IRepositories;

namespace FysioEnterprise.Infrastructure.QueryHandlers
{
    public class EarningsReportQueryImpl : IEarningsReportQuery
    {
        private readonly ISessionRepository _sessionRepository;
        private readonly IClientRepository _clientRepository;
        private readonly IPricingStrategyFactory _pricingFactory;
        private readonly ISessionTypeRepository _sessionTypeRepository;
        private readonly IPromotionRepository _promotionRepository;
        private readonly IStaffRepository _staffRepository;

        public EarningsReportQueryImpl(ISessionRepository sessionRepository, IClientRepository clientRepository, IPricingStrategyFactory pricingFactory, ISessionTypeRepository sessionTypeRepository, IPromotionRepository promotionRepository, IStaffRepository staffRepository)
        {
            _sessionRepository = sessionRepository;
            _clientRepository = clientRepository;
            _pricingFactory = pricingFactory;
            _sessionTypeRepository = sessionTypeRepository;
            _promotionRepository = promotionRepository;
            _staffRepository = staffRepository;
        }

        public async Task<EarningsReportDTO> GetEarningsReportAsync(EarningsReportRequestDTO request)
        {
            var sessions = await _sessionRepository
            .GetSessionsInRangeAsync(request.From, request.To);

            var clientIds = sessions.Select(s => s.SessionClientID).Distinct().ToList();
            var staffIds = sessions.Select(s => s.SessionStaffID).Distinct().ToList();
            var sessionTypeIds = sessions.Select(s => s.SessionInstanceTypeID).Distinct().ToList();
            var promotionIds = sessions
                .Where(s => s.SessionPromotion.HasValue)
                .Select(s => s.SessionPromotion!.Value).Distinct().ToList();

            var clients = new Dictionary<Guid, Client>();
            foreach (var id in clientIds)
            {
                var c = await _clientRepository.GetClientAsync(id);
                if (c is not null) clients[id] = c.Value;
            }

            var sessionTypes = new Dictionary<Guid, SessionType>();
            foreach (var id in sessionTypeIds)
            {
                var st = await _sessionTypeRepository.GetSessionTypeAsync(id);
                if (st is not null) sessionTypes[id] = st.Value;
            }

            var promotions = new Dictionary<Guid, Promotion>();
            foreach (var id in promotionIds)
            {
                var p = await _promotionRepository.GetPromotionAsync(id);
                if (p is not null) promotions[id] = p.Value;
            }

            var staff = new Dictionary<Guid, Staff>();
            foreach (var id in staffIds)
            {
                var s = await _staffRepository.GetStaffAsync(id);
                if (s is not null) staff[id] = s.Value;
            }

            var discounts = new System.Collections.Concurrent.ConcurrentBag<DiscountProjectionDTO>();

            await Parallel.ForEachAsync(
                sessions,
                new ParallelOptions { MaxDegreeOfParallelism = Environment.ProcessorCount },
                async (session, _) =>
                {
                    if (!clients.TryGetValue(session.SessionClientID, out var client))
                    {
                        await Task.CompletedTask;
                        return;
                    }

                    if (!sessionTypes.TryGetValue(session.SessionInstanceTypeID, out var sessionType))
                    {
                        await Task.CompletedTask;
                        return;
                    }

                    if (!staff.TryGetValue(session.SessionStaffID, out var staffs))
                    {
                        await Task.CompletedTask;
                        return;
                    }

                    Promotion? promotion = null;
                    if (session.SessionPromotion.HasValue)
                        promotions.TryGetValue(session.SessionPromotion.Value, out promotion);

                    var originalPrice = sessionType.SessionTypePrice.Value;
                    var discountAmount = 0d;
                    var finalPrice = 0d;
                    var upcomingPrice = 0d;
                    var lostRevenue = 0d;
                    var strategyName = "Ingen rabat";

                    switch (session.SessionStatus)
                    {
                        case SessionStatusEnum.Completed:
                            var completedResult = await _pricingFactory.BuildStrategies(client, promotion, sessionType);
                            finalPrice = completedResult.Value;
                            discountAmount = Math.Max(0, originalPrice - finalPrice);
                            strategyName = discountAmount > 0
                                ? GetWinningStrategyName(client, promotion, sessionType)
                                : "Ingen rabat";
                            break;

                        case SessionStatusEnum.Active:
                            var activeResult = await _pricingFactory.BuildStrategies(client, promotion, sessionType);
                            upcomingPrice = activeResult.Value;
                            discountAmount = Math.Max(0, originalPrice - upcomingPrice);
                            strategyName = discountAmount > 0
                                ? GetWinningStrategyName(client, promotion, sessionType)
                                : "Ingen rabat";
                            break;

                        case SessionStatusEnum.Cancelled:
                        case SessionStatusEnum.NoShow:
                            lostRevenue = originalPrice;
                            break;
                    }

                    discounts.Add(new DiscountProjectionDTO(
                        SessionId: session.Id,
                        SessionStart: session.SessionTimeSlot.From,
                        ClientName: $"{client.ClientFirstName} {client.ClientLastName}",
                        StaffName: $"{staffs.StaffFirstName} {staffs.StaffLastName}",
                        SessiontypeName: sessionType.SessionTypeName,
                        SessionStatus: session.SessionStatus,
                        StrategyName: strategyName,
                        OriginalPrice: originalPrice,
                        DiscountAmount: discountAmount, 
                        FinalPrice: finalPrice, 
                        LostRevenue: lostRevenue, 
                        UpcomingPrice: upcomingPrice));

                    await Task.CompletedTask;
                });

            var discountList = discounts.ToList();

            return new EarningsReportDTO(
                From: request.From,
                To: request.To,
                TotalRevenue: discountList
                .Where(d => d.SessionStatus == SessionStatusEnum.Completed)
                .Sum(d => d.FinalPrice),
            SessionCount: discountList
                .Count(d => d.SessionStatus == SessionStatusEnum.Completed),
            AverageRevenue: discountList.Any(d => d.SessionStatus == SessionStatusEnum.Completed)
                ? discountList
                    .Where(d => d.SessionStatus == SessionStatusEnum.Completed)
                    .Average(d => d.FinalPrice)
                : 0,
            Discounts: discountList);
        }

        private string GetWinningStrategyName(
        Client client, Promotion? promotion, SessionType sessionType)
        {
            if (promotion is not null)
                return $"Kampagne: {promotion.PromotionName}";

            if (client.ClientLoyaltyLevel.LoyaltyLevelDiscountPercentage > 0)
                return $"Loyalitetsniveau: {client.ClientLoyaltyLevel.LoyaltyLevelName}";

            if (client.HasUsedBirthdayDiscountThisYear == true)
                return $"Birthday discount: {client.ClientBirthDate}";

            return "Rabat";
        }

    }
}
