using Moq;
using MSP.Application.Models.Requests.Limitation;
using MSP.Application.Repositories;
using MSP.Application.Services.Implementations.Limitation;
using MSP.Domain.Entities;
using Xunit;

namespace MSP.Tests.Services.LimitationServicesTest
{
    public class UpdateLimitationTest
    {
        private readonly Mock<ILimitationRepository> _mockLimitationRepository;
        private readonly LimitationService _limitationService;

        public UpdateLimitationTest()
        {
            _mockLimitationRepository = new Mock<ILimitationRepository>();
            _limitationService = new LimitationService(_mockLimitationRepository.Object);
        }

        #region TC_UpdateLim_01 - Limitation not found
        [Fact]
        public async Task UpdateLimitationAsync_LimitationNotFound_ReturnsError()
        {
            // Arrange
            var request = new UpdateLimitationRequest
            {
                LimitationId = Guid.NewGuid(),
                Name = "Updated Name",
                Description = "Updated Description",
                IsUnlimited = false,
                LimitValue = 100,
                LimitUnit = "Units",
                LimitationType = "Test"
            };

            _mockLimitationRepository
                .Setup(x => x.GetByIdAsync(request.LimitationId))
                .ReturnsAsync((Limitation)null);

            // Act
            var result = await _limitationService.UpdateLimitationAsync(request);

            // Assert
            Assert.False(result.Success);
            Assert.Equal("Limitation not found", result.Message);
            Assert.Null(result.Data);

            _mockLimitationRepository.Verify(x => x.GetByIdAsync(request.LimitationId), Times.Once);
            _mockLimitationRepository.Verify(x => x.UpdateAsync(It.IsAny<Limitation>()), Times.Never);
            _mockLimitationRepository.Verify(x => x.SaveChangesAsync(), Times.Never);
        }
        #endregion

        #region TC_UpdateLim_02 - Limitation is deleted
        [Fact]
        public async Task UpdateLimitationAsync_LimitationIsDeleted_ReturnsError()
        {
            // Arrange
            var limitationId = Guid.NewGuid();
            var deletedLimitation = new Limitation
            {
                Id = limitationId,
                Name = "Deleted Limitation",
                IsDeleted = true
            };

            var request = new UpdateLimitationRequest
            {
                LimitationId = limitationId,
                Name = "Updated Name",
                Description = "Updated Description",
                IsUnlimited = false,
                LimitValue = 100,
                LimitUnit = "Units",
                LimitationType = "Test"
            };

            _mockLimitationRepository
                .Setup(x => x.GetByIdAsync(request.LimitationId))
                .ReturnsAsync(deletedLimitation);

            // Act
            var result = await _limitationService.UpdateLimitationAsync(request);

            // Assert
            Assert.False(result.Success);
            Assert.Equal("Limitation not found", result.Message);
            Assert.Null(result.Data);

            _mockLimitationRepository.Verify(x => x.GetByIdAsync(request.LimitationId), Times.Once);
            _mockLimitationRepository.Verify(x => x.UpdateAsync(It.IsAny<Limitation>()), Times.Never);
            _mockLimitationRepository.Verify(x => x.SaveChangesAsync(), Times.Never);
        }
        #endregion

        #region TC_UpdateLim_03 - Success with complete update
        [Fact]
        public async Task UpdateLimitationAsync_ValidRequest_UpdatesSuccessfully()
        {
            // Arrange
            var limitationId = Guid.NewGuid();
            var existingLimitation = new Limitation
            {
                Id = limitationId,
                Name = "Old Name",
                Description = "Old Description",
                IsUnlimited = false,
                LimitValue = 10,
                LimitUnit = "OldUnit",
                LimitationType = "OldType",
                IsDeleted = false,
                CreatedAt = DateTime.UtcNow.AddDays(-10),
                UpdatedAt = DateTime.UtcNow.AddDays(-5)
            };

            var request = new UpdateLimitationRequest
            {
                LimitationId = limitationId,
                Name = "New Name",
                Description = "New Description",
                IsUnlimited = true,
                LimitValue = 100,
                LimitUnit = "NewUnit",
                LimitationType = "NewType"
            };

            _mockLimitationRepository
                .Setup(x => x.GetByIdAsync(request.LimitationId))
                .ReturnsAsync(existingLimitation);

            _mockLimitationRepository
                .Setup(x => x.UpdateAsync(It.IsAny<Limitation>()))
                .Returns(Task.CompletedTask);

            _mockLimitationRepository
                .Setup(x => x.SaveChangesAsync())
                .Returns(Task.CompletedTask);

            // Act
            var result = await _limitationService.UpdateLimitationAsync(request);

            // Assert
            Assert.True(result.Success);
            Assert.Equal("Limitation updated successfully", result.Message);
            Assert.NotNull(result.Data);
            Assert.Equal(limitationId, result.Data.Id);
            Assert.Equal("New Name", result.Data.Name);
            Assert.Equal("New Description", result.Data.Description);
            Assert.True(result.Data.IsUnlimited);
            Assert.Equal(100, result.Data.LimitValue);
            Assert.Equal("NewUnit", result.Data.LimitUnit);

            // Verify entity updated
            Assert.Equal("New Name", existingLimitation.Name);
            Assert.Equal("New Description", existingLimitation.Description);
            Assert.True(existingLimitation.IsUnlimited);
            Assert.Equal(100, existingLimitation.LimitValue);
            Assert.Equal("NewUnit", existingLimitation.LimitUnit);
            Assert.Equal("NewType", existingLimitation.LimitationType);
            Assert.True((DateTime.UtcNow - existingLimitation.UpdatedAt).TotalSeconds < 2);

            _mockLimitationRepository.Verify(x => x.GetByIdAsync(request.LimitationId), Times.Once);
            _mockLimitationRepository.Verify(x => x.UpdateAsync(existingLimitation), Times.Once);
            _mockLimitationRepository.Verify(x => x.SaveChangesAsync(), Times.Once);
        }
        #endregion

        #region TC_UpdateLim_04 - Success with null description
        [Fact]
        public async Task UpdateLimitationAsync_NullDescription_UpdatesSuccessfully()
        {
            // Arrange
            var limitationId = Guid.NewGuid();
            var existingLimitation = new Limitation
            {
                Id = limitationId,
                Name = "Old Name",
                Description = "Old Description",
                IsDeleted = false
            };

            var request = new UpdateLimitationRequest
            {
                LimitationId = limitationId,
                Name = "New Name",
                Description = null, // Set to null
                IsUnlimited = false,
                LimitValue = 50,
                LimitUnit = "Units",
                LimitationType = "Test"
            };

            _mockLimitationRepository
                .Setup(x => x.GetByIdAsync(request.LimitationId))
                .ReturnsAsync(existingLimitation);

            _mockLimitationRepository
                .Setup(x => x.UpdateAsync(It.IsAny<Limitation>()))
                .Returns(Task.CompletedTask);

            _mockLimitationRepository
                .Setup(x => x.SaveChangesAsync())
                .Returns(Task.CompletedTask);

            // Act
            var result = await _limitationService.UpdateLimitationAsync(request);

            // Assert
            Assert.True(result.Success);
            Assert.NotNull(result.Data);
            Assert.Null(result.Data.Description);
            Assert.Null(existingLimitation.Description);

            _mockLimitationRepository.Verify(x => x.UpdateAsync(existingLimitation), Times.Once);
            _mockLimitationRepository.Verify(x => x.SaveChangesAsync(), Times.Once);
        }
        #endregion

        #region TC_UpdateLim_05 - Verify all fields updated
        [Fact]
        public async Task UpdateLimitationAsync_ValidRequest_UpdatesAllFields()
        {
            // Arrange
            var limitationId = Guid.NewGuid();
            var existingLimitation = new Limitation
            {
                Id = limitationId,
                Name = "Original Name",
                Description = "Original Description",
                IsUnlimited = false,
                LimitValue = 10,
                LimitUnit = "Original",
                LimitationType = "OriginalType",
                IsDeleted = false,
                CreatedAt = DateTime.UtcNow.AddDays(-30),
                UpdatedAt = DateTime.UtcNow.AddDays(-10)
            };

            var originalCreatedAt = existingLimitation.CreatedAt;
            var originalUpdatedAt = existingLimitation.UpdatedAt;

            var request = new UpdateLimitationRequest
            {
                LimitationId = limitationId,
                Name = "Updated Name",
                Description = "Updated Description",
                IsUnlimited = true,
                LimitValue = 999,
                LimitUnit = "Updated",
                LimitationType = "UpdatedType"
            };

            _mockLimitationRepository
                .Setup(x => x.GetByIdAsync(request.LimitationId))
                .ReturnsAsync(existingLimitation);

            _mockLimitationRepository
                .Setup(x => x.UpdateAsync(It.IsAny<Limitation>()))
                .Returns(Task.CompletedTask);

            _mockLimitationRepository
                .Setup(x => x.SaveChangesAsync())
                .Returns(Task.CompletedTask);

            // Act
            var result = await _limitationService.UpdateLimitationAsync(request);

            // Assert
            Assert.True(result.Success);

            // Verify all fields updated
            Assert.Equal("Updated Name", existingLimitation.Name);
            Assert.Equal("Updated Description", existingLimitation.Description);
            Assert.True(existingLimitation.IsUnlimited);
            Assert.Equal(999, existingLimitation.LimitValue);
            Assert.Equal("Updated", existingLimitation.LimitUnit);
            Assert.Equal("UpdatedType", existingLimitation.LimitationType);

            // Verify timestamps
            Assert.Equal(originalCreatedAt, existingLimitation.CreatedAt); // CreatedAt unchanged
            Assert.NotEqual(originalUpdatedAt, existingLimitation.UpdatedAt); // UpdatedAt changed
            Assert.True((DateTime.UtcNow - existingLimitation.UpdatedAt).TotalSeconds < 2);

            _mockLimitationRepository.Verify(x => x.UpdateAsync(existingLimitation), Times.Once);
            _mockLimitationRepository.Verify(x => x.SaveChangesAsync(), Times.Once);
        }
        #endregion

        #region TC_UpdateLim_06 - Update to unlimited limitation
        [Fact]
        public async Task UpdateLimitationAsync_ChangeToUnlimited_UpdatesSuccessfully()
        {
            // Arrange
            var limitationId = Guid.NewGuid();
            var existingLimitation = new Limitation
            {
                Id = limitationId,
                Name = "Limited",
                IsUnlimited = false,
                LimitValue = 100,
                IsDeleted = false
            };

            var request = new UpdateLimitationRequest
            {
                LimitationId = limitationId,
                Name = "Now Unlimited",
                Description = "Changed to unlimited",
                IsUnlimited = true,
                LimitValue = 0, // Should be ignored when unlimited
                LimitUnit = "Units",
                LimitationType = "Test"
            };

            _mockLimitationRepository
                .Setup(x => x.GetByIdAsync(request.LimitationId))
                .ReturnsAsync(existingLimitation);

            _mockLimitationRepository
                .Setup(x => x.UpdateAsync(It.IsAny<Limitation>()))
                .Returns(Task.CompletedTask);

            _mockLimitationRepository
                .Setup(x => x.SaveChangesAsync())
                .Returns(Task.CompletedTask);

            // Act
            var result = await _limitationService.UpdateLimitationAsync(request);

            // Assert
            Assert.True(result.Success);
            Assert.NotNull(result.Data);
            Assert.Equal("Now Unlimited", result.Data.Name);
            Assert.True(result.Data.IsUnlimited);
            Assert.Equal(0, result.Data.LimitValue); // Value stored even when unlimited

            Assert.True(existingLimitation.IsUnlimited);
            Assert.Equal(0, existingLimitation.LimitValue);

            _mockLimitationRepository.Verify(x => x.UpdateAsync(existingLimitation), Times.Once);
            _mockLimitationRepository.Verify(x => x.SaveChangesAsync(), Times.Once);
        }
        #endregion

        #region TC_UpdateLim_07 - UpdateAsync throws exception
        [Fact]
        public async Task UpdateLimitationAsync_UpdateAsyncFails_ThrowsException()
        {
            // Arrange
            var limitationId = Guid.NewGuid();
            var existingLimitation = new Limitation
            {
                Id = limitationId,
                Name = "Test",
                IsDeleted = false
            };

            var request = new UpdateLimitationRequest
            {
                LimitationId = limitationId,
                Name = "Updated",
                Description = "Test",
                IsUnlimited = false,
                LimitValue = 10,
                LimitUnit = "Units",
                LimitationType = "Test"
            };

            _mockLimitationRepository
                .Setup(x => x.GetByIdAsync(request.LimitationId))
                .ReturnsAsync(existingLimitation);

            _mockLimitationRepository
                .Setup(x => x.UpdateAsync(It.IsAny<Limitation>()))
                .ThrowsAsync(new Exception("Database update failed"));

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(
                async () => await _limitationService.UpdateLimitationAsync(request));

            _mockLimitationRepository.Verify(x => x.GetByIdAsync(request.LimitationId), Times.Once);
            _mockLimitationRepository.Verify(x => x.UpdateAsync(existingLimitation), Times.Once);
            _mockLimitationRepository.Verify(x => x.SaveChangesAsync(), Times.Never);
        }
        #endregion

        #region TC_UpdateLim_08 - SaveChangesAsync throws exception
        [Fact]
        public async Task UpdateLimitationAsync_SaveChangesFails_ThrowsException()
        {
            // Arrange
            var limitationId = Guid.NewGuid();
            var existingLimitation = new Limitation
            {
                Id = limitationId,
                Name = "Test",
                IsDeleted = false
            };

            var request = new UpdateLimitationRequest
            {
                LimitationId = limitationId,
                Name = "Updated",
                Description = "Test",
                IsUnlimited = false,
                LimitValue = 10,
                LimitUnit = "Units",
                LimitationType = "Test"
            };

            _mockLimitationRepository
                .Setup(x => x.GetByIdAsync(request.LimitationId))
                .ReturnsAsync(existingLimitation);

            _mockLimitationRepository
                .Setup(x => x.UpdateAsync(It.IsAny<Limitation>()))
                .Returns(Task.CompletedTask);

            _mockLimitationRepository
                .Setup(x => x.SaveChangesAsync())
                .ThrowsAsync(new Exception("Failed to save changes"));

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(
                async () => await _limitationService.UpdateLimitationAsync(request));

            _mockLimitationRepository.Verify(x => x.GetByIdAsync(request.LimitationId), Times.Once);
            _mockLimitationRepository.Verify(x => x.UpdateAsync(existingLimitation), Times.Once);
            _mockLimitationRepository.Verify(x => x.SaveChangesAsync(), Times.Once);
        }
        #endregion

        #region TC_UpdateLim_09 - Update with zero limit value
        [Fact]
        public async Task UpdateLimitationAsync_ZeroLimitValue_UpdatesSuccessfully()
        {
            // Arrange
            var limitationId = Guid.NewGuid();
            var existingLimitation = new Limitation
            {
                Id = limitationId,
                Name = "Test",
                LimitValue = 100,
                IsDeleted = false
            };

            var request = new UpdateLimitationRequest
            {
                LimitationId = limitationId,
                Name = "Zero Limit",
                Description = "Set to zero",
                IsUnlimited = false,
                LimitValue = 0,
                LimitUnit = "Units",
                LimitationType = "Test"
            };

            _mockLimitationRepository
                .Setup(x => x.GetByIdAsync(request.LimitationId))
                .ReturnsAsync(existingLimitation);

            _mockLimitationRepository
                .Setup(x => x.UpdateAsync(It.IsAny<Limitation>()))
                .Returns(Task.CompletedTask);

            _mockLimitationRepository
                .Setup(x => x.SaveChangesAsync())
                .Returns(Task.CompletedTask);

            // Act
            var result = await _limitationService.UpdateLimitationAsync(request);

            // Assert
            Assert.True(result.Success);
            Assert.NotNull(result.Data);
            Assert.Equal(0, result.Data.LimitValue);
            Assert.False(result.Data.IsUnlimited);
            Assert.Equal(0, existingLimitation.LimitValue);

            _mockLimitationRepository.Verify(x => x.UpdateAsync(existingLimitation), Times.Once);
            _mockLimitationRepository.Verify(x => x.SaveChangesAsync(), Times.Once);
        }
        #endregion

        #region TC_UpdateLim_10 - Empty Guid
        [Fact]
        public async Task UpdateLimitationAsync_EmptyGuid_ReturnsNotFound()
        {
            // Arrange
            var request = new UpdateLimitationRequest
            {
                LimitationId = Guid.Empty,
                Name = "Test",
                Description = "Test",
                IsUnlimited = false,
                LimitValue = 10,
                LimitUnit = "Units",
                LimitationType = "Test"
            };

            _mockLimitationRepository
                .Setup(x => x.GetByIdAsync(Guid.Empty))
                .ReturnsAsync((Limitation)null);

            // Act
            var result = await _limitationService.UpdateLimitationAsync(request);

            // Assert
            Assert.False(result.Success);
            Assert.Equal("Limitation not found", result.Message);
            Assert.Null(result.Data);

            _mockLimitationRepository.Verify(x => x.GetByIdAsync(Guid.Empty), Times.Once);
            _mockLimitationRepository.Verify(x => x.UpdateAsync(It.IsAny<Limitation>()), Times.Never);
        }
        #endregion
    }
}
