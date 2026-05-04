using FluentResults;
using FysioEnterprise.Domain.Enums;
using FysioEnterprise.Domain.ValueObjects;

namespace FysioEnterprise.Port.Driving.Commands.SessionComands
{
    public interface ICreateSessionCommand
    {
        Task<Result> CreateSessionAsync(CreateSessionCommand command);

        public record CreateSessionCommand(Guid ClientId, Guid StaffId, DateTime StartTime, DateTime EndTime, Guid SessionRoom, SessionStatusEnum SessionStatus, SessionType SessionType, Guid PromotionID, int? SessionTotalPrice);

    }
}
