using System;
using System.Collections.Generic;
using System.Text;
using FysioEnterprise.Facade.DTOs;

namespace FysioEnterprise.Facade.UseCase
{
    public interface ICancelSessionUseCase
    {
        Task CancelSessionRequest(CancelSessionRequest request);
    }
}
