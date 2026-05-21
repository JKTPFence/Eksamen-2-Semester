using FluentResults;
using FysioEnterprise.Domain.Entities;
using FysioEnterprise.Domain.Exceptions;
using FysioEnterprise.Domain.Service;
using FysioEnterprise.Domain.Service.PricingService;
using FysioEnterprise.Domain.Service.PricingService.Strategies.PricingMethods;
using FysioEnterprise.Domain.ValueObjects;
using FysioEnterprise.Facade.UseCase.SessionUseCase;
using FysioEnterprise.UseCase.IRepositories;
using static FysioEnterprise.Facade.RequestModels.SessionRequests;

namespace FysioEnterprise.UseCase.CommandHandlers.SessionCommands
{
    public class SessionCommandHandler : ICreateSessionUseCase, IUpdateSessionUseCase, ICancelSessionUseCase, IEndSessionUseCase, IMarkSessionAsNoShowUseCase
    {
        private readonly IClientRepository _clientRepository;
        private readonly IStaffRepository _staffRepository;
        private readonly IClinicRepository _clinicRepository;
        private readonly IPromotionRepository _promotionRepository;
        private readonly ISessionRepository _sessionRepository;
        private readonly ISessionTypeRepository _sessionTypeRepository;
        private readonly ITimeNow _now;
        private readonly IPricingStrategyFactory _strategyFactory;
        private readonly PriceCalculator _calculator;
        private static readonly SemaphoreSlim _sessionLock = new(1, 1);

        public SessionCommandHandler(
            IClientRepository clientRepository,
            IStaffRepository staffRepository,
            IClinicRepository clinicRepository,
            IPromotionRepository promotionRepository,
            ISessionRepository sessionRepository,
            ISessionTypeRepository sessionTypeRepository,
            ITimeNow now,
            IPricingStrategyFactory strategyFactory,
            PriceCalculator calculator)
        {
            _clientRepository = clientRepository;
            _staffRepository = staffRepository;
            _sessionRepository = sessionRepository;
            _sessionTypeRepository = sessionTypeRepository;
            _clinicRepository = clinicRepository;
            _promotionRepository = promotionRepository;
            _sessionRepository = sessionRepository;
            _now = now;
            _strategyFactory = strategyFactory;
            _calculator = calculator;
        }

        public async Task<Result> CreateSessionAsync(CreateSessionRequest request)
        {
            var clientResult = await _clientRepository.GetClientAsync(request.ClientID);
            if (clientResult.IsFailed)
                return Result.Fail("Client not found.");

            var staffResult = await _staffRepository.GetStaffAsync(request.StaffID);
            if (staffResult.IsFailed)
                return Result.Fail("Staff not found.");

            var clinicResult = await _clinicRepository.GetClinicAsync(request.ClinicID);
            if (clinicResult.IsFailed) return Result.Fail("Clinic not found.");

            var roomResult = clinicResult.Value.GetRoom(request.SessionRoomID);
            if (roomResult.IsFailed) return Result.Fail("Room not found.");

            var sessionTypeResult = await _sessionTypeRepository.GetSessionTypeAsync(request.SessionInstanceTypeID);
            if (sessionTypeResult.IsFailed)
                return Result.Fail("Session type not found.");
            
            var promotionResult = await _promotionRepository.GetPromotionAsync(request.PromotionID);
                if (promotionResult.IsFailed)
                    return Result.Fail("Promotion not found.");
    

            var existingClientSessions = await _sessionRepository.GetSessionsByClientAsync(request.ClientID);
            var existingStaffSessions = await _sessionRepository.GetSessionsByStaffAsync(request.StaffID);
            var existingRoomSessions = await _sessionRepository.GetSessionsByRoomAsync(request.ClinicID, request.SessionRoomID);
            var timeSlot = new TimeSlot(request.StartTime, request.EndTime);

            await _sessionLock.WaitAsync();
            try
            {

                bool birthdayEligible = clientResult.Value.IsBirthdayMonth(
                DateOnly.FromDateTime(request.StartTime))
                && !clientResult.Value.HasUsedBirthdayDiscountThisYear;

                var strategies = _strategyFactory.BuildStrategies(
                    clientResult.Value.ClientLoyaltyLevel,
                    birthdayEligible,
                    promotionResult?.Value
                );

                var totalPrice = await _calculator.Calculate(
                    sessionTypeResult.Value.SessionTypePrice,
                    strategies
                );

                if (birthdayEligible)
                {
                    var birthdayPrice = new BirthdayPricingStrategy().Apply(
                        sessionTypeResult.Value.SessionTypePrice);

                    if (birthdayPrice == totalPrice)
                    {
                        clientResult.Value.MarkBirthdayDiscountUsed(
                            DateOnly.FromDateTime(request.StartTime));
                        await _clientRepository.UpdateClientAsync(clientResult.Value);
                    }
                }
                try
                {
                    var session = Session.Create(
                        clientResult.Value.Id,
                        staffResult.Value.Id,
                        sessionTypeResult.Value.Id,
                        roomResult.Value.Id,
                        timeSlot,
                        totalPrice,
                        promotionResult?.Value?.Id,
                        existingClientSessions,
                        existingStaffSessions,
                        existingRoomSessions);
                    await _sessionRepository.CreateSessionAsync(session);
                }
                catch (DomainException ex)
                {
                    return Result.Fail(ex.Message);
                }
                return Result.Ok();
            }
            finally
            {
                _sessionLock.Release();
            }
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
            var existingRoomSessions = await _sessionRepository.GetSessionsByRoomAsync(request.ClinicID, request.SessionRoomID);
            var timeSlot = new TimeSlot(request.StartTime, request.EndTime);

            await _sessionLock.WaitAsync();
            try
            {
                  session.UpdateSessionTime(
                    request.SessionID,
                    timeSlot,
                    existingClientSessions.Where(s => s.Id != session.Id),
                    existingStaffSessions.Where(s => s.Id != session.Id),
                    existingRoomSessions.Where(s => s.Id != session.Id)
                    );

                await _sessionRepository.UpdateSessionAsync(session);
                return Result.Ok();
            }

            catch (DomainException ex)
            {
                return Result.Fail(ex.Message);
            }

         finally
          {
            _sessionLock.Release();
          }

        }

        public async Task<Result> EndSessionAsync(EndSessionRequest request)
        {
            var sessionResult = await _sessionRepository.GetSessionAsync(request.SessionId);
            if (sessionResult.IsFailed)
                return Result.Fail("Session not found.");

            try
            {
                sessionResult.Value.CompletedSession();
            }
            catch (UserInvalidInputException ex)
            {
                return Result.Fail($"Error couldn't complete session: {ex.Message}");
            }

            await _sessionRepository.UpdateSessionAsync(sessionResult.Value);
            return Result.Ok();
        }

        public async Task<Result> CancelSessionAsync(CancelSessionRequest request)
        {
            var sessionResult = await _sessionRepository.GetSessionAsync(request.SessionId);
            if (sessionResult.IsFailed)
                return Result.Fail("Session not found.");

            try
            {
                sessionResult.Value.CancelSession();
            }
            catch (UserInvalidInputException ex)
            {
                return Result.Fail($"Error couldn't set session to cancelled: {ex.Message}");
            }

            await _sessionRepository.UpdateSessionAsync(sessionResult.Value);
            return Result.Ok();
        }

        public async Task<Result> MarkSessionAsNoShowAsync(MarkNoShowSessionRequest request)
        {
            var sessionResult = await _sessionRepository.GetSessionAsync(request.SessionID);
            if (sessionResult.IsFailed)
                return Result.Fail("Session not found.");

            try
            {
                sessionResult.Value.SetNoShowSession();
            }
            catch (UserInvalidInputException ex)
            {
                return Result.Fail($"Error couldn't set session as NoShow: {ex.Message}");
            }

            await _sessionRepository.UpdateSessionAsync(sessionResult.Value);
            return Result.Ok();
        }
    }
}
