using Moq;
using Xunit;
using FluentResults;
using FysioEnterprise.Domain.Entities;
using FysioEnterprise.Domain.ValueObjects;
using FysioEnterprise.UseCase.CommandHandler.ClientCommands;
using FysioEnterprise.UseCase.IRepositories;
using static FysioEnterprise.Facade.RequestModels.ClientRequests;

namespace FysioEnterprise.Testing.UseCase
{
    public class ClientCommandHandlerTests
    {
        private readonly Mock<IClientRepository> _mockClientRepository;
        private readonly Mock<IStaffRepository> _mockStaffRepository;
        private readonly ClientCommandHandler _handler;

        public ClientCommandHandlerTests()
        {
            _mockClientRepository = new Mock<IClientRepository>();
            _mockStaffRepository = new Mock<IStaffRepository>();
            _handler = new ClientCommandHandler(
                _mockClientRepository.Object,
                _mockStaffRepository.Object);
        }

        [Fact]
        public async Task CreateClientAsync_WithValidData_ShouldReturnSuccess()
        {
            // Arrange
            var staffId = Guid.NewGuid();
            var clientId = Guid.NewGuid();
            var request = new CreateClientRequest(
                clientId,
                "John",
                "Doe",
                "john@example.com",
                "1234567890",
                new DateOnly(1990, 1, 1),
                "123 Main St",
                "Regular client",
                LoyaltyLevel.Bronze,
                staffId);
            // Act
            var staff = new Staff("Jane", "Smith", "1234567890", "Physiotherapist", 12345, new List<Clinic>());

            _mockStaffRepository.Setup(x => x.GetStaffAsync(staffId))
                .ReturnsAsync(Result.Ok(staff));
            _mockClientRepository.Setup(x => x.CreateClientAsync(It.IsAny<Client>()))
                .ReturnsAsync(Result.Ok());
            // Assert
            var result = await _handler.CreateClientAsync(request);
            Assert.True(result.IsSuccess);
            _mockClientRepository.Verify(x => x.CreateClientAsync(It.IsAny<Client>()), Times.Once);
        }

        [Fact]
        public async Task CreateClientAsync_WithNullRequest_ReturnsFail()
        {
            var result = await _handler.CreateClientAsync(null);

            Assert.True(result.IsFailed);
            Assert.Contains("Request cannot be null", result.Errors.Select(e => e.Message));
            _mockClientRepository.Verify(x => x.CreateClientAsync(It.IsAny<Client>()), Times.Never);
        }

        [Fact]
        public async Task CreateClientAsync_WithInvalidStaff_ReturnsFail()
        {
            var staffId = Guid.NewGuid();
            var clientId = Guid.NewGuid();
            var request = new CreateClientRequest(
                clientId,
                "John",
                "Doe",
                "john@example.com",
                "1234567890",
                new DateOnly(1990, 1, 1),
                "123 Main St",
                "",
                LoyaltyLevel.Bronze,
                staffId);

            _mockStaffRepository.Setup(x => x.GetStaffAsync(It.IsAny<Guid>()))
                .ReturnsAsync(Result.Fail("Staff not found"));

            var result = await _handler.CreateClientAsync(request);

            Assert.True(result.IsFailed);
            Assert.Contains("Preferred staff not found", result.Errors.Select(e => e.Message).ToList());
        }

        [Fact]
        public async Task CreateClientAsync_WithDomainException_ReturnsFail()
        {
            var staffId = Guid.NewGuid();
            var clientId = Guid.NewGuid();
            var request = new CreateClientRequest(
                clientId,
                "", // Invalid - empty name
                "Doe",
                "john@example.com",
                "1234567890",
                new DateOnly(1990, 1, 1),
                "123 Main St",
                "",
                LoyaltyLevel.Bronze,
                staffId);

            var staff = new Staff("Jane", "Smith", "1234567890", "Physiotherapist", 12345, new List<Clinic>());
            _mockStaffRepository.Setup(x => x.GetStaffAsync(staffId))
                .ReturnsAsync(Result.Ok(staff));

            var result = await _handler.CreateClientAsync(request);
            Assert.True(result.IsFailed);
        }

        [Fact]
        public async Task DeleteClientAsync_WithValidId_ReturnsSuccess()
        {
            var clientId = Guid.NewGuid();
            var request = new DeleteClientRequest(clientId);

            _mockClientRepository.Setup(x => x.DeleteClientAsync(clientId))
                .ReturnsAsync(Result.Ok());

            var result = await _handler.DeleteClientAsync(request);

            Assert.True(result.IsSuccess);
            _mockClientRepository.Verify(x => x.DeleteClientAsync(clientId), Times.Once);
        }

        [Fact]
        public async Task DeleteClientAsync_WithEmptyId_ReturnsFail()
        {
            var request = new DeleteClientRequest(Guid.Empty);

            var result = await _handler.DeleteClientAsync(request);

            Assert.True(result.IsFailed);
            Assert.Contains("Client ID cannot be empty", result.Errors.Select(e => e.Message).ToList());
            _mockClientRepository.Verify(x => x.DeleteClientAsync(It.IsAny<Guid>()), Times.Never);
        }

        [Fact]
        public async Task UpdateClientAsync_WithValidRequest_ReturnsSuccess()
        {
            var clientId = Guid.NewGuid();
            var client = Client.Create("John", "Doe", "john@example.com", "1234567890",
                new DateOnly(1990, 1, 1), "123 Main St", "Note", Guid.NewGuid(), LoyaltyLevel.Bronze);

            var request = new UpdateClientRequest(
                clientId,
                "Jane",
                "Smith",
                "jane@example.com",
                "0987654321",
                new DateOnly(1995, 5, 5),
                "456 Oak Ave",
                "Note",
                Guid.NewGuid());

            _mockClientRepository.Setup(x => x.GetClientAsync(clientId))
                .ReturnsAsync(Result.Ok(client));

            _mockClientRepository.Setup(x => x.UpdateClientAsync(It.IsAny<Client>()))
                .ReturnsAsync(Result.Ok());

            var result = await _handler.UpdateClientAsync(request);

            Assert.True(result.IsSuccess);
            _mockClientRepository.Verify(x => x.UpdateClientAsync(It.IsAny<Client>()), Times.Once);
        }

        [Fact]
        public async Task UpdateClientAsync_WithEmptyClientId_ReturnsFail()
        {
            var request = new UpdateClientRequest(
                Guid.Empty,
                "Jane",
                "Smith",
                "jane@example.com",
                "0987654321",
                new DateOnly(1995, 5, 5),
                "456 Oak Ave",
                "Note",
                Guid.NewGuid());

            var result = await _handler.UpdateClientAsync(request);

            Assert.True(result.IsFailed);
            Assert.Contains("Client ID cannot be empty", result.Errors.Select(e => e.Message));
        }

        [Fact]
        public async Task UpdateClientAsync_WithNonexistentClient_ReturnsFail()
        {
            var clientId = Guid.NewGuid();
            var request = new UpdateClientRequest(
                clientId,
                "Jane",
                "Smith",
                "jane@example.com",
                "0987654321",
                new DateOnly(1995, 5, 5),
                "456 Oak Ave",
                "Note",
                Guid.NewGuid());

            _mockClientRepository.Setup(x => x.GetClientAsync(clientId))
                .ReturnsAsync(Result.Fail("Client not found"));

            var result = await _handler.UpdateClientAsync(request);

            Assert.True(result.IsFailed);
            Assert.Contains($"Client with ID {clientId} was not found", result.Errors.Select(e => e.Message));
        }

        [Fact]
        public async Task UpdateClientPrefferedStaffAsync_WithValidRequest_ReturnsSuccess()
        {
            var clientId = Guid.NewGuid();
            var staffId = Guid.NewGuid();
            var client = Client.Create("John", "Doe", "john@example.com", "1234567890",
                new DateOnly(1990, 1, 1), "123 Main St", "Note", Guid.NewGuid(), LoyaltyLevel.Bronze);
            var staff = new Staff("Jane", "Smith", "1234567890", "Physiotherapist", 12345, new List<Clinic>());

            var request = new UpdateClientStaffRequest(
                clientId,
                staffId
            );

            _mockClientRepository.Setup(x => x.GetClientAsync(clientId))
                .ReturnsAsync(Result.Ok(client));

            _mockStaffRepository.Setup(x => x.GetStaffAsync(staffId))
                .ReturnsAsync(Result.Ok(staff));

            var result = await _handler.UpdateClientPrefferedStaffAsync(request);

            Assert.True(result.IsSuccess);
        }

        [Fact]
        public async Task UpdateClientPrefferedStaffAsync_WithEmptyClientId_ReturnsFail()
        {
            var request = new UpdateClientStaffRequest(Guid.Empty, Guid.NewGuid());

            var result = await _handler.UpdateClientPrefferedStaffAsync(request);

            Assert.True(result.IsFailed);
            Assert.Contains("Client ID cannot be empty", result.Errors.Select(e => e.Message));
        }

        [Fact]
        public async Task UpdateClientPrefferedStaffAsync_WithNonexistentStaff_ReturnsFail()
        {
            var clientId = Guid.NewGuid();
            var staffId = Guid.NewGuid();
            var client = Client.Create("John", "Doe", "john@example.com", "1234567890",
                new DateOnly(1990, 1, 1), "123 Main St", "Note", Guid.NewGuid(), LoyaltyLevel.Bronze);

            var request = new UpdateClientStaffRequest(
                clientId,
                staffId
            );

            _mockClientRepository.Setup(x => x.GetClientAsync(clientId))
                .ReturnsAsync(Result.Ok(client));

            _mockStaffRepository.Setup(x => x.GetStaffAsync(staffId))
                .ReturnsAsync(Result.Fail("Staff not found"));

            var result = await _handler.UpdateClientPrefferedStaffAsync(request);

            Assert.True(result.IsFailed);
            Assert.Contains($"Staff with ID {staffId} was not found", result.Errors.Select(e => e.Message));
        }

        [Fact]
        public async Task UpdateClientNoteAsync_WithValidRequest_ReturnsSuccess()
        {
            var clientId = Guid.NewGuid();
            var client = Client.Create("John", "Doe", "john@example.com", "1234567890",
                new DateOnly(1990, 1, 1), "123 Main St", "Old Note", Guid.NewGuid(), LoyaltyLevel.Bronze);

            var request = new UpdateClientNoteRequest(
                clientId,
                "Updated Note"
            );

            _mockClientRepository.Setup(x => x.GetClientAsync(clientId))
                .ReturnsAsync(Result.Ok(client));

            _mockClientRepository.Setup(x => x.UpdateClientAsync(It.IsAny<Client>()))
                .ReturnsAsync(Result.Ok());

            var result = await _handler.UpdateClientNoteAsync(request);

            Assert.True(result.IsSuccess);
            _mockClientRepository.Verify(x => x.UpdateClientAsync(It.IsAny<Client>()), Times.Once);
        }

        [Fact]
        public async Task UpdateClientNoteAsync_WithEmptyClientId_ReturnsFail()
        {
            var request = new UpdateClientNoteRequest(Guid.Empty, "Note");

            var result = await _handler.UpdateClientNoteAsync(request);

            Assert.True(result.IsFailed);
            Assert.Contains("Client ID cannot be empty", result.Errors.Select(e => e.Message).ToList());
        }

        [Fact]
        public async Task UpdateClientNoteAsync_WithRepositoryFailure_ReturnsFail()
        {
            var clientId = Guid.NewGuid();
            var client = Client.Create("John", "Doe", "john@example.com", "1234567890",
                new DateOnly(1990, 1, 1), "123 Main St", "Note", Guid.NewGuid(), LoyaltyLevel.Bronze);

            var request = new UpdateClientNoteRequest(
                clientId,
                "Updated Note"
            );

            _mockClientRepository.Setup(x => x.GetClientAsync(clientId))
                .ReturnsAsync(Result.Ok(client));

            _mockClientRepository.Setup(x => x.UpdateClientAsync(It.IsAny<Client>()))
                .ReturnsAsync(Result.Fail("Database error"));

            var result = await _handler.UpdateClientNoteAsync(request);

            Assert.True(result.IsFailed);
            Assert.Contains("An error occurred while updating the client's note", result.Errors.Select(e => e.Message));
        }
    }
}