using FluentResults;
using FysioEnterprise.Domain.Enums;
using FysioEnterprise.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Text;

namespace FysioEnterprise.Port.Driving.Commands.SessionComands
{
    public interface IDeleteSessionCommand
    {
        Task<Result> DeleteSessionAsync(DeleteSessionCommand command);

        public record DeleteSessionCommand(Guid SessionId);
    }
}
