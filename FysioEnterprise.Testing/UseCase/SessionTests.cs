using FluentResults;
using FysioEnterprise.Domain.Entities;
using FysioEnterprise.Domain.Exceptions;
using FysioEnterprise.Domain.Service;
using FysioEnterprise.Domain.Service.PricingService;
using FysioEnterprise.Domain.Service.PricingService.Strategies.PricingMethods;
using FysioEnterprise.Facade.DTOs;
using FysioEnterprise.Facade.UseCase.SessionUseCase;
using FysioEnterprise.UseCase;
using FysioEnterprise.UseCase.IRepositories;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;
using static FysioEnterprise.Facade.RequestModels.SessionRequests;

namespace FysioEnterprise.Testing.UseCase
{
    public class SessionCommandHandlerTests
    {
        private readonly Mock<IClientRepository> _mockclientRepository = new();
        private readonly Mock<IStaffRepository> _mockstaffRepository = new();
        private readonly Mock<IRoomRepository> _mockroomRepository = new();
        private readonly Mock<IPromotionRepository> _mockpromotionRepository = new();
        private readonly Mock<ISessionRepository> _mocksessionRepository = new();
        private readonly Mock<ISessionTypeRepository> _mocksessionTypeRepository = new();
        private readonly Mock<ITimeNow> _mocknow = new();
        private readonly Mock<IPricingStrategyFactory> _mockstrategyFactory = new();
        private readonly Mock<PriceCalculator> _mockcalculator = new();
        private static readonly SemaphoreSlim _birthdayLock = new(1, 1);

        private SessionCommandHandlerTests CreateSession() => new(
            _mockclientRepository.Object,
            _mockstaffRepository.Object,
            _mockroomRepository.Object,
            _mockpromotionRepository.Object,
            _mocksessionRepository.Object,
            _mocknow.Object,
            _mockstrategyFactory.Object,
            _mockcalculator.Object)
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

            var sessionTypeResult = await _sessionTypeRepository.GetSessionTypeAsync(request.SessionInstanceTypeID);
            if (sessionTypeResult.IsFailed)
                return Result.Fail("Session type not found.");

            Result<Promotion> promotionResult = null;
            if (request.PromotionID != Guid.Empty)
            {
                promotionResult = await _promotionRepository.GetPromotionAsync(request.PromotionID);
                if (promotionResult.IsFailed)
                    return Result.Fail("Promotion not found.");
            }

            var existingClientSessions = await _sessionRepository.GetSessionsByClientAsync(request.ClientID);
            var existingStaffSessions = await _sessionRepository.GetSessionsByStaffAsync(request.StaffID);

            await _birthdayLock.WaitAsync();
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

                var totalPrice = _calculator.Calculate(
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
                        clientResult.Value.ClientID,
                        staffResult.Value.StaffID,
                        sessionTypeResult.Value.SessionTypeId,
                        roomResult.Value.RoomID,
                        request.StartTime,
                        request.EndTime,
                        totalPrice,
                        promotionResult?.Value?.PromotionID,
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
            finally
            {
                _birthdayLock.Release();
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