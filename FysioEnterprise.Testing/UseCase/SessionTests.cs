using System.Reflection;
using FluentResults;
using FysioEnterprise.Domain.Entities;
using FysioEnterprise.Domain.Service;
using FysioEnterprise.Domain.Service.PricingService;
using FysioEnterprise.Domain.ValueObjects;
using FysioEnterprise.UseCase.CommandHandlers.SessionCommands;
using FysioEnterprise.UseCase.IRepositories;
using Moq;
using Xunit;
using static FysioEnterprise.Facade.RequestModels.SessionRequests;


namespace FysioEnterprise.Testing.UseCase
{
    public class SessionCommandHandlerTests
    {
        private readonly Mock<IClientRepository> _mockClientRepository;
        private readonly Mock<IStaffRepository> _mockStaffRepository;
        private readonly Mock<IClinicRepository> _mockClinicRepository;
        private readonly Mock<IPromotionRepository> _mockPromotionRepository;
        private readonly Mock<ISessionRepository> _mockSessionRepository;
        private readonly Mock<ISessionTypeRepository> _mockSessionTypeRepository;
        private readonly Mock<ITimeNow> _mockTimeNow;
        private readonly Mock<IPricingStrategyFactory> _mockStrategyFactory;
        private readonly Mock<PriceCalculator> _mockCalculator;
        private readonly SessionCommandHandler _handler;

        public SessionCommandHandlerTests()
        {
            _mockClientRepository = new Mock<IClientRepository>();
            _mockStaffRepository = new Mock<IStaffRepository>();
            _mockClinicRepository = new Mock<IClinicRepository>();
            _mockPromotionRepository = new Mock<IPromotionRepository>();
            _mockSessionRepository = new Mock<ISessionRepository>();
            _mockSessionTypeRepository = new Mock<ISessionTypeRepository>();
            _mockTimeNow = new Mock<ITimeNow>();
            _mockStrategyFactory = new Mock<IPricingStrategyFactory>();
            _mockCalculator = new Mock<PriceCalculator>();

            _handler = new SessionCommandHandler(
                _mockClientRepository.Object,
                _mockStaffRepository.Object,
                _mockClinicRepository.Object, 
                _mockPromotionRepository.Object,
                _mockSessionRepository.Object,
                _mockSessionTypeRepository.Object,
                _mockStrategyFactory.Object,
                _mockTimeNow.Object);
        }

        private Client CreateMockClient(Guid clientId)
        {
            var client = Client.Create(
                "John",
                "Doe",
                "john@example.com",
                "1234567890",
                DateOnly.FromDateTime(DateTime.Now.AddYears(-30)),
                "123 Main St",
                null,
                Guid.NewGuid(),
                LoyaltyLevel.None);
            client.GetType().GetProperty("Id")?.SetValue(client, clientId);
            return client;
        }

        private Client CreateMockClientWithBirthday(Guid clientId, DateTime birthDate)
        {
            var client = Client.Create(
                "John",
                "Doe",
                "john@example.com",
                "1234567890",
                DateOnly.FromDateTime(birthDate),
                "123 Main St",
                null,
                Guid.NewGuid(),
                LoyaltyLevel.None);
            client.GetType().GetProperty("Id")?.SetValue(client, clientId);
            return client;
        }

        private Staff CreateMockStaff(Guid staffId)
        {
            var staff = new Staff("Jane", "Smith", "0987654321", "Physiotherapist", 12345, new List<Clinic>());
            staff.GetType().GetProperty("Id")?.SetValue(staff, staffId);
            return staff;
        }

        private Clinic CreateMockClinic(Guid clinicId, Guid? roomId = null)
        {

            var roomsList = new List<Room>();
            var clinic = new Clinic("123 Clinic St", new List<OpeningHours>(), roomsList);

            typeof(Clinic)
                .GetProperty("Id", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.FlattenHierarchy)
                ?.SetValue(clinic, clinicId);

            if (roomId.HasValue)
            {
                var room = new Room(clinic, 2);
                typeof(Room)
                    .GetProperty("Id", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.FlattenHierarchy)
                    ?.SetValue(room, roomId.Value);
                roomsList.Add(room);
            }

            typeof(Clinic)
                .GetField("_clinicRooms", BindingFlags.NonPublic | BindingFlags.Instance)
                ?.SetValue(clinic, roomsList);

            return clinic;
        }

        private Clinic CreateMockClinicWithFailingRoom(Guid clinicId)
        {
            var clinic = new Clinic("123 Clinic St", new List<OpeningHours>(), new List<Room>());
            clinic.GetType().GetProperty("Id")?.SetValue(clinic, clinicId);
            return clinic;
        }

        private SessionType CreateMockSessionType(Guid sessionTypeId)
        {
            var sessionType = new SessionType("Massage", new Price (100), 60, new TimeOnly(1, 0), new List<int>());
            sessionType.GetType().GetProperty("Id")?.SetValue(sessionType, sessionTypeId);
            return sessionType;
        }

        private void SetupRepositoryMocks(Client client, Staff staff, Clinic clinic, SessionType sessionType, Promotion? promotion)
        {
            _mockClientRepository.Setup(r => r.GetClientAsync(client.Id))
                .ReturnsAsync(Result.Ok(client));

            _mockStaffRepository.Setup(r => r.GetStaffAsync(staff.Id))
                .ReturnsAsync(Result.Ok(staff));

            _mockClinicRepository.Setup(r => r.GetClinicAsync(clinic.Id))
                .ReturnsAsync(Result.Ok(clinic));

            _mockSessionTypeRepository.Setup(r => r.GetSessionTypeAsync(sessionType.Id))
                .ReturnsAsync(Result.Ok(sessionType));

                _mockStrategyFactory
                .Setup(f => f.BuildStrategies(It.IsAny<Client>(), It.IsAny<Promotion>(), It.IsAny<SessionType>()))
                .ReturnsAsync(new Price(100D));

                _mockClientRepository
                .Setup(r => r.UpdateClientAsync(It.IsAny<Client>()))
                .ReturnsAsync(Result.Ok());

                _mockSessionRepository
                .Setup(r => r.CreateSessionAsync(It.IsAny<Session>()))
                .Returns(Task.CompletedTask);

            _mockSessionRepository.Setup(r => r.GetSessionsByClientAsync(It.IsAny<Guid>()))
                .ReturnsAsync(new List<Session>());
            _mockSessionRepository.Setup(r => r.GetSessionsByStaffAsync(It.IsAny<Guid>()))
                .ReturnsAsync(new List<Session>());
            _mockSessionRepository.Setup(r => r.GetSessionsByRoomAsync(It.IsAny<Guid>(), It.IsAny<Guid>()))
                .ReturnsAsync(new List<Session>());

            if (promotion != null)
            {
                _mockPromotionRepository.Setup(r => r.GetPromotionAsync(promotion.Id))
                    .ReturnsAsync(promotion);
            }
        }

        [Fact]
        public async Task CreateSessionAsync_WithValidData()
        {
            //Arrange
            var clientId = Guid.NewGuid();
            var staffId = Guid.NewGuid();
            var clinicId = Guid.NewGuid();
            var roomId = Guid.NewGuid();
            var promotionId = Guid.NewGuid();
            var sessionTypeId = Guid.NewGuid();
            var timeNow = DateTime.UtcNow.AddDays(90);
            var endTime = timeNow.AddHours(1);

            var client = CreateMockClient(clientId);
            var staff = CreateMockStaff(staffId);
            var clinic = CreateMockClinic(clinicId, roomId);
            var sessionType = CreateMockSessionType(sessionTypeId);

            SetupRepositoryMocks(client, staff, clinic, sessionType, null);

            var request = new CreateSessionRequest(
                ClientID: clientId,
                StaffID: staffId,
                PromotionID: Guid.Empty,
                ClinicID: clinicId,
                SessionRoomID: roomId,
                SessionInstanceTypeID: sessionTypeId,
                SessionTotalPrice: 100,
                StartTime: timeNow,
                EndTime: endTime);

            //Act
            var result = await _handler.CreateSessionAsync(request);

            // Assert
            Assert.True(result.IsSuccess);
            _mockSessionRepository.Verify(r => r.CreateSessionAsync(It.IsAny<Session>()), Times.Once);
        }

        [Fact]
        public async Task CreateSessionAsync_WithInvalidClient_ShouldReturnFailure()
        {
            var request = new CreateSessionRequest(
                ClientID: Guid.NewGuid(),
                StaffID: Guid.NewGuid(),
                PromotionID: Guid.Empty,
                ClinicID: Guid.NewGuid(),
                SessionRoomID: Guid.NewGuid(),
                SessionInstanceTypeID: Guid.NewGuid(),
                SessionTotalPrice: 100,
                StartTime: DateTime.Now.AddDays(1),
                EndTime: DateTime.Now.AddDays(1).AddHours(1));

            _mockClientRepository.Setup(r => r.GetClientAsync(request.ClientID))
                .ReturnsAsync(Result.Fail("Klienten blev ikke fundet"));

            var result = await _handler.CreateSessionAsync(request);

            Assert.False(result.IsSuccess);
            Assert.Contains("Klienten blev ikke fundet", result.Errors[0].Message);
        }

        [Fact]
        public async Task CreateSessionAsync_WithInvalidStaff_ShouldReturnFailure()
        {
            var clientId = Guid.NewGuid();
            var staffId = Guid.NewGuid();
            var client = CreateMockClient(clientId);

            _mockClientRepository.Setup(r => r.GetClientAsync(clientId))
                .ReturnsAsync(Result.Ok(client));

            _mockStaffRepository.Setup(r => r.GetStaffAsync(staffId))
                .ReturnsAsync(Result.Fail("Medarbejder blev ikke fundet"));

            var request = new CreateSessionRequest(
                ClientID: clientId,
                StaffID: staffId,
                PromotionID: Guid.Empty,
                ClinicID: Guid.NewGuid(),
                SessionRoomID: Guid.NewGuid(),
                SessionInstanceTypeID: Guid.NewGuid(),
                SessionTotalPrice: 100,
                StartTime: DateTime.Now.AddDays(1),
                EndTime: DateTime.Now.AddDays(1).AddHours(1));

            var result = await _handler.CreateSessionAsync(request);

            Assert.False(result.IsSuccess);
            Assert.Contains("Medarbejder blev ikke fundet", result.Errors[0].Message);
        }

        [Fact]
        public async Task CreateSessionAsync_WithInvalidClinic_ShouldReturnFailure()
        {
            var clientId = Guid.NewGuid();
            var staffId = Guid.NewGuid();
            var clinicId = Guid.NewGuid();

            var client = CreateMockClient(clientId);
            var staff = CreateMockStaff(staffId);

            _mockClientRepository.Setup(r => r.GetClientAsync(clientId))
                .ReturnsAsync(Result.Ok(client));

            _mockStaffRepository.Setup(r => r.GetStaffAsync(staffId))
                .ReturnsAsync(Result.Ok(staff));

            _mockClinicRepository.Setup(r => r.GetClinicAsync(clinicId))
                .ReturnsAsync(Result.Fail("Klinik blev ikke fundet"));

            var request = new CreateSessionRequest(
                ClientID: clientId,
                StaffID: staffId,
                PromotionID: Guid.Empty,
                ClinicID: clinicId,
                SessionRoomID: Guid.NewGuid(),
                SessionInstanceTypeID: Guid.NewGuid(),
                SessionTotalPrice: 100,
                StartTime: DateTime.Now.AddDays(1),
                EndTime: DateTime.Now.AddDays(1).AddHours(1));

            var result = await _handler.CreateSessionAsync(request);

            Assert.False(result.IsSuccess);
            Assert.Contains("Klinik blev ikke fundet", result.Errors[0].Message);
        }

        [Fact]
        public async Task CreateSessionAsync_WithInvalidRoom_ShouldReturnFailure()
        {
            var clientId = Guid.NewGuid();
            var staffId = Guid.NewGuid();
            var clinicId = Guid.NewGuid();
            var roomId = Guid.NewGuid();

            var client = CreateMockClient(clientId);
            var staff = CreateMockStaff(staffId);
            var clinic = CreateMockClinicWithFailingRoom(clinicId);

            _mockClientRepository.Setup(r => r.GetClientAsync(clientId))
                .ReturnsAsync(Result.Ok(client));

            _mockStaffRepository.Setup(r => r.GetStaffAsync(staffId))
                .ReturnsAsync(Result.Ok(staff));

            _mockClinicRepository.Setup(r => r.GetClinicAsync(clinicId))
                .ReturnsAsync(Result.Ok(clinic));

            var request = new CreateSessionRequest(
                ClientID: clientId,
                StaffID: staffId,
                PromotionID: Guid.Empty,
                ClinicID: clinicId,
                SessionRoomID: roomId,
                SessionInstanceTypeID: Guid.NewGuid(),
                SessionTotalPrice: 100,
                StartTime: DateTime.Now.AddDays(1),
                EndTime: DateTime.Now.AddDays(1).AddHours(1));

            var result = await _handler.CreateSessionAsync(request);

            Assert.False(result.IsSuccess);
            Assert.Contains("Rum blev ikke fundet", result.Errors[0].Message);
        }

        [Fact]
        public async Task CreateSessionAsync_WithInvalidSessionType_ShouldReturnFailure()
        {
            var clientId = Guid.NewGuid();
            var staffId = Guid.NewGuid();
            var clinicId = Guid.NewGuid();
            var roomId = Guid.NewGuid();
            var sessionTypeId = Guid.NewGuid();

            var client = CreateMockClient(clientId);
            var staff = CreateMockStaff(staffId);
            var clinic = CreateMockClinic(clinicId, roomId);

            _mockClientRepository.Setup(r => r.GetClientAsync(clientId))
                .ReturnsAsync(Result.Ok(client));

            _mockStaffRepository.Setup(r => r.GetStaffAsync(staffId))
                .ReturnsAsync(Result.Ok(staff));

            _mockClinicRepository.Setup(r => r.GetClinicAsync(clinicId))
                .ReturnsAsync(Result.Ok(clinic));

            _mockSessionTypeRepository.Setup(r => r.GetSessionTypeAsync(sessionTypeId))
                .ReturnsAsync(Result.Fail("Denne bookingtype blev ikke fundet"));

            _mockSessionRepository.Setup(r => r.GetSessionsByRoomAsync(It.IsAny<Guid>(), It.IsAny<Guid>())).ReturnsAsync(new List<Session>());

            var request = new CreateSessionRequest(
                ClientID: clientId,
                StaffID: staffId,
                PromotionID: Guid.Empty,
                ClinicID: clinicId,
                SessionRoomID: roomId,
                SessionInstanceTypeID: sessionTypeId,
                SessionTotalPrice: 100,
                StartTime: DateTime.Now.AddDays(1),
                EndTime: DateTime.Now.AddDays(1).AddHours(1));

            var result = await _handler.CreateSessionAsync(request);

            Assert.False(result.IsSuccess);
            Assert.Contains("Denne bookingtype blev ikke fundet", result.Errors[0].Message);
        }

        [Fact]
        public async Task CreateSessionAsync_WithInvalidPromotion_ShouldReturnFailure()
        {
            var clientId = Guid.NewGuid();
            var staffId = Guid.NewGuid();
            var clinicId = Guid.NewGuid();
            var roomId = Guid.NewGuid();
            var sessionTypeId = Guid.NewGuid();
            var promotionId = Guid.NewGuid();

            var client = CreateMockClient(clientId);
            var staff = CreateMockStaff(staffId);
            var clinic = CreateMockClinic(clinicId, roomId);
            var sessionType = CreateMockSessionType(sessionTypeId);

            _mockClientRepository.Setup(r => r.GetClientAsync(It.IsAny<Guid>()))
                .ReturnsAsync(Result.Ok(client));

            _mockStaffRepository.Setup(r => r.GetStaffAsync(It.IsAny<Guid>()))
                .ReturnsAsync(Result.Ok(staff));

            _mockClinicRepository.Setup(r => r.GetClinicAsync(It.IsAny<Guid>()))
                .ReturnsAsync(Result.Ok(clinic));

            _mockSessionTypeRepository.Setup(r => r.GetSessionTypeAsync(It.IsAny<Guid>()))
                .ReturnsAsync(Result.Ok(sessionType));

            _mockPromotionRepository.Setup(r => r.GetPromotionAsync(It.IsAny<Guid>()))
                .ReturnsAsync(Result.Fail("Ingen kampagne er blevet fundet"));

            _mockSessionRepository.Setup(r => r.GetSessionsByRoomAsync(It.IsAny<Guid>(), It.IsAny<Guid>())).ReturnsAsync(new List<Session>());
            _mockSessionRepository.Setup(r => r.GetSessionsByClientAsync(It.IsAny<Guid>())).ReturnsAsync(new List<Session>());
            _mockSessionRepository.Setup(r => r.GetSessionsByStaffAsync(It.IsAny<Guid>())).ReturnsAsync(new List<Session>());



            var request = new CreateSessionRequest(
                ClientID: clientId,
                StaffID: staffId,
                PromotionID: promotionId,
                ClinicID: clinicId,
                SessionRoomID: roomId,
                SessionInstanceTypeID: sessionTypeId,
                SessionTotalPrice: 100,
                StartTime: DateTime.Now.AddDays(1),
                EndTime: DateTime.Now.AddDays(1).AddHours(1));

            var result = await _handler.CreateSessionAsync(request);

            Assert.False(result.IsSuccess);
            Assert.Contains("Ingen kampagne er blevet fundet", result.Errors[0].Message);
        }

        [Fact]
        public async Task CreateSessionAsync_WithBirthdayDiscount_ShouldMarkBirthdayDiscountAsUsed()
        {
            var clientId = Guid.NewGuid();
            var staffId = Guid.NewGuid();
            var clinicId = Guid.NewGuid();
            var roomId = Guid.NewGuid();
            var sessionTypeId = Guid.NewGuid();
            var birthDate = new DateTime(1990, 5, 15);
            var sessionStartTime = new DateTime(2027, 5, 20);

            var client = CreateMockClientWithBirthday(clientId, birthDate);
            var staff = CreateMockStaff(staffId);
            var clinic = CreateMockClinic(clinicId, roomId);
            var sessionType = CreateMockSessionType(sessionTypeId);

            _mockClientRepository.Setup(r => r.GetClientAsync(clientId)).ReturnsAsync(Result.Ok(client));
            _mockStaffRepository.Setup(r => r.GetStaffAsync(staffId)).ReturnsAsync(Result.Ok(staff));
            _mockClinicRepository.Setup(r => r.GetClinicAsync(clinicId)).ReturnsAsync(Result.Ok(clinic));
            _mockSessionTypeRepository.Setup(r => r.GetSessionTypeAsync(sessionTypeId)).ReturnsAsync(Result.Ok(sessionType));

            _mockSessionRepository.Setup(r => r.GetSessionsByClientAsync(clientId)).ReturnsAsync(new List<Session>());
            _mockSessionRepository.Setup(r => r.GetSessionsByStaffAsync(staffId)).ReturnsAsync(new List<Session>());

            _mockSessionRepository.Setup(r => r.GetSessionsByRoomAsync(It.IsAny<Guid>(), It.IsAny<Guid>())).ReturnsAsync(new List<Session>());

            var discountedPrice = new Price(75D);

            _mockStrategyFactory.Setup(f => f.BuildStrategies(It.IsAny<Client>(), It.IsAny<Promotion>(), It.IsAny<SessionType>()))
                .ReturnsAsync(discountedPrice);

            var request = new CreateSessionRequest(
                ClientID: clientId,
                StaffID: staffId,
                PromotionID: Guid.Empty,
                ClinicID: clinicId,
                SessionRoomID: roomId,
                SessionInstanceTypeID: sessionTypeId,
                SessionTotalPrice: 75,
                StartTime: sessionStartTime,
                EndTime: sessionStartTime.AddHours(1));

            var result = await _handler.CreateSessionAsync(request);

            Assert.True(result.IsSuccess);
            _mockClientRepository.Verify(r => r.UpdateClientAsync(It.Is<Client>(c => c.Id == clientId)), Times.Once);
        }
    }
}