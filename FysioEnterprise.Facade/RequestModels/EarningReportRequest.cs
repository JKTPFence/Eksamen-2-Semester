using System;
using System.Collections.Generic;
using System.Text;

namespace FysioEnterprise.Facade.RequestModels
{
    public record EarningsReportRequestDTO(
        DateTime From,
        DateTime To);
    
}
