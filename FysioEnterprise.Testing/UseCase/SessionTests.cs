using FluentResults;
using FysioEnterprise.Domain.Entities;
using FysioEnterprise.Domain.Exceptions;
using FysioEnterprise.Domain.Service;
using FysioEnterprise.Domain.Service.PricingService;
using FysioEnterprise.Domain.Service.PricingService.Strategies.PricingMethods;
using FysioEnterprise.Facade.DTOs;
using FysioEnterprise.Facade.UseCase.SessionUseCase;
using FysioEnterprise.UseCase;
using FysioEnterprise.UseCase.CommandHandlers.SessionCommands;
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

        private SessionCommandHandler CreateSessionTest() => new(
            _mockclientRepository.Object,
            _mockstaffRepository.Object,
            _mockroomRepository.Object,
            _mockpromotionRepository.Object,
            _mocksessionRepository.Object,
            _mocksessionTypeRepository.Object,
            _mocknow.Object,
            _mockstrategyFactory.Object,
            _mockcalculator.Object);

        [Fact]
        public async Task<Result> CreateSessionAsync()
        {
            var clientId = Guid.NewGuid();
            var staffId = Guid.NewGuid();
            var roomId = Guid.NewGuid();
            var promotionId = Guid.NewGuid();
            var sessionId = Guid.NewGuid();
            var sessionTypeId = Guid.NewGuid();
            var timeNow = DateTime.Now;
            var pricingStrategy = new BirthdayPricingStrategy();

            _mockclientRepository.Setup(r => r.GetClientAsync(clientId))
                .ReturnsAsync(new Client("Hans", "1312"));
            _mockstaffRepository.Setup(r => r.GetStaffAsync(staffId))
                .ReturnsAsync(new Staff("John", "Fysio"));
            _mockroomRepository.Setup(r => r.GetRoomAsync(roomId))
                .ReturnsAsync(new Room("Room 1"));
            _mocksessionTypeRepository.Setup(r => r.GetSessionTypeAsync(sessionTypeId))
                .ReturnsAsync(new SessionType("Massage", 100));
            _mocksessionRepository.Setup(r => r.GetSessionsByClientAsync(clientId))
                .ReturnsAsync(new List<Session>());
            _mocksessionRepository.Setup(r => r.GetSessionsByStaffAsync(staffId))
                .ReturnsAsync(new List<Session>());
/*
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
            return Result.Ok();*/
        }
    }
}