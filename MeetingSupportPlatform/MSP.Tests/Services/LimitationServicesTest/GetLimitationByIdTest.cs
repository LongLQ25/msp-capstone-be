using Moq;
using MSP.Application.Repositories;
using MSP.Application.Services.Implementations.Limitation;
using MSP.Application.Services.Interfaces.Limitation;
using MSP.Domain.Entities;
using Xunit;

namespace MSP.Tests.Services.LimitationServicesTest
{
    public class GetLimitationByIdTest
    {
        private readonly Mock<ILimitationRepository> _mockLimitationRepository;
        private readonly ILimitationService _limitationService;

        public GetLimitationByIdTest()
        {
            _mockLimitationRepository = new Mock<ILimitationRepository>();
            _limitationService = new LimitationService(_mockLimitationRepository.Object);
        }

        #region TC_GetLimById_01 - Limitation not found
        [Fact]
        public async Task GetLimitationByIdAsync_LimitationNotFound_ReturnsError()
        {
            // Arrange
            var limitationId = Guid.NewGuid();

            _mockLimitationRepository
                .Setup(x => x.GetLimitationByIdAsync(limitationId))
                .ReturnsAsync((Limitation)null);

            // Act
            var result = await _limitationService.GetLimitationByIdAsync(limitationId);

            // Assert
            Assert.False(result.Success);
            Assert.Equal("Limitation not found", result.Message);
            Assert.Null(result.Data);

            _mockLimitationRepository.Verify(x => x.GetLimitationByIdAsync(limitationId), Times.Once);
        }
        #endregion

        #region TC_GetLimById_02 - Limitation is deleted
        [Fact]
        public async Task GetLimitationByIdAsync_LimitationIsDeleted_ReturnsError()
        {
            // Arrange
            var limitationId = Guid.NewGuid();
            var deletedLimitation = new Limitation
            {
                Id = limitationId,
                Name = "Deleted Limitation",
                Description = "This is deleted",
                IsDeleted = true,
                LimitValue = 100
            };

            _mockLimitationRepository
                .Setup(x => x.GetLimitationByIdAsync(limitationId))
                .ReturnsAsync(deletedLimitation);

            // Act
            var result = await _limitationService.GetLimitationByIdAsync(limitationId);

            // Assert
            Assert.False(result.Success);
            Assert.Equal("Limitation not found", result.Message);
            Assert.Null(result.Data);

            _mockLimitationRepository.Verify(x => x.GetLimitationByIdAsync(limitationId), Times.Once);
        }
        #endregion

        #region TC_GetLimById_03 - Success with complete data
        [Fact]
        public async Task GetLimitationByIdAsync_ValidLimitation_ReturnsSuccess()
        {
            // Arrange
            var limitationId = Guid.NewGuid();
            var limitation = new Limitation
            {
                Id = limitationId,
                Name = "Project Limit",
                Description = "Maximum number of projects",
                IsUnlimited = false,
                LimitValue = 10,
                LimitUnit = "Projects",
                LimitationType = "Project",
                IsDeleted = false
            };

            _mockLimitationRepository
                .Setup(x => x.GetLimitationByIdAsync(limitationId))
                .ReturnsAsync(limitation);

            // Act
            var result = await _limitationService.GetLimitationByIdAsync(limitationId);

            // Assert
            Assert.True(result.Success);
            Assert.NotNull(result.Data);
            Assert.Equal(limitationId, result.Data.Id);
            Assert.Equal("Project Limit", result.Data.Name);
            Assert.Equal("Maximum number of projects", result.Data.Description);
            Assert.False(result.Data.IsUnlimited);
            Assert.Equal(10, result.Data.LimitValue);
            Assert.Equal("Projects", result.Data.LimitUnit);
            Assert.Equal("Project", result.Data.LimitationType);
            Assert.False(result.Data.IsDeleted);

            _mockLimitationRepository.Verify(x => x.GetLimitationByIdAsync(limitationId), Times.Once);
        }
        #endregion

        #region TC_GetLimById_04 - Success with unlimited limitation
        [Fact]
        public async Task GetLimitationByIdAsync_UnlimitedLimitation_ReturnsSuccessWithNullValue()
        {
            // Arrange
            var limitationId = Guid.NewGuid();
            var limitation = new Limitation
            {
                Id = limitationId,
                Name = "Storage Limit",
                Description = "Unlimited storage",
                IsUnlimited = true,
                LimitValue = null,
                LimitUnit = "GB",
                LimitationType = "Storage",
                IsDeleted = false
            };

            _mockLimitationRepository
                .Setup(x => x.GetLimitationByIdAsync(limitationId))
                .ReturnsAsync(limitation);

            // Act
            var result = await _limitationService.GetLimitationByIdAsync(limitationId);

            // Assert
            Assert.True(result.Success);
            Assert.NotNull(result.Data);
            Assert.Equal("Storage Limit", result.Data.Name);
            Assert.True(result.Data.IsUnlimited);
            Assert.Null(result.Data.LimitValue);
            Assert.Equal("GB", result.Data.LimitUnit);

            _mockLimitationRepository.Verify(x => x.GetLimitationByIdAsync(limitationId), Times.Once);
        }
        #endregion

        #region TC_GetLimById_05 - Empty Guid
        [Fact]
        public async Task GetLimitationByIdAsync_EmptyGuid_ReturnsNotFound()
        {
            // Arrange
            var limitationId = Guid.Empty;

            _mockLimitationRepository
                .Setup(x => x.GetLimitationByIdAsync(limitationId))
                .ReturnsAsync((Limitation)null);

            // Act
            var result = await _limitationService.GetLimitationByIdAsync(limitationId);

            // Assert
            Assert.False(result.Success);
            Assert.Equal("Limitation not found", result.Message);
            Assert.Null(result.Data);

            _mockLimitationRepository.Verify(x => x.GetLimitationByIdAsync(limitationId), Times.Once);
        }
        #endregion
    }
}
