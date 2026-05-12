using FysioEnterprise.Facade.DTOs;
using FysioEnterprise.Facade.RequestModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace FysioEnterprise.Facade.Queries
{
    public interface IEarningsReportQuery
    {
        Task<EarningsReportDTO> GetEarningsReportAsync(EarningsReportRequestDTO request);
    }
}
