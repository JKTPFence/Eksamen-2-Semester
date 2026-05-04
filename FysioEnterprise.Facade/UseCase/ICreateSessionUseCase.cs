using System;
using System.Collections.Generic;
using System.Text;
using FysioEnterprise.Facade.DTOs;

namespace FysioEnterprise.Facade.UseCase
{
    public interface ICreateSessionUseCase
    {
        Task CreateSessionRequest(CreateSessionRequest request);
    }
}
