using FluentResults;
using FysioEnterprise.Domain.Entities;
using FysioEnterprise.Domain.Exceptions;
using FysioEnterprise.Domain.Service;
using FysioEnterprise.UseCase.CommandHandlers.PromotionCommands;
using FysioEnterprise.UseCase.IRepositories;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;
using static FysioEnterprise.Facade.RequestModels.PromotionRequests;

namespace FysioEnterprise.UnitTests.UseCase.CommandHandlers
{
    public class PromotionCommandHandlerTests
    {
        private readonly Mock<IPromotionRepository> _mockPromotionRepository;
        private readonly Mock<ITimeNow> _mockTimeNow;
        private readonly PromotionCommandHandler _handler;

        public PromotionCommandHandlerTests()
        {
            _mockPromotionRepository = new Mock<IPromotionRepository>();
            _mockTimeNow = new Mock<ITimeNow>();
            _handler = new PromotionCommandHandler(_mockPromotionRepository.Object, _mockTimeNow.Object);
        }

        [Fact]
        public async Task CreatePromotionAsync_WithNullRequest_ReturnsFail()
        {
            // Arrange
            CreatePromotionRequest request = null;

            // Act
            var result = await _handler.CreatePromotionAsync(request);

            // Assert
            Assert.True(result.IsFailed);
            Assert.Contains("Request cannot be null", result.Errors[0].Message);
        }

        [Fact]
        public async Task CreatePromotionAsync_WithExistingPromotionID_ReturnsFail()
        {
            var promotionId = Guid.NewGuid();
            var existingPromotion = Promotion.Create("Existing", 10, DateTime.Now.AddDays(1), DateTime.Now.AddDays(30));

            var request = new CreatePromotionRequest(
                promotionId,
                "Summer Sale",
                20,
                DateTime.Now.AddDays(1),
                DateTime.Now.AddDays(30)
            );

            _mockPromotionRepository.Setup(x => x.GetPromotionAsync(promotionId))
                .ReturnsAsync(existingPromotion);

            var result = await _handler.CreatePromotionAsync(request);

            Assert.True(result.IsFailed);
            Assert.Contains("already exists", result.Errors[0].Message);
        }

        [Fact]
        public async Task CreatePromotionAsync_WithValidRequest_ReturnsSuccess()
        {
            var promotionId = Guid.NewGuid();
            var startDate = DateTime.Now.AddDays(1);
            var endDate = DateTime.Now.AddDays(30);

            var request = new CreatePromotionRequest(
                promotionId,
                "Summer Sale",
                20,
                startDate,
                endDate
            );

            _mockPromotionRepository.Setup(x => x.GetPromotionAsync(promotionId))
                .ReturnsAsync((Promotion)null);

            _mockPromotionRepository.Setup(x => x.CreatePromotionAsync(It.IsAny<Promotion>()))
                .ReturnsAsync(Result.Ok());

            _mockTimeNow.Setup(x => x.Now())
                .Returns(DateTime.Now);

            var result = await _handler.CreatePromotionAsync(request);

            Assert.True(result.IsSuccess);
            _mockPromotionRepository.Verify(x => x.CreatePromotionAsync(It.IsAny<Promotion>()), Times.Once);
        }

        [Fact]
        public async Task CreatePromotionAsync_WithInvalidTimeRange_ReturnsFail()
        {
            var promotionId = Guid.NewGuid();
            var startDate = DateTime.Now.AddDays(-5); // Past date
            var endDate = DateTime.Now.AddDays(-1);

            var request = new CreatePromotionRequest(
                promotionId,
                "Summer Sale",
                20,
                startDate,
                endDate
            );

            _mockPromotionRepository.Setup(x => x.GetPromotionAsync(promotionId))
                .ReturnsAsync((Promotion)null);

            _mockTimeNow.Setup(x => x.Now())
                .Returns(DateTime.Now);

            var result = await _handler.CreatePromotionAsync(request);

            Assert.True(result.IsFailed);
            _mockPromotionRepository.Verify(x => x.CreatePromotionAsync(It.IsAny<Promotion>()), Times.Never);
        }

        [Fact]
        public async Task UpdatePromotionAsync_WithNullRequest_ReturnsFail()
        {
            UpdatePromotionRequest request = null;

            var result = await _handler.UpdatePromotionAsync(request);

            Assert.True(result.IsFailed);
            Assert.Contains("Request cannot be null", result.Errors[0].Message);
        }

        [Fact]
        public async Task UpdatePromotionAsync_WithEmptyPromotionID_ReturnsFail()
        {
            var request = new UpdatePromotionRequest(
                Guid.Empty,
                "Summer Sale",
                20,
                DateTime.Now.AddDays(1),
                DateTime.Now.AddDays(30)
            );

            var result = await _handler.UpdatePromotionAsync(request);

            Assert.True(result.IsFailed);
            Assert.Contains("Promotion ID cannot be empty", result.Errors[0].Message);
        }

        [Fact]
        public async Task UpdatePromotionAsync_WithNullName_ReturnsFail()
        {
            var request = new UpdatePromotionRequest(
                Guid.NewGuid(),
                null,
                20,
                DateTime.Now.AddDays(1),
                DateTime.Now.AddDays(30)
            );

            var result = await _handler.UpdatePromotionAsync(request);

            Assert.True(result.IsFailed);
            Assert.Contains("Promotion name cannot be null", result.Errors[0].Message);
        }

        [Fact]
        public async Task UpdatePromotionAsync_WithNegativeDiscountPercentage_ReturnsFail()
        {
            var request = new UpdatePromotionRequest(
                Guid.NewGuid(),
                "Summer Sale",
                -10,
                DateTime.Now.AddDays(1),
                DateTime.Now.AddDays(30)
            );

            var result = await _handler.UpdatePromotionAsync(request);

            Assert.True(result.IsFailed);
            Assert.Contains("must be greater than zero", result.Errors[0].Message);
        }

        [Fact]
        public async Task UpdatePromotionAsync_WithNonExistentPromotion_ReturnsFail()
        {
            var promotionId = Guid.NewGuid();
            var request = new UpdatePromotionRequest(
                promotionId,
                "Summer Sale",
                20,
                DateTime.Now.AddDays(1),
                DateTime.Now.AddDays(30)
            );

            _mockPromotionRepository.Setup(x => x.GetPromotionAsync(promotionId))
                .ReturnsAsync((Promotion)null);

            _mockTimeNow.Setup(x => x.Now())
                .Returns(DateTime.Now);

            var result = await _handler.UpdatePromotionAsync(request);

            Assert.True(result.IsFailed);
            Assert.Contains("Promotion not found", result.Errors[0].Message);
        }

        [Fact]
        public async Task UpdatePromotionAsync_WithValidRequest_ReturnsSuccess()
        {
            var promotionId = Guid.NewGuid();
            var existingPromotion = Promotion.Create("Existing", 10, DateTime.Now.AddDays(1), DateTime.Now.AddDays(30));
            var startDate = DateTime.Now.AddDays(1);
            var endDate = DateTime.Now.AddDays(30);

            var request = new UpdatePromotionRequest(
                promotionId,
                "Summer Sale Updated",
                25,
                startDate,
                endDate
            );

            _mockPromotionRepository.Setup(x => x.GetPromotionAsync(promotionId))
                .ReturnsAsync(existingPromotion);

            _mockPromotionRepository.Setup(x => x.UpdatePromotionAsync(It.IsAny<Promotion>()))
                .ReturnsAsync(Result.Ok());

            _mockTimeNow.Setup(x => x.Now())
                .Returns(DateTime.Now);

            var result = await _handler.UpdatePromotionAsync(request);

            Assert.True(result.IsSuccess);
            _mockPromotionRepository.Verify(x => x.UpdatePromotionAsync(It.IsAny<Promotion>()), Times.Once);
        }

        [Fact]
        public async Task DeletePromotionAsync_WithNullRequest_ReturnsFail()
        {
            DeletePromotionRequest request = null;

            var result = await _handler.DeletePromotionAsync(request);

            Assert.True(result.IsFailed);
            Assert.Contains("Command cannot be null", result.Errors[0].Message);
        }

        [Fact]
        public async Task DeletePromotionAsync_WithEmptyPromotionID_ReturnsFail()
        {
            var request = new DeletePromotionRequest(
                Guid.Empty
            );

            var result = await _handler.DeletePromotionAsync(request);

            Assert.True(result.IsFailed);
            Assert.Contains("Promotion ID cannot be empty", result.Errors[0].Message);
        }

        [Fact]
        public async Task DeletePromotionAsync_WithNonExistentPromotion_ReturnsFail()
        {
            var promotionId = Guid.NewGuid();
            var request = new DeletePromotionRequest(
                promotionId
            );

            _mockPromotionRepository.Setup(x => x.GetPromotionAsync(promotionId))
                .ReturnsAsync((Promotion)null);

            var result = await _handler.DeletePromotionAsync(request);

            Assert.True(result.IsFailed);
            Assert.Contains("Promotion not found", result.Errors[0].Message);
        }

        [Fact]
        public async Task DeletePromotionAsync_WithValidRequest_ReturnsSuccess()
        {
            var promotionId = Guid.NewGuid();
            var existingPromotion = Promotion.Create("Existing", 10, DateTime.Now.AddDays(1), DateTime.Now.AddDays(30));
            var request = new DeletePromotionRequest(
                promotionId
            );

            _mockPromotionRepository.Setup(x => x.GetPromotionAsync(promotionId))
                .ReturnsAsync(existingPromotion);

            _mockPromotionRepository.Setup(x => x.DeletePromotionAsync(promotionId))
                .ReturnsAsync(existingPromotion);

            var result = await _handler.DeletePromotionAsync(request);

            Assert.True(result.IsSuccess);
            _mockPromotionRepository.Verify(x => x.DeletePromotionAsync(promotionId), Times.Once);
        }
    }
}