using FluentResults;
using FysioEnterprise.Domain.Entities;
using FysioEnterprise.Domain.Enums;
using FysioEnterprise.Domain.Exceptions;
using FysioEnterprise.Domain.Service;
using FysioEnterprise.Domain.Service.PricingService;
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
        private readonly IPricingStrategyFactory _pricingStrategyFactory;
        private readonly ITimeNow _now;
        private static readonly SemaphoreSlim _sessionLock = new(1, 1);

        public SessionCommandHandler(
            IClientRepository clientRepository,
            IStaffRepository staffRepository,
            IClinicRepository clinicRepository,
            IPromotionRepository promotionRepository,
            ISessionRepository sessionRepository,
            ISessionTypeRepository sessionTypeRepository,
            IPricingStrategyFactory pricingStrategyFactory,
            ITimeNow now)
        {
            _clientRepository = clientRepository;
            _staffRepository = staffRepository;
            _sessionRepository = sessionRepository;
            _sessionTypeRepository = sessionTypeRepository;
            _clinicRepository = clinicRepository;
            _promotionRepository = promotionRepository;
            _sessionRepository = sessionRepository;
            _pricingStrategyFactory = pricingStrategyFactory;
            _now = now;
        }

        public async Task<Result> CreateSessionAsync(CreateSessionRequest request)
        {
            var clientResult = await _clientRepository.GetClientAsync(request.ClientID);
            if (clientResult.IsFailed)
                return Result.Fail("Klienten blev ikke fundet");

            var staffResult = await _staffRepository.GetStaffAsync(request.StaffID);
            if (staffResult.IsFailed)
                return Result.Fail("Medarbejder blev ikke fundet");

            var clinicResult = await _clinicRepository.GetClinicAsync(request.ClinicID);
            if (clinicResult.IsFailed) return Result.Fail("Klinik blev ikke fundet");

            var roomResult = clinicResult.Value.GetRoom(request.SessionRoomID);
            if (roomResult.IsFailed) return Result.Fail("Rum blev ikke fundet");

            var sessionTypeResult = await _sessionTypeRepository.GetSessionTypeAsync(request.SessionInstanceTypeID);
            if (sessionTypeResult.IsFailed)
                return Result.Fail("Denne bookingtype blev ikke fundet");

            Result<Promotion>? promotionResult = null;

            if (request.PromotionID != Guid.Empty)
            {
                promotionResult = await _promotionRepository.GetPromotionAsync(request.PromotionID);
                if (promotionResult.IsFailed)
                    return Result.Fail("Ingen kampagne er blevet fundet");
            }

            await _sessionLock.WaitAsync();
            try
            {
            var existingClientSessions = await _sessionRepository.GetSessionsByClientAsync(request.ClientID);
            var existingStaffSessions = await _sessionRepository.GetSessionsByStaffAsync(request.StaffID);
            var existingRoomSessions = await _sessionRepository.GetSessionsByRoomAsync(request.ClinicID, request.SessionRoomID);
            var timeSlot = new TimeSlot(request.StartTime, request.EndTime);
            var twelveMonthsAgo = request.StartTime.AddMonths(-12);
            double totalSpend = existingClientSessions
            .Where(s => s.SessionStatus == SessionStatusEnum.Completed &&
                        s.SessionTimeSlot.From >= twelveMonthsAgo)
            .Sum(s => s.priceTotal.Value);
            clientResult.Value.EvaluateLoyaltyStatus(totalSpend);

                bool birthdayEligible = clientResult.Value.IsBirthdayMonth(
                DateOnly.FromDateTime(request.StartTime))
                && !clientResult.Value.HasUsedBirthdayDiscountThisYear;

                try
                {
                    var session = Session.Create(
                        clientResult.Value,
                        staffResult.Value,
                        sessionTypeResult.Value,
                        roomResult.Value.Id,
                        timeSlot,
                        promotionResult?.Value,
                        existingClientSessions,
                        existingStaffSessions,
                        existingRoomSessions,
                        _pricingStrategyFactory,
                        clinicResult.Value.ClinicOpeningHours);
                    await _sessionRepository.CreateSessionAsync(session);

                    //Updates client Loyaltylevel if the value has exceeded a limit
                    await _clientRepository.UpdateClientAsync(clientResult.Value);
                }
                catch (DomainException ex)
                {
                    return ex switch
                    {
                        UserInvalidInputException => Result.Fail("Et input var ikke korrekt" + ex.Message),
                        ValidationException => Result.Fail("Der er sket en valideringsfejl" + ex.Message),
                        _ => Result.Fail("Der er sket en uforventet fejl " + ex.Message) // Fallback catch-all for base DomainException
                    };
                }
                catch (Exception ex) // Catch-all for any other unexpected exceptions (typically SQL or Infrastructure exceptions)
                {
                    System.Diagnostics.Debug.WriteLine($"Infrastructure Failure: {ex.Message}");
                    return Result.Fail("An unexpected system error occurred." + ex.Message);
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
                return Result.Fail("Bookingen blev ikke fundet");

            var clinicResult = await _clinicRepository.GetClinicAsync(request.ClinicID);
            if (clinicResult.IsFailed) return Result.Fail("Klinik blev ikke fundet");

            var session = sessionResult.Value;

            if (request.ClientID != session.SessionClientID)
                return Result.Fail("Denne booking høre ikke til denne klient");

            await _sessionLock.WaitAsync();
            try
            {
            var existingClientSessions = await _sessionRepository.GetSessionsByClientAsync(request.ClientID);
            var existingStaffSessions = await _sessionRepository.GetSessionsByStaffAsync(request.StaffID);
            var existingRoomSessions = await _sessionRepository.GetSessionsByRoomAsync(request.ClinicID, request.SessionRoomID);
            var timeSlot = new TimeSlot(request.StartTime, request.EndTime);

                session.UpdateSessionTime(
                  request.SessionID,
                  timeSlot,
                  existingClientSessions.Where(s => s.Id != session.Id),
                  existingStaffSessions.Where(s => s.Id != session.Id),
                  existingRoomSessions.Where(s => s.Id != session.Id),
                  clinicResult.Value.ClinicOpeningHours
                  );

                await _sessionRepository.UpdateSessionAsync(session);
                return Result.Ok();
            }

            catch (DomainException ex)
            {
                return ex switch
                {
                    UserInvalidInputException => Result.Fail("Et input var ikke korrekt" + ex.Message),
                    ValidationException => Result.Fail("Der er sket en valideringsfejl" + ex.Message),
                    _ => Result.Fail("Der er sket en uforventet fejl " + ex.Message) // Fallback catch-all for base DomainException
                };
            }
            catch (Exception ex) // Catch-all for any other unexpected exceptions (typically SQL or Infrastructure exceptions)
            {
                System.Diagnostics.Debug.WriteLine($"Infrastructure Failure: {ex.Message}");
                return Result.Fail("An unexpected system error occurred." + ex.Message);
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
                return Result.Fail("Booking blev ikke fundet");

            try
            {
                sessionResult.Value.CompletedSession();
            }
            catch (UserInvalidInputException ex)
            {
                return Result.Fail($"Kunne ikke sætte booking til afsluttet: {ex.Message}");
            }

            await _sessionRepository.UpdateSessionAsync(sessionResult.Value);
            return Result.Ok();
        }

        public async Task<Result> CancelSessionAsync(CancelSessionRequest request)
        {
            var sessionResult = await _sessionRepository.GetSessionAsync(request.SessionId);
            if (sessionResult.IsFailed)
                return Result.Fail("booking blev ikke fundet.");

            try
            {
                sessionResult.Value.CancelSession();
            }
            catch (UserInvalidInputException ex)
            {
                return Result.Fail($"Kunne ikke sætte booking til aflyst: {ex.Message}");
            }

            await _sessionRepository.UpdateSessionAsync(sessionResult.Value);
            return Result.Ok();
        }

        public async Task<Result> MarkSessionAsNoShowAsync(MarkNoShowSessionRequest request)
        {
            var sessionResult = await _sessionRepository.GetSessionAsync(request.SessionID);
            if (sessionResult.IsFailed)
                return Result.Fail("booking blev ikke fundet");
                
            sessionResult.Value.SetNoShowSession();

            await _sessionRepository.UpdateSessionAsync(sessionResult.Value);
            return Result.Ok();
        }
    }
}
