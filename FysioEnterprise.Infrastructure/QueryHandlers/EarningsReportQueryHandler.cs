using FysioEnterprise.Domain.ValueObjects;
using FysioEnterprise.Facade.DTOs;
using FysioEnterprise.Facade.Queries;
using FysioEnterprise.Facade.RequestModels;
using FysioEnterprise.UseCase.IRepositories;
using System;
using System.Collections.Generic;
using System.Text;

namespace FysioEnterprise.Infrastructure.QueryHandlers
{
    public class EarningsReportQueryImpl : IEarningsReportQuery
    {
        private readonly ISessionRepository _sessionRepository;

        public EarningsReportQueryImpl(ISessionRepository sessionRepository)
        {
            _sessionRepository = sessionRepository;
        }

        public async Task<EarningsReportDTO> GetEarningsReportAsync(EarningsReportRequestDTO request)
        {
            var sessions = await _sessionRepository
                .GetCompletedSessionsInRangeAsync(request.From, request.To);

            return new EarningsReportDTO(
                request.From,
                request.To,
                sessions.Sum(s => s.priceTotal?.Value ?? 0D),

                sessions.Count,
                sessions.Any()
                    ? sessions.Average(s => s.priceTotal?.Value ?? 0D)
                    : 0D
            );
        }

    }
}
