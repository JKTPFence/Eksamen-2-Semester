using FysioEnterprise.Application.Repository.Interfaces;
using FysioEnterprise.Domain.Entities;
using FysioEnterprise.Domain.Exceptions;
using FysioEnterprise.Domain.Service;
using FysioEnterprise.UseCase.Repository.Interfaces;
using FysioEnterprise.Port.Driving.Commands.SessionComands;
using static FysioEnterprise.Port.Driving.Commands.SessionComands.ICreateSessionCommand;
using static FysioEnterprise.Port.Driving.Commands.SessionComands.IUpdateSessionCommand;
using static FysioEnterprise.Port.Driving.Commands.SessionComands.IDeleteSessionCommand;
using FluentResults;

namespace FysioEnterprise.UseCase.Commands
{
    public class SessionCommandHandler : ICreateSessionCommand, IUpdateSessionCommand, IDeleteSessionCommand
    {
        private readonly IClientRepository _clientRepository;
        private readonly IStaffRepository _staffRepository;
        private readonly IRoomRepository _roomRepository;
        private readonly IPromotionRepository _promotionRepository;
        private readonly ISessionRepository _sessionRepository;
        private readonly ITimeNow _now;

        public SessionCommandHandler(
            IClientRepository clientRepository,
            IStaffRepository staffRepository,
            IRoomRepository roomRepository,
            IPromotionRepository promotionRepository,
            ISessionRepository sessionRepository,
            ITimeNow now)
        {
            _clientRepository = clientRepository;
            _staffRepository = staffRepository;
            _roomRepository = roomRepository;
            _promotionRepository = promotionRepository;
            _sessionRepository = sessionRepository;
            _now = now;
        }

        public async Task<Result> CreateSessionAsync(CreateSessionCommand command)
        {
            var clientResult = await _clientRepository.GetClientAsync(command.ClientId);
            if (clientResult.IsFailed)
                return Result.Fail("Client not found.");

            var staffResult = await _staffRepository.GetStaffAsync(command.StaffId);
            if (staffResult.IsFailed)
                return Result.Fail("Staff not found.");

            var roomResult = await _roomRepository.GetRoomAsync(command.SessionRoom);
            if (roomResult.IsFailed)
                return Result.Fail("Room not found.");

            Result<Promotion> promotionResult = null;
            if (command.PromotionID != Guid.Empty)
            {
                promotionResult = await _promotionRepository.GetPromotionAsync(command.PromotionID);
                if (promotionResult.IsFailed)
                    return Result.Fail("Promotion not found.");
            }

            var existingClientSessions = await _sessionRepository.GetSessionsByClientAsync(command.ClientId);
            var existingStaffSessions = await _sessionRepository.GetSessionsByStaffAsync(command.StaffId);

            try
            {
                var session = Session.Create(
                    clientResult.Value,
                    staffResult.Value,
                    command.SessionType,
                    roomResult.Value,
                    command.StartTime,
                    command.EndTime,
                    command.SessionTotalPrice,
                    promotionResult?.Value,
                    existingClientSessions,
                    existingStaffSessions);
                await _sessionRepository.CreateSessionAsync(session);
            }
            catch (DomainException ex)
            {
                return Result.Fail(ex.Message);
            }
            return Result.Ok();
        }

        public async Task<Result> UpdateSessionAsync(UpdateSessionCommand command)
        {
            var sessionResult = await _sessionRepository.GetSessionAsync(command.SessionId);
            if (sessionResult.IsFailed)
                return Result.Fail("Session not found.");

            var session = sessionResult.Value;

            if (command.ClientId != session.SessionClientID)
                return Result.Fail("Session does not belong to the specified client.");

            var existingClientSessions = await _sessionRepository.GetSessionsByClientAsync(command.ClientId);
            var existingStaffSessions = await _sessionRepository.GetSessionsByStaffAsync(command.StaffId);

            try
            {
                  session.UpdateSessionTime(
                    command.StartTime,
                    command.EndTime,
                    existingClientSessions.Where(s => s.SessionID != session.SessionID),
                    existingStaffSessions.Where(s => s.SessionID != session.SessionID)
                    );

            }

            catch (DomainException ex)
            {
                return Result.Fail(ex.Message);
            }

            await _sessionRepository.UpdateSessionAsync(session);
            return Result.Ok();
        }

        public async Task<Result> DeleteSessionAsync(DeleteSessionCommand command)
        {
            var sessionResult = await _sessionRepository.GetSessionAsync(command.SessionId);
            if (sessionResult.IsFailed)
                return Result.Fail("Session not found.");

            try
            {
                sessionResult.Value.CancelSession();
            }
            catch (DomainException ex)
            {
                return Result.Fail(ex.Message);
            }

            await _sessionRepository.UpdateSessionAsync(sessionResult.Value);
            return Result.Ok();
        }
    }
}
