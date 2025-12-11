using Moq;
using MSP.Application.Repositories;
using MSP.Application.Services.Implementations.Limitation;
using MSP.Application.Services.Interfaces.Limitation;
using MSP.Domain.Entities;
using Xunit;

namespace MSP.Tests.Services.LimitationServicesTest
{
    public class DeleteLimitationTest
    {
        private readonly Mock<ILimitationRepository> _mockLimitationRepository;
        private readonly ILimitationService _limitationService;

        public DeleteLimitationTest()
        {
            _mockLimitationRepository = new Mock<ILimitationRepository>();
            _limitationService = new LimitationService(_mockLimitationRepository.Object);
        }

        #region TC_DeleteLim_01 - Limitation not found
        [Fact]
        public async Task DeleteLimitationAsync_LimitationNotFound_ReturnsError()
        {
            // Arrange
            var limitationId = Guid.NewGuid();

            _mockLimitationRepository
                .Setup(x => x.GetByIdAsync(limitationId))
                .ReturnsAsync((Limitation)null);

            // Act
            var result = await _limitationService.DeleteLimitationAsync(limitationId);

            // Assert
            Assert.False(result.Success);
            Assert.Equal("Limitation not found", result.Message);
            Assert.Null(result.Data);

            _mockLimitationRepository.Verify(x => x.GetByIdAsync(limitationId), Times.Once);
            _mockLimitationRepository.Verify(x => x.SoftDeleteAsync(It.IsAny<Limitation>()), Times.Never);
            _mockLimitationRepository.Verify(x => x.SaveChangesAsync(), Times.Never);
        }
        #endregion

        #region TC_DeleteLim_02 - Limitation already deleted
        [Fact]
        public async Task DeleteLimitationAsync_LimitationAlreadyDeleted_ReturnsError()
        {
            // Arrange
            var limitationId = Guid.NewGuid();
            var deletedLimitation = new Limitation
            {
                Id = limitationId,
                Name = "Already Deleted",
                IsDeleted = true
            };

            _mockLimitationRepository
                .Setup(x => x.GetByIdAsync(limitationId))
                .ReturnsAsync(deletedLimitation);

            // Act
            var result = await _limitationService.DeleteLimitationAsync(limitationId);

            // Assert
            Assert.False(result.Success);
            Assert.Equal("Limitation not found", result.Message);
            Assert.Null(result.Data);

            _mockLimitationRepository.Verify(x => x.GetByIdAsync(limitationId), Times.Once);
            _mockLimitationRepository.Verify(x => x.SoftDeleteAsync(It.IsAny<Limitation>()), Times.Never);
            _mockLimitationRepository.Verify(x => x.SaveChangesAsync(), Times.Never);
        }
        #endregion

        #region TC_DeleteLim_03 - Success soft delete
        [Fact]
        public async Task DeleteLimitationAsync_ValidLimitation_DeletesSuccessfully()
        {
            // Arrange
            var limitationId = Guid.NewGuid();
            var limitation = new Limitation
            {
                Id = limitationId,
                Name = "Test Limitation",
                Description = "To be deleted",
                IsDeleted = false,
                LimitValue = 100
            };

            _mockLimitationRepository
                .Setup(x => x.GetByIdAsync(limitationId))
                .ReturnsAsync(limitation);

            _mockLimitationRepository
                .Setup(x => x.SoftDeleteAsync(limitation))
                .Returns(Task.CompletedTask);

            _mockLimitationRepository
                .Setup(x => x.SaveChangesAsync())
                .Returns(Task.CompletedTask);

            // Act
            var result = await _limitationService.DeleteLimitationAsync(limitationId);

            // Assert
            Assert.True(result.Success);
            Assert.Equal("Limitation deleted successfully", result.Message);
            Assert.Null(result.Data);

            _mockLimitationRepository.Verify(x => x.GetByIdAsync(limitationId), Times.Once);
            _mockLimitationRepository.Verify(x => x.SoftDeleteAsync(limitation), Times.Once);
            _mockLimitationRepository.Verify(x => x.SaveChangesAsync(), Times.Once);
        }
        #endregion

        #region TC_DeleteLim_04 - Empty Guid
        [Fact]
        public async Task DeleteLimitationAsync_EmptyGuid_ReturnsNotFound()
        {
            // Arrange
            var limitationId = Guid.Empty;

            _mockLimitationRepository
                .Setup(x => x.GetByIdAsync(limitationId))
                .ReturnsAsync((Limitation)null);

            // Act
            var result = await _limitationService.DeleteLimitationAsync(limitationId);

            // Assert
            Assert.False(result.Success);
            Assert.Equal("Limitation not found", result.Message);
            Assert.Null(result.Data);

            _mockLimitationRepository.Verify(x => x.GetByIdAsync(Guid.Empty), Times.Once);
            _mockLimitationRepository.Verify(x => x.SoftDeleteAsync(It.IsAny<Limitation>()), Times.Never);
        }
        #endregion

        #region TC_DeleteLim_07 - Delete limitation with relationships
        [Fact]
        public async Task DeleteLimitationAsync_LimitationWithRelationships_DeletesSuccessfully()
        {
            // Arrange
            var limitationId = Guid.NewGuid();
            var limitation = new Limitation
            {
                Id = limitationId,
                Name = "Limitation with packages",
                Description = "Has related packages",
                IsDeleted = false,
                LimitValue = 50,
                // Assume this limitation is used by packages (business logic)
            };

            _mockLimitationRepository
                .Setup(x => x.GetByIdAsync(limitationId))
                .ReturnsAsync(limitation);

            _mockLimitationRepository
                .Setup(x => x.SoftDeleteAsync(limitation))
                .Returns(Task.CompletedTask);

            _mockLimitationRepository
                .Setup(x => x.SaveChangesAsync())
                .Returns(Task.CompletedTask);

            // Act
            var result = await _limitationService.DeleteLimitationAsync(limitationId);

            // Assert
            Assert.True(result.Success);
            Assert.Equal("Limitation deleted successfully", result.Message);

            // Soft delete allows deletion even with relationships
            _mockLimitationRepository.Verify(x => x.SoftDeleteAsync(limitation), Times.Once);
            _mockLimitationRepository.Verify(x => x.SaveChangesAsync(), Times.Once);
        }
        #endregion

    }
}
