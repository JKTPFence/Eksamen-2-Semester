using FysioEnterprise.Application.Repository.Interfaces;
using FysioEnterprise.Domain.Entities;
using FysioEnterprise.Domain.Exceptions;
using FysioEnterprise.Domain.Service;
using FysioEnterprise.UseCase.Repository.Interfaces;
using FysioEnterprise.Port.Driving.Commands.SessionComands;
using static FysioEnterprise.Port.Driving.Commands.SessionComands.ICreateSessionCommand;
using static FysioEnterprise.Port.Driving.Commands.SessionComands.IUpdateSessionCommand;
using static FysioEnterprise.Port.Driving.Commands.SessionComands.IDeleteSessionCommand;

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

        public async Task CreateSessionAsync(CreateSessionCommand command)
        {
            var client = await _clientRepository.GetClientAsync(command.ClientId);
            var staff = await _staffRepository.GetStaffAsync(command.StaffId);
            var room = await _roomRepository.GetRoomAsync(command.SessionRoom);
            var promotion = command.PromotionID != Guid.Empty
                ? await _promotionRepository.GetPromotionAsync(command.PromotionID)
                : null;

            // Load existing sessions for overlap check
            var existingClientSessions = await _sessionRepository.GetSessionsByClientAsync(command.ClientId);
            var existingStaffSessions = await _sessionRepository.GetSessionsByStaffAsync(command.StaffId);

            var session = Session.Create(
                client, staff, command.SessionType, room,
                command.StartTime, command.EndTime,
                command.SessionTotalPrice, promotion,
                existingClientSessions,
                existingStaffSessions
            );

            await _sessionRepository.CreateSessionAsync(session);
        }

        public async Task UpdateSessionAsync(UpdateSessionCommand command)
        {
            // Load
            var session = await _sessionRepository.GetSessionAsync(command.SessionId);
            var client = await _clientRepository.GetClientAsync(command.ClientId);
            var staff = await _staffRepository.GetStaffAsync(command.StaffId);
            var room = await _roomRepository.GetRoomAsync(command.SessionRoom);
            var promotion = command.PromotionID != Guid.Empty
                ? await _promotionRepository.GetPromotionAsync(command.PromotionID)
                : null;

            if (command.ClientId != session.SessionClientID)
                throw new OwnershipException("Session does not belong to the specified client.");

            // Load existing sessions for overlap check
            var existingClientSessions = await _sessionRepository.GetSessionsByClientAsync(command.ClientId);
            var existingStaffSessions = await _sessionRepository.GetSessionsByStaffAsync(command.StaffId);

            // Do
            session.UpdateSessionTime(
                command.StartTime,
                command.EndTime,
                existingClientSessions.Where(s => s.SessionID != session.SessionID),
                existingStaffSessions.Where(s => s.SessionID != session.SessionID)
            );

            // Save
            await _sessionRepository.UpdateSessionAsync(session);
        }

        public async Task DeleteSessionAsync(DeleteSessionCommand command)
        {
            // Load
            var session = await _sessionRepository.GetSessionAsync(command.SessionId);
            // Do
            session.CancelSession();
            // Save
            await _sessionRepository.UpdateSessionAsync(session);
        }
    }
}
