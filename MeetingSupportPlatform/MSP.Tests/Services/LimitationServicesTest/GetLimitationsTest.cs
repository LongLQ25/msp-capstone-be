using Moq;
using MSP.Application.Repositories;
using MSP.Application.Services.Implementations.Limitation;
using MSP.Application.Services.Interfaces.Limitation;
using MSP.Domain.Entities;
using Xunit;

namespace MSP.Tests.Services.LimitationServicesTest
{
    public class GetLimitationsTest
    {
        private readonly Mock<ILimitationRepository> _mockLimitationRepository;
        private readonly ILimitationService _limitationService;

        public GetLimitationsTest()
        {
            _mockLimitationRepository = new Mock<ILimitationRepository>();
            _limitationService = new LimitationService(_mockLimitationRepository.Object);
        }

        #region TC_GetLimitations_01 - Success with multiple limitations
        [Fact]
        public async Task GetLimitationsAsync_HasLimitations_ReturnsSuccessWithList()
        {
            // Arrange
            var limitations = new List<Limitation>
            {
                new Limitation
                {
                    Id = Guid.NewGuid(),
                    Name = "Project Limit",
                    Description = "Maximum number of projects",
                    IsUnlimited = false,
                    LimitValue = 10,
                    LimitUnit = "Projects",
                    LimitationType = "Project",
                    IsDeleted = false
                },
                new Limitation
                {
                    Id = Guid.NewGuid(),
                    Name = "User Limit",
                    Description = "Maximum number of users",
                    IsUnlimited = false,
                    LimitValue = 50,
                    LimitUnit = "Users",
                    LimitationType = "User",
                    IsDeleted = false
                },
                new Limitation
                {
                    Id = Guid.NewGuid(),
                    Name = "Storage Limit",
                    Description = "Maximum storage space",
                    IsUnlimited = true,
                    LimitValue = null,
                    LimitUnit = "GB",
                    LimitationType = "Storage",
                    IsDeleted = false
                }
            };

            _mockLimitationRepository
                .Setup(x => x.GetAll())
                .ReturnsAsync(limitations);

            // Act
            var result = await _limitationService.GetLimitationsAsync();

            // Assert
            Assert.True(result.Success);
            Assert.NotNull(result.Data);
            Assert.Equal(3, result.Data.Count);

            // Verify first limitation
            var limitation1 = result.Data[0];
            Assert.Equal("Project Limit", limitation1.Name);
            Assert.Equal("Maximum number of projects", limitation1.Description);
            Assert.False(limitation1.IsUnlimited);
            Assert.Equal(10, limitation1.LimitValue);
            Assert.Equal("Projects", limitation1.LimitUnit);
            Assert.Equal("Project", limitation1.LimitationType);
            Assert.False(limitation1.IsDeleted);

            // Verify second limitation
            var limitation2 = result.Data[1];
            Assert.Equal("User Limit", limitation2.Name);
            Assert.Equal(50, limitation2.LimitValue);

            // Verify third limitation (unlimited)
            var limitation3 = result.Data[2];
            Assert.Equal("Storage Limit", limitation3.Name);
            Assert.True(limitation3.IsUnlimited);
            Assert.Null(limitation3.LimitValue);

            _mockLimitationRepository.Verify(x => x.GetAll(), Times.Once);
        }
        #endregion

        #region TC_GetLimitations_02 - Success with empty list
        [Fact]
        public async Task GetLimitationsAsync_NoLimitations_ReturnsSuccessWithEmptyList()
        {
            // Arrange
            var emptyList = new List<Limitation>();

            _mockLimitationRepository
                .Setup(x => x.GetAll())
                .ReturnsAsync(emptyList);

            // Act
            var result = await _limitationService.GetLimitationsAsync();

            // Assert
            Assert.True(result.Success);
            Assert.NotNull(result.Data);
            Assert.Empty(result.Data);

            _mockLimitationRepository.Verify(x => x.GetAll(), Times.Once);
        }
        #endregion

        #region TC_GetLimitations_03 - Success with single limitation
        [Fact]
        public async Task GetLimitationsAsync_SingleLimitation_ReturnsSuccessWithOneItem()
        {
            // Arrange
            var limitations = new List<Limitation>
            {
                new Limitation
                {
                    Id = Guid.NewGuid(),
                    Name = "API Calls",
                    Description = "Maximum API calls per day",
                    IsUnlimited = false,
                    LimitValue = 1000,
                    LimitUnit = "Calls",
                    LimitationType = "API",
                    IsDeleted = false
                }
            };

            _mockLimitationRepository
                .Setup(x => x.GetAll())
                .ReturnsAsync(limitations);

            // Act
            var result = await _limitationService.GetLimitationsAsync();

            // Assert
            Assert.True(result.Success);
            Assert.NotNull(result.Data);
            Assert.Single(result.Data);

            var limitation = result.Data[0];
            Assert.Equal("API Calls", limitation.Name);
            Assert.Equal(1000, limitation.LimitValue);
            Assert.False(limitation.IsUnlimited);

            _mockLimitationRepository.Verify(x => x.GetAll(), Times.Once);
        }
        #endregion

        #region TC_GetLimitations_04 - Large list performance test
        [Fact]
        public async Task GetLimitationsAsync_LargeList_HandlesCorrectly()
        {
            // Arrange
            var largeList = new List<Limitation>();
            for (int i = 0; i < 100; i++)
            {
                largeList.Add(new Limitation
                {
                    Id = Guid.NewGuid(),
                    Name = $"Limitation {i}",
                    Description = $"Description {i}",
                    IsUnlimited = i % 2 == 0,
                    LimitValue = i % 2 == 0 ? null : i * 10,
                    LimitUnit = "Units",
                    LimitationType = $"Type{i % 5}",
                    IsDeleted = false
                });
            }

            _mockLimitationRepository
                .Setup(x => x.GetAll())
                .ReturnsAsync(largeList);

            // Act
            var result = await _limitationService.GetLimitationsAsync();

            // Assert
            Assert.True(result.Success);
            Assert.Equal(100, result.Data.Count);

            _mockLimitationRepository.Verify(x => x.GetAll(), Times.Once);
        }
        #endregion

        #region TC_GetLimitations_05 - Limitations with deleted and active records
        [Fact]
        public async Task GetLimitationsAsync_MixedDeletedStatus_ReturnsAll()
        {
            // Arrange
            var limitations = new List<Limitation>
            {
                new Limitation
                {
                    Id = Guid.NewGuid(),
                    Name = "Active Limitation",
                    Description = "Active record",
                    IsDeleted = false,
                    LimitValue = 10
                },
                new Limitation
                {
                    Id = Guid.NewGuid(),
                    Name = "Deleted Limitation",
                    Description = "Deleted record",
                    IsDeleted = true,
                    LimitValue = 20
                }
            };

            _mockLimitationRepository
                .Setup(x => x.GetAll())
                .ReturnsAsync(limitations);

            // Act
            var result = await _limitationService.GetLimitationsAsync();

            // Assert
            Assert.True(result.Success);
            Assert.Equal(2, result.Data.Count);

            // Verify both active and deleted are returned
            Assert.Contains(result.Data, l => l.Name == "Active Limitation" && !l.IsDeleted);
            Assert.Contains(result.Data, l => l.Name == "Deleted Limitation" && l.IsDeleted);

            _mockLimitationRepository.Verify(x => x.GetAll(), Times.Once);
        }
        #endregion
    }
}
