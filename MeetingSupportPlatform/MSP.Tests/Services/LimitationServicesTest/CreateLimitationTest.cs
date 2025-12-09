using Moq;
using MSP.Application.Models.Requests.Limitation;
using MSP.Application.Repositories;
using MSP.Application.Services.Implementations.Limitation;
using MSP.Domain.Entities;
using Xunit;

namespace MSP.Tests.Services.LimitationServicesTest
{
    public class CreateLimitationTest
    {
        private readonly Mock<ILimitationRepository> _mockLimitationRepository;
        private readonly LimitationService _limitationService;

        public CreateLimitationTest()
        {
            _mockLimitationRepository = new Mock<ILimitationRepository>();
            _limitationService = new LimitationService(_mockLimitationRepository.Object);
        }

        #region TC_CreateLim_01 - Success with complete data
        [Fact]
        public async Task CreateLimitationAsync_ValidRequest_CreatesSuccessfully()
        {
            // Arrange
            var request = new CreateLimitationRequest
            {
                Name = "Project Limit",
                Description = "Maximum number of projects",
                IsUnlimited = false,
                LimitValue = 10,
                LimitUnit = "Projects",
                LimitationType = "Project"
            };

            Limitation capturedLimitation = null;

            _mockLimitationRepository
                .Setup(x => x.AddAsync(It.IsAny<Limitation>()))
                .Callback<Limitation>(l =>
                {
                    // ✅ Set Id when AddAsync is called (simulate EF behavior)
                    l.Id = Guid.NewGuid();
                    capturedLimitation = l;
                })
                .ReturnsAsync((Limitation l) => l);

            _mockLimitationRepository
                .Setup(x => x.SaveChangesAsync())
                .Returns(Task.CompletedTask);

            // Act
            var result = await _limitationService.CreateLimitationAsync(request);

            // Assert
            Assert.True(result.Success);
            Assert.Equal("Limitation created successfully", result.Message);
            Assert.NotNull(result.Data);
            Assert.Equal("Project Limit", result.Data.Name);
            Assert.Equal("Maximum number of projects", result.Data.Description);
            Assert.False(result.Data.IsUnlimited);
            Assert.Equal(10, result.Data.LimitValue);
            Assert.Equal("Projects", result.Data.LimitUnit);

            // Verify captured limitation
            Assert.NotNull(capturedLimitation);
            Assert.Equal("Project Limit", capturedLimitation.Name);
            Assert.NotEqual(Guid.Empty, capturedLimitation.Id); // ✅ Now passes
            Assert.True((DateTime.UtcNow - capturedLimitation.CreatedAt).TotalSeconds < 2);
            Assert.True((DateTime.UtcNow - capturedLimitation.UpdatedAt).TotalSeconds < 2);

            _mockLimitationRepository.Verify(x => x.AddAsync(It.IsAny<Limitation>()), Times.Once);
            _mockLimitationRepository.Verify(x => x.SaveChangesAsync(), Times.Once);
        }
        #endregion

        #region TC_CreateLim_02 - Success with unlimited limitation
        [Fact]
        public async Task CreateLimitationAsync_UnlimitedRequest_CreatesSuccessfully()
        {
            // Arrange
            var request = new CreateLimitationRequest
            {
                Name = "Storage Limit",
                Description = "Unlimited storage",
                IsUnlimited = true,
                LimitValue = 0, // Ignored when IsUnlimited = true
                LimitUnit = "GB",
                LimitationType = "Storage"
            };

            _mockLimitationRepository
                .Setup(x => x.AddAsync(It.IsAny<Limitation>()))
                .ReturnsAsync((Limitation l) => l);

            _mockLimitationRepository
                .Setup(x => x.SaveChangesAsync())
                .Returns(Task.CompletedTask);

            // Act
            var result = await _limitationService.CreateLimitationAsync(request);

            // Assert
            Assert.True(result.Success);
            Assert.NotNull(result.Data);
            Assert.Equal("Storage Limit", result.Data.Name);
            Assert.True(result.Data.IsUnlimited);
            Assert.Equal(0, result.Data.LimitValue); // Value still stored

            _mockLimitationRepository.Verify(x => x.AddAsync(It.IsAny<Limitation>()), Times.Once);
            _mockLimitationRepository.Verify(x => x.SaveChangesAsync(), Times.Once);
        }
        #endregion

        #region TC_CreateLim_03 - Success with null description
        [Fact]
        public async Task CreateLimitationAsync_NullDescription_CreatesSuccessfully()
        {
            // Arrange
            var request = new CreateLimitationRequest
            {
                Name = "API Limit",
                Description = null,
                IsUnlimited = false,
                LimitValue = 1000,
                LimitUnit = "Calls",
                LimitationType = "API"
            };

            _mockLimitationRepository
                .Setup(x => x.AddAsync(It.IsAny<Limitation>()))
                .ReturnsAsync((Limitation l) => l);

            _mockLimitationRepository
                .Setup(x => x.SaveChangesAsync())
                .Returns(Task.CompletedTask);

            // Act
            var result = await _limitationService.CreateLimitationAsync(request);

            // Assert
            Assert.True(result.Success);
            Assert.NotNull(result.Data);
            Assert.Equal("API Limit", result.Data.Name);
            Assert.Null(result.Data.Description);
            Assert.Equal(1000, result.Data.LimitValue);

            _mockLimitationRepository.Verify(x => x.AddAsync(It.IsAny<Limitation>()), Times.Once);
            _mockLimitationRepository.Verify(x => x.SaveChangesAsync(), Times.Once);
        }
        #endregion

        #region TC_CreateLim_04 - Success with zero limit value
        [Fact]
        public async Task CreateLimitationAsync_ZeroLimitValue_CreatesSuccessfully()
        {
            // Arrange
            var request = new CreateLimitationRequest
            {
                Name = "Zero Limit",
                Description = "Limit with zero value",
                IsUnlimited = false,
                LimitValue = 0,
                LimitUnit = "Count",
                LimitationType = "Test"
            };

            _mockLimitationRepository
                .Setup(x => x.AddAsync(It.IsAny<Limitation>()))
                .ReturnsAsync((Limitation l) => l);

            _mockLimitationRepository
                .Setup(x => x.SaveChangesAsync())
                .Returns(Task.CompletedTask);

            // Act
            var result = await _limitationService.CreateLimitationAsync(request);

            // Assert
            Assert.True(result.Success);
            Assert.NotNull(result.Data);
            Assert.Equal(0, result.Data.LimitValue);
            Assert.False(result.Data.IsUnlimited);

            _mockLimitationRepository.Verify(x => x.AddAsync(It.IsAny<Limitation>()), Times.Once);
            _mockLimitationRepository.Verify(x => x.SaveChangesAsync(), Times.Once);
        }
        #endregion

        #region TC_CreateLim_05 - AddAsync throws exception
        [Fact]
        public async Task CreateLimitationAsync_AddAsyncFails_ThrowsException()
        {
            // Arrange
            var request = new CreateLimitationRequest
            {
                Name = "Test Limit",
                Description = "Test",
                IsUnlimited = false,
                LimitValue = 10,
                LimitUnit = "Units",
                LimitationType = "Test"
            };

            _mockLimitationRepository
                .Setup(x => x.AddAsync(It.IsAny<Limitation>()))
                .ThrowsAsync(new Exception("Database error"));

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(
                async () => await _limitationService.CreateLimitationAsync(request));

            _mockLimitationRepository.Verify(x => x.AddAsync(It.IsAny<Limitation>()), Times.Once);
            _mockLimitationRepository.Verify(x => x.SaveChangesAsync(), Times.Never);
        }
        #endregion

        #region TC_CreateLim_06 - SaveChangesAsync throws exception
        [Fact]
        public async Task CreateLimitationAsync_SaveChangesFails_ThrowsException()
        {
            // Arrange
            var request = new CreateLimitationRequest
            {
                Name = "Test Limit",
                Description = "Test",
                IsUnlimited = false,
                LimitValue = 10,
                LimitUnit = "Units",
                LimitationType = "Test"
            };

            _mockLimitationRepository
                .Setup(x => x.AddAsync(It.IsAny<Limitation>()))
                .ReturnsAsync((Limitation l) => l);

            _mockLimitationRepository
                .Setup(x => x.SaveChangesAsync())
                .ThrowsAsync(new Exception("Failed to save changes"));

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(
                async () => await _limitationService.CreateLimitationAsync(request));

            _mockLimitationRepository.Verify(x => x.AddAsync(It.IsAny<Limitation>()), Times.Once);
            _mockLimitationRepository.Verify(x => x.SaveChangesAsync(), Times.Once);
        }
        #endregion

    }
}
