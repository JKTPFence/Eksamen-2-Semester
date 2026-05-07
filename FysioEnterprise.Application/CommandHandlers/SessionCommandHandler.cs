using FysioEnterprise.Domain.Entities;
using FysioEnterprise.Domain.Exceptions;
using FysioEnterprise.Domain.Service;
using FysioEnterprise.UseCase.IRepositories;
using FluentResults;
using FysioEnterprise.Facade.UseCase.SessionUseCase;
using static FysioEnterprise.Facade.RequestModels.SessionRequests;

namespace FysioEnterprise.UseCase.CommandHandlers.SessionCommands
{
    public class SessionCommandHandler : ICreateSessionUseCase, IUpdateSessionUseCase, IDeleteSessionUseCase
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

        public async Task<Result> CreateSessionAsync(CreateSessionRequest request)
        {
            var clientResult = await _clientRepository.GetClientAsync(request.ClientID);
            if (clientResult.IsFailed)
                return Result.Fail("Client not found.");

            var staffResult = await _staffRepository.GetStaffAsync(request.StaffID);
            if (staffResult.IsFailed)
                return Result.Fail("Staff not found.");

            var roomResult = await _roomRepository.GetRoomAsync(request.SessionRoomID);
            if (roomResult.IsFailed)
                return Result.Fail("Room not found.");

            Result<Promotion> promotionResult = null;
            if (request.PromotionID != Guid.Empty)
            {
                promotionResult = await _promotionRepository.GetPromotionAsync(request.PromotionID);
                if (promotionResult.IsFailed)
                    return Result.Fail("Promotion not found.");
            }

            var existingClientSessions = await _sessionRepository.GetSessionsByClientAsync(request.ClientID);
            var existingStaffSessions = await _sessionRepository.GetSessionsByStaffAsync(request.StaffID);

            try
            {
                var session = Session.Create(
                    clientResult.Value,
                    staffResult.Value,
                    request.SessionInstanceType,
                    roomResult.Value,
                    request.StartTime,
                    request.EndTime,
                    request.SessionTotalPrice,
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

        public async Task<Result> UpdateSessionAsync(UpdateSessionRequest request)
        {
            var sessionResult = await _sessionRepository.GetSessionAsync(request.SessionID);
            if (sessionResult.IsFailed)
                return Result.Fail("Session not found.");

            var session = sessionResult.Value;

            if (request.ClientID != session.SessionClientID)
                return Result.Fail("Session does not belong to the specified client.");

            var existingClientSessions = await _sessionRepository.GetSessionsByClientAsync(request.ClientID);
            var existingStaffSessions = await _sessionRepository.GetSessionsByStaffAsync(request.StaffID);

            try
            {
                  session.UpdateSessionTime(
                    request.StartTime,
                    request.EndTime,
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

        public async Task<Result> DeleteSessionAsync(DeleteSessionRequest request)
        {
            var sessionResult = await _sessionRepository.GetSessionAsync(request.SessionID);
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
