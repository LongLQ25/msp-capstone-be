using Microsoft.AspNetCore.Identity;
using Moq;
using MSP.Application.Abstracts;
using MSP.Application.Repositories;
using MSP.Application.Services.Implementations.SubscriptionService;
using MSP.Application.Services.Interfaces.Payment;
using MSP.Application.Services.Interfaces.Subscription;
using MSP.Domain.Entities;
using Xunit;

namespace MSP.Tests.Services.SubscriptionServicesTest
{
    public class GetTransactionHistoryTest
    {
        private readonly Mock<IPaymentService> _mockPaymentService;
        private readonly Mock<IPackageRepository> _mockPackageRepository;
        private readonly Mock<ISubscriptionRepository> _mockSubscriptionRepository;
        private readonly Mock<UserManager<User>> _mockUserManager;
        private readonly Mock<IProjectRepository> _mockProjectRepository;
        private readonly Mock<IUserRepository> _mockUserRepository;
        private readonly Mock<IMeetingRepository> _mockMeetingRepository;
        private readonly Mock<IOrganizationInviteRepository> _mockOrganizationInviteRepository;

        private readonly ISubscriptionService _subscriptionService;

        public GetTransactionHistoryTest()
        {
            _mockPaymentService = new Mock<IPaymentService>();
            _mockPackageRepository = new Mock<IPackageRepository>();
            _mockSubscriptionRepository = new Mock<ISubscriptionRepository>();
            _mockUserManager = new Mock<UserManager<User>>(
                new Mock<IUserStore<User>>().Object,
                null, null, null, null, null, null, null, null
            );
            _mockProjectRepository = new Mock<IProjectRepository>();
            _mockUserRepository = new Mock<IUserRepository>();
            _mockMeetingRepository = new Mock<IMeetingRepository>();
            _mockOrganizationInviteRepository = new Mock<IOrganizationInviteRepository>();

            _subscriptionService = new SubscriptionService(
                _mockPaymentService.Object,
                _mockPackageRepository.Object,
                _mockSubscriptionRepository.Object,
                _mockUserManager.Object,
                _mockProjectRepository.Object,
                _mockUserRepository.Object,
                _mockMeetingRepository.Object,
                _mockOrganizationInviteRepository.Object
            );
        }

        #region GetSubscriptionsByUserIdAsync Tests

        [Fact]
        public async Task GetSubscriptionsByUserIdAsync_WithValidUserId_ReturnsSuccessResponse()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var subscriptionId = Guid.NewGuid();
            var packageId = Guid.NewGuid();

            var user = new User
            {
                Id = userId,
                Email = "test@example.com",
                FullName = "Test User",
                CreatedAt = DateTime.UtcNow
            };

            var limitation = new Limitation
            {
                Id = Guid.NewGuid(),
                Name = "Projects",
                Description = "Number of projects",
                LimitationType = "NumberProject",
                IsUnlimited = false,
                LimitValue = 5,
                LimitUnit = "projects",
                IsDeleted = false
            };

            var package = new Package
            {
                Id = packageId,
                Name = "Premium",
                Description = "Premium package",
                Price = 100000,
                BillingCycle = 1,
                Currency = "VND",
                Limitations = new List<Limitation> { limitation }
            };

            var subscription = new Subscription
            {
                Id = subscriptionId,
                UserId = userId,
                PackageId = packageId,
                TotalPrice = 100000,
                IsActive = true,
                Status = "ACTIVE",
                PaymentMethod = "PayOS",
                TransactionID = "TXN123456",
                StartDate = DateTime.UtcNow.AddDays(-30),
                EndDate = DateTime.UtcNow.AddDays(30),
                PaidAt = DateTime.UtcNow.AddDays(-30),
                User = user,
                Package = package
            };

            var subscriptions = new List<Subscription> { subscription };

            _mockUserManager
                .Setup(x => x.FindByIdAsync(userId.ToString()))
                .ReturnsAsync(user);

            _mockSubscriptionRepository
                .Setup(x => x.GetByUserIdAsync(userId))
                .ReturnsAsync(subscriptions);

            // Act
            var result = await _subscriptionService.GetSubscriptionsByUserIdAsync(userId);

            // Assert
            Assert.True(result.Success);
            Assert.Equal("Get subscriptions successfully", result.Message);
            Assert.NotNull(result.Data);
            Assert.Single(result.Data);

            var firstSubscription = result.Data.First();
            Assert.Equal(subscriptionId, firstSubscription.Id);
            Assert.Equal(userId, firstSubscription.UserId);
            Assert.Equal(packageId, firstSubscription.PackageId);
            Assert.Equal(100000, firstSubscription.TotalPrice);
            Assert.True(firstSubscription.IsActive);
            Assert.Equal("ACTIVE", firstSubscription.Status);
            Assert.Equal("TXN123456", firstSubscription.TransactionID);
            Assert.Equal("PayOS", firstSubscription.PaymentMethod);

            // Verify Package mapping
            Assert.NotNull(firstSubscription.Package);
            Assert.Equal(packageId, firstSubscription.Package.Id);
            Assert.Equal("Premium", firstSubscription.Package.Name);
            Assert.Equal(100000, firstSubscription.Package.Price);

            // Verify User mapping
            Assert.NotNull(firstSubscription.User);
            Assert.Equal(userId, firstSubscription.User.Id);
            Assert.Equal("Test User", firstSubscription.User.FullName);

            // Verify Limitations
            Assert.Single(firstSubscription.Package.Limitations);
            Assert.Equal("Projects", firstSubscription.Package.Limitations.First().Name);

            _mockUserManager.Verify(x => x.FindByIdAsync(userId.ToString()), Times.Once);
            _mockSubscriptionRepository.Verify(x => x.GetByUserIdAsync(userId), Times.Once);
        }

        [Fact]
        public async Task GetSubscriptionsByUserIdAsync_WithUserNotFound_ReturnsErrorResponse()
        {
            // Arrange
            var userId = Guid.NewGuid();

            _mockUserManager
                .Setup(x => x.FindByIdAsync(userId.ToString()))
                .ReturnsAsync((User)null);

            // Act
            var result = await _subscriptionService.GetSubscriptionsByUserIdAsync(userId);

            // Assert
            Assert.False(result.Success);
            Assert.Equal("User not found", result.Message);
            Assert.Null(result.Data);

            _mockUserManager.Verify(x => x.FindByIdAsync(userId.ToString()), Times.Once);
            _mockSubscriptionRepository.Verify(x => x.GetByUserIdAsync(It.IsAny<Guid>()), Times.Never);
        }

        [Fact]
        public async Task GetSubscriptionsByUserIdAsync_WithNoSubscriptions_ReturnsEmptyList()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var user = new User
            {
                Id = userId,
                Email = "test@example.com",
                FullName = "Test User"
            };

            var emptySubscriptions = new List<Subscription>();

            _mockUserManager
                .Setup(x => x.FindByIdAsync(userId.ToString()))
                .ReturnsAsync(user);

            _mockSubscriptionRepository
                .Setup(x => x.GetByUserIdAsync(userId))
                .ReturnsAsync(emptySubscriptions);

            // Act
            var result = await _subscriptionService.GetSubscriptionsByUserIdAsync(userId);

            // Assert
            Assert.True(result.Success);
            Assert.Equal("Get subscriptions successfully", result.Message);
            Assert.NotNull(result.Data);
            Assert.Empty(result.Data);

            _mockUserManager.Verify(x => x.FindByIdAsync(userId.ToString()), Times.Once);
            _mockSubscriptionRepository.Verify(x => x.GetByUserIdAsync(userId), Times.Once);
        }

        [Fact]
        public async Task GetSubscriptionsByUserIdAsync_WithMultipleSubscriptions_ReturnsAllSubscriptions()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var packageId1 = Guid.NewGuid();
            var packageId2 = Guid.NewGuid();

            var user = new User
            {
                Id = userId,
                Email = "test@example.com",
                FullName = "Test User",
                CreatedAt = DateTime.UtcNow
            };

            var package1 = new Package
            {
                Id = packageId1,
                Name = "MSP BASIC",
                Description = "Standard package",
                Price = 50000,
                BillingCycle = 1,
                Currency = "VND",
                Limitations = new List<Limitation>()
            };

            var package2 = new Package
            {
                Id = packageId2,
                Name = "MSP PRO",
                Description = "Premium package",
                Price = 100000,
                BillingCycle = 1,
                Currency = "VND",
                Limitations = new List<Limitation>()
            };

            var subscription1 = new Subscription
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                PackageId = packageId1,
                TotalPrice = 50000,
                IsActive = false,
                Status = "EXPIRED",
                PaymentMethod = "PayOS",
                TransactionID = "TXN111111",
                StartDate = DateTime.UtcNow.AddDays(-90),
                EndDate = DateTime.UtcNow.AddDays(-30),
                PaidAt = DateTime.UtcNow.AddDays(-90),
                User = user,
                Package = package1
            };

            var subscription2 = new Subscription
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                PackageId = packageId2,
                TotalPrice = 100000,
                IsActive = true,
                Status = "ACTIVE",
                PaymentMethod = "PayOS",
                TransactionID = "TXN222222",
                StartDate = DateTime.UtcNow.AddDays(-30),
                EndDate = DateTime.UtcNow.AddDays(30),
                PaidAt = DateTime.UtcNow.AddDays(-30),
                User = user,
                Package = package2
            };

            var subscriptions = new List<Subscription> { subscription1, subscription2 };

            _mockUserManager
                .Setup(x => x.FindByIdAsync(userId.ToString()))
                .ReturnsAsync(user);

            _mockSubscriptionRepository
                .Setup(x => x.GetByUserIdAsync(userId))
                .ReturnsAsync(subscriptions);

            // Act
            var result = await _subscriptionService.GetSubscriptionsByUserIdAsync(userId);

            // Assert
            Assert.True(result.Success);
            Assert.NotNull(result.Data);
            Assert.Equal(2, result.Data.Count());

            var resultList = result.Data.ToList();

            // First subscription (expired)
            Assert.Equal("TXN111111", resultList[0].TransactionID);
            Assert.False(resultList[0].IsActive);
            Assert.Equal("EXPIRED", resultList[0].Status);
            Assert.Equal("MSP BASIC", resultList[0].Package.Name);
            Assert.Equal(50000, resultList[0].TotalPrice);

            // Second subscription (active)
            Assert.Equal("TXN222222", resultList[1].TransactionID);
            Assert.True(resultList[1].IsActive);
            Assert.Equal("ACTIVE", resultList[1].Status);
            Assert.Equal("MSP PRO", resultList[1].Package.Name);
            Assert.Equal(100000, resultList[1].TotalPrice);

            _mockUserManager.Verify(x => x.FindByIdAsync(userId.ToString()), Times.Once);
            _mockSubscriptionRepository.Verify(x => x.GetByUserIdAsync(userId), Times.Once);
        }

        [Fact]
        public async Task GetSubscriptionsByUserIdAsync_VerifiesAllSubscriptionProperties()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var subscriptionId = Guid.NewGuid();
            var packageId = Guid.NewGuid();
            var startDate = DateTime.UtcNow.AddDays(-30);
            var endDate = DateTime.UtcNow.AddDays(30);
            var paidAt = DateTime.UtcNow.AddDays(-30);

            var user = new User
            {
                Id = userId,
                Email = "user@example.com",
                FullName = "Full Name",
                CreatedAt = DateTime.UtcNow.AddDays(-100)
            };

            var limitation1 = new Limitation
            {
                Id = Guid.NewGuid(),
                Name = "Limit 1",
                Description = "Desc 1",
                LimitationType = "NumberProject",
                IsUnlimited = false,
                LimitValue = 10,
                LimitUnit = "projects",
                IsDeleted = false
            };

            var limitation2 = new Limitation
            {
                Id = Guid.NewGuid(),
                Name = "Limit 2",
                Description = "Desc 2",
                LimitationType = "NumberMeeting",
                IsUnlimited = true,
                LimitValue = null,
                LimitUnit = "meetings",
                IsDeleted = false
            };

            var package = new Package
            {
                Id = packageId,
                Name = "Test Package",
                Description = "Test Description",
                Price = 200000,
                BillingCycle = 30,
                Currency = "USD",
                Limitations = new List<Limitation> { limitation1, limitation2 }
            };

            var subscription = new Subscription
            {
                Id = subscriptionId,
                UserId = userId,
                PackageId = packageId,
                TotalPrice = 200000,
                IsActive = true,
                Status = "PAID",
                PaymentMethod = "CreditCard",
                TransactionID = "TXN999999",
                StartDate = startDate,
                EndDate = endDate,
                PaidAt = paidAt,
                User = user,
                Package = package
            };

            _mockUserManager
                .Setup(x => x.FindByIdAsync(userId.ToString()))
                .ReturnsAsync(user);

            _mockSubscriptionRepository
                .Setup(x => x.GetByUserIdAsync(userId))
                .ReturnsAsync(new List<Subscription> { subscription });

            // Act
            var result = await _subscriptionService.GetSubscriptionsByUserIdAsync(userId);

            // Assert
            Assert.True(result.Success);
            var sub = result.Data.First();

            // Verify Subscription properties
            Assert.Equal(subscriptionId, sub.Id);
            Assert.Equal(userId, sub.UserId);
            Assert.Equal(packageId, sub.PackageId);
            Assert.Equal(200000, sub.TotalPrice);
            Assert.True(sub.IsActive);
            Assert.Equal("PAID", sub.Status);
            Assert.Equal("CreditCard", sub.PaymentMethod);
            Assert.Equal("TXN999999", sub.TransactionID);
            Assert.Equal(startDate, sub.StartDate);
            Assert.Equal(endDate, sub.EndDate);
            Assert.Equal(paidAt, sub.PaidAt);

            // Verify User
            Assert.Equal("Full Name", sub.User.FullName);
            Assert.Equal("user@example.com", sub.User.Email);

            // Verify Package
            Assert.Equal("Test Package", sub.Package.Name);
            Assert.Equal("Test Description", sub.Package.Description);
            Assert.Equal(200000, sub.Package.Price);
            Assert.Equal(30, sub.Package.BillingCycle);
            Assert.Equal("USD", sub.Package.Currency);

            // Verify Limitations
            Assert.Equal(2, sub.Package.Limitations.Count);
            var lim1 = sub.Package.Limitations.First(l => l.Name == "Limit 1");
            Assert.False(lim1.IsUnlimited);
            Assert.Equal(10, lim1.LimitValue);
            Assert.Equal("projects", lim1.LimitUnit);

            var lim2 = sub.Package.Limitations.First(l => l.Name == "Limit 2");
            Assert.True(lim2.IsUnlimited);
            Assert.Null(lim2.LimitValue);
        }

        [Fact]
        public async Task GetSubscriptionsByUserIdAsync_WithSubscriptionWithoutLimitations_ReturnsSuccessfully()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var user = new User
            {
                Id = userId,
                Email = "test@example.com",
                FullName = "Test User",
                CreatedAt = DateTime.UtcNow
            };

            var package = new Package
            {
                Id = Guid.NewGuid(),
                Name = "Free Package",
                Description = "No limitations",
                Price = 0,
                BillingCycle = 30,
                Currency = "VND",
                Limitations = new List<Limitation>() // Empty limitations
            };

            var subscription = new Subscription
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                PackageId = package.Id,
                TotalPrice = 0,
                IsActive = true,
                Status = "ACTIVE",
                PaymentMethod = "Free",
                TransactionID = "FREE001",
                User = user,
                Package = package
            };

            _mockUserManager
                .Setup(x => x.FindByIdAsync(userId.ToString()))
                .ReturnsAsync(user);

            _mockSubscriptionRepository
                .Setup(x => x.GetByUserIdAsync(userId))
                .ReturnsAsync(new List<Subscription> { subscription });

            // Act
            var result = await _subscriptionService.GetSubscriptionsByUserIdAsync(userId);

            // Assert
            Assert.True(result.Success);
            var sub = result.Data.First();
            Assert.Empty(sub.Package.Limitations);
            Assert.Equal("Free Package", sub.Package.Name);
            Assert.Equal(0, sub.TotalPrice);
        }

        #endregion
    }
}
