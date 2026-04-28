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
        private readonly ISessionOverlap _overlapCheck;
        private readonly ITimeNow _now;

        public SessionCommandHandler(
            IClientRepository clientRepository,
            IStaffRepository staffRepository,
            IRoomRepository roomRepository,
            IPromotionRepository promotionRepository,
            ISessionRepository sessionRepository,
            ISessionOverlap overlapCheck,
            ITimeNow now)
        {
            _clientRepository = clientRepository;
            _staffRepository = staffRepository;
            _roomRepository = roomRepository;
            _promotionRepository = promotionRepository;
            _sessionRepository = sessionRepository;
            _overlapCheck = overlapCheck;
            _now = now;
        }

        public async Task CreateSessionAsync(CreateSessionCommand command)
        {
            try
            {
                if (command.ClientId == Guid.Empty)
                    throw new ArgumentException("Client ID cannot be empty.");
                if (command.StaffId == Guid.Empty)
                    throw new ArgumentException("Staff ID cannot be empty.");
                if (command.SessionRoom == Guid.Empty)
                    throw new ArgumentException("Session room ID cannot be empty.");
                
                // Load
                var client = await _clientRepository.GetClientAsync(command.ClientId);
                var staff = await _staffRepository.GetStaffAsync(command.StaffId);
                var room = await _roomRepository.GetRoomAsync(command.SessionRoom);
                var promotion = command.PromotionID != Guid.Empty ? await _promotionRepository.GetPromotionAsync(command.PromotionID) : null;

                // Do
                var session = new Session(
                    client,
                    staff,
                    command.SessionType,
                    room,
                    command.StartTime,
                    command.EndTime,
                    command.SessionTotalPrice,
                    command.SessionStatus,
                    promotion,
                    _overlapCheck,
                    _now
                );
                // Save
                await _sessionRepository.CreateSessionAsync(session);
            }
            catch (Exception ex)
            {
                throw new Exception($"An error occurred while creating the session: {ex.Message}", ex);
            }
        }

        public async Task UpdateSessionAsync(UpdateSessionCommand command)
        {
            try
            {
                var session = await _sessionRepository.GetSessionAsync(command.SessionId);
                var client = await _clientRepository.GetClientAsync(command.ClientId);
                var staff = await _staffRepository.GetStaffAsync(command.StaffId);
                var room = await _roomRepository.GetRoomAsync(command.SessionRoom);
                var promotion = command.PromotionID != Guid.Empty ? await _promotionRepository.GetPromotionAsync(command.PromotionID) : null;
                if (command.ClientId != session.SessionClientID)
                    throw new OwnershipException("Session does not belong to the specified customer.");

                // Do
                var updatedSession = new Session(
                    client,
                    staff,
                    command.SessionType,
                    room,
                    command.StartTime,
                    command.EndTime,
                    command.SessionTotalPrice,
                    command.SessionStatus,
                    promotion,
                    _overlapCheck,
                    _now
                );
                // Save
                await _sessionRepository.UpdateSessionAsync(updatedSession);
            }
            catch (Exception ex)
            {
                throw new Exception($"An error occurred while updating the session: {ex.Message}", ex);
            }
            // Load
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
