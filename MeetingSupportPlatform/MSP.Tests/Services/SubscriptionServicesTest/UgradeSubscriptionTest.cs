using Microsoft.AspNetCore.Identity;
using Moq;
using MSP.Application.Abstracts;
using MSP.Application.Models.Requests.Payment;
using MSP.Application.Models.Requests.Subscription;
using MSP.Application.Models.Responses.Payment;
using MSP.Application.Repositories;
using MSP.Application.Services.Implementations.SubscriptionService;
using MSP.Application.Services.Interfaces.Payment;
using MSP.Application.Services.Interfaces.Subscription;
using MSP.Domain.Entities;
using MSP.Shared.Enums;
using Xunit;

namespace MSP.Tests.Services.SubscriptionServicesTest
{
    public class UgradeSubscriptionTest
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

        public UgradeSubscriptionTest()
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

        #region CreateSubscriptionAsync Tests

        [Fact]
        public async Task CreateSubscriptionAsync_WithValidRequest_ReturnsSuccessResponse()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var packageId = Guid.NewGuid();
            var subscriptionId = Guid.NewGuid();
            var orderCode = 123456789L;

            var createRequest = new CreateSubscriptionRequest
            {
                UserId = userId,
                PackageId = packageId,
                ReturnUrl = "https://example.com/return",
                CancelUrl = "https://example.com/cancel"
            };

            var package = new Package
            {
                Id = packageId,
                Name = "Premium Package",
                Price = 99.99m,
                Description = "Premium subscription",
                BillingCycle = 1,
                Currency = "VND",
                Limitations = new List<Limitation>()
            };

            var paymentLink = new PaymentResponse
            {
                OrderCode = orderCode,
                CheckoutUrl = "https://payos.vn/checkout",
                QrCode = "https://payos.vn/qr",
                Status = PaymentEnum.Pending.ToString()
            };

            _mockPackageRepository
                .Setup(x => x.GetPackageByIdAsync(packageId))
                .ReturnsAsync(package);

            _mockPaymentService
                .Setup(x => x.CreatePaymentLinkAsync(It.IsAny<CreatePaymentLinkRequest>()))
                .ReturnsAsync(paymentLink);

            _mockSubscriptionRepository
                .Setup(x => x.AddAsync(It.IsAny<Subscription>()))
                .ReturnsAsync((Subscription s) => s);

            _mockSubscriptionRepository
                .Setup(x => x.SaveChangesAsync())
                .Returns(Task.CompletedTask);

            // Act
            var result = await _subscriptionService.CreateSubscriptionAsync(createRequest);

            // Assert
            Assert.True(result.Success);
            Assert.Equal("Create subscription successfully", result.Message);
            Assert.NotNull(result.Data);
            Assert.Equal(paymentLink.CheckoutUrl, result.Data.PaymentUrl);
            Assert.Equal(paymentLink.QrCode, result.Data.QrCodeUrl);
            Assert.Equal((int)orderCode, result.Data.OrderCode);
            Assert.Equal(package.Price, result.Data.TotalAmount);
            Assert.Equal(Guid.Empty, result.Data.SubscriptionId);

            _mockPackageRepository.Verify(x => x.GetPackageByIdAsync(packageId), Times.Once);
            _mockPaymentService.Verify(x => x.CreatePaymentLinkAsync(It.IsAny<CreatePaymentLinkRequest>()), Times.Once);
            _mockSubscriptionRepository.Verify(x => x.AddAsync(It.IsAny<Subscription>()), Times.Once);
            _mockSubscriptionRepository.Verify(x => x.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task CreateSubscriptionAsync_WithInvalidPackageId_ReturnsErrorResponse()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var packageId = Guid.NewGuid();

            var createRequest = new CreateSubscriptionRequest
            {
                UserId = userId,
                PackageId = packageId,
                ReturnUrl = "https://example.com/return",
                CancelUrl = "https://example.com/cancel"
            };

            _mockPackageRepository
                .Setup(x => x.GetPackageByIdAsync(packageId))
                .ReturnsAsync((Package)null);

            // Act
            var result = await _subscriptionService.CreateSubscriptionAsync(createRequest);

            // Assert
            Assert.False(result.Success);
            Assert.Equal("Package not found", result.Message);
            Assert.Null(result.Data);

            _mockPackageRepository.Verify(x => x.GetPackageByIdAsync(packageId), Times.Once);
            _mockPaymentService.Verify(x => x.CreatePaymentLinkAsync(It.IsAny<CreatePaymentLinkRequest>()), Times.Never);
            _mockSubscriptionRepository.Verify(x => x.AddAsync(It.IsAny<Subscription>()), Times.Never);
        }

        [Fact]
        public async Task CreateSubscriptionAsync_WithValidRequest_CreatesSubscriptionWithCorrectData()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var packageId = Guid.NewGuid();
            var orderCode = 987654321L;
            var beforeTest = DateTime.UtcNow;

            var createRequest = new CreateSubscriptionRequest
            {
                UserId = userId,
                PackageId = packageId,
                ReturnUrl = "https://example.com/return",
                CancelUrl = "https://example.com/cancel"
            };

            var package = new Package
            {
                Id = packageId,
                Name = "Standard Package",
                Price = 49.99m,
                Description = "Standard subscription",
                BillingCycle = 1,
                Currency = "VND",
                Limitations = new List<Limitation>()
            };

            var paymentLink = new PaymentResponse
            {
                OrderCode = orderCode,
                CheckoutUrl = "https://payos.vn/checkout",
                QrCode = "https://payos.vn/qr",
                Status = "PENDING"
            };

            Subscription capturedSubscription = null;

            _mockPackageRepository
                .Setup(x => x.GetPackageByIdAsync(packageId))
                .ReturnsAsync(package);

            _mockPaymentService
                .Setup(x => x.CreatePaymentLinkAsync(It.IsAny<CreatePaymentLinkRequest>()))
                .ReturnsAsync(paymentLink);

            _mockSubscriptionRepository
                .Setup(x => x.AddAsync(It.IsAny<Subscription>()))
                .Callback<Subscription>(s => capturedSubscription = s)
                .ReturnsAsync((Subscription s) => s);

            _mockSubscriptionRepository
                .Setup(x => x.SaveChangesAsync())
                .Returns(Task.CompletedTask);

            // Act
            await _subscriptionService.CreateSubscriptionAsync(createRequest);
            var afterTest = DateTime.UtcNow;

            // Assert
            Assert.NotNull(capturedSubscription);
            Assert.Equal(userId, capturedSubscription.UserId);
            Assert.Equal(packageId, capturedSubscription.PackageId);
            Assert.Equal(package.Price, capturedSubscription.TotalPrice);
            Assert.Equal(orderCode.ToString(), capturedSubscription.TransactionID);
            Assert.Equal("PENDING", capturedSubscription.Status);
            Assert.False(capturedSubscription.IsActive);
            Assert.Empty(capturedSubscription.PaymentMethod);
            Assert.True(capturedSubscription.CreatedAt >= beforeTest && capturedSubscription.CreatedAt <= afterTest);

            // Verify snapshot is captured
            Assert.NotNull(capturedSubscription.SnapshotPackageJson);
            Assert.NotNull(capturedSubscription.SnapshotLimitationsJson);
        }

        [Fact]
        public async Task CreateSubscriptionAsync_PaymentRequestContainsCorrectData()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var packageId = Guid.NewGuid();
            var returnUrl = "https://example.com/return";
            var cancelUrl = "https://example.com/cancel";

            var createRequest = new CreateSubscriptionRequest
            {
                UserId = userId,
                PackageId = packageId,
                ReturnUrl = returnUrl,
                CancelUrl = cancelUrl
            };

            var package = new Package
            {
                Id = packageId,
                Name = "Test Package",
                Price = 100000,
                Description = "Test subscription",
                BillingCycle = 1,
                Currency = "USD",
                Limitations = new List<Limitation>()
            };

            var paymentLink = new PaymentResponse
            {
                OrderCode = 111111111L,
                CheckoutUrl = "https://payos.vn/checkout",
                QrCode = "https://payos.vn/qr",
                Status = "PENDING"
            };

            CreatePaymentLinkRequest capturedPaymentRequest = null;

            _mockPackageRepository
                .Setup(x => x.GetPackageByIdAsync(packageId))
                .ReturnsAsync(package);

            _mockPaymentService
                .Setup(x => x.CreatePaymentLinkAsync(It.IsAny<CreatePaymentLinkRequest>()))
                .Callback<CreatePaymentLinkRequest>(r => capturedPaymentRequest = r)
                .ReturnsAsync(paymentLink);

            _mockSubscriptionRepository
                .Setup(x => x.AddAsync(It.IsAny<Subscription>()))
                .ReturnsAsync((Subscription s) => s);

            _mockSubscriptionRepository
                .Setup(x => x.SaveChangesAsync())
                .Returns(Task.CompletedTask);

            // Act
            await _subscriptionService.CreateSubscriptionAsync(createRequest);

            // Assert
            Assert.NotNull(capturedPaymentRequest);
            Assert.Equal((int)package.Price, capturedPaymentRequest.Amount);
            Assert.Equal("Buy package", capturedPaymentRequest.Description);
            Assert.Equal(returnUrl, capturedPaymentRequest.ReturnUrl);
            Assert.Equal(cancelUrl, capturedPaymentRequest.CancelUrl);
        }

        [Fact]
        public async Task CreateSubscriptionAsync_GeneratesNewSubscriptionId()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var packageId = Guid.NewGuid();

            var createRequest = new CreateSubscriptionRequest
            {
                UserId = userId,
                PackageId = packageId,
                ReturnUrl = "https://example.com/return",
                CancelUrl = "https://example.com/cancel"
            };

            var package = new Package
            {
                Id = packageId,
                Name = "Test Package",
                Price = 50000,
                BillingCycle = 30,
                Currency = "VND",
                Limitations = new List<Limitation>()
            };

            var paymentLink = new PaymentResponse
            {
                OrderCode = 999999999L,
                CheckoutUrl = "https://payos.vn/checkout",
                QrCode = "https://payos.vn/qr",
                Status = "PENDING"
            };

            Subscription capturedSubscription = null;

            _mockPackageRepository
                .Setup(x => x.GetPackageByIdAsync(packageId))
                .ReturnsAsync(package);

            _mockPaymentService
                .Setup(x => x.CreatePaymentLinkAsync(It.IsAny<CreatePaymentLinkRequest>()))
                .ReturnsAsync(paymentLink);

            _mockSubscriptionRepository
                .Setup(x => x.AddAsync(It.IsAny<Subscription>()))
                .Callback<Subscription>(s => capturedSubscription = s)
                .ReturnsAsync((Subscription s) => s);

            _mockSubscriptionRepository
                .Setup(x => x.SaveChangesAsync())
                .Returns(Task.CompletedTask);

            // Act
            var result = await _subscriptionService.CreateSubscriptionAsync(createRequest);

            // Assert
            Assert.NotNull(capturedSubscription);
            Assert.Equal(Guid.Empty, capturedSubscription.Id);
            Assert.Equal(capturedSubscription.Id, result.Data.SubscriptionId);
        }

        [Fact]
        public async Task CreateSubscriptionAsync_WithDifferentPrices_CreatesCorrectSubscriptions()
        {
            // Arrange
            var testCases = new[]
            {
                new { Price = 0m, Name = "Free" },
                new { Price = 49.99m, Name = "Basic" },
                new { Price = 99.99m, Name = "Standard" },
                new { Price = 199.99m, Name = "Premium" }
            };

            foreach (var testCase in testCases)
            {
                var userId = Guid.NewGuid();
                var packageId = Guid.NewGuid();

                var createRequest = new CreateSubscriptionRequest
                {
                    UserId = userId,
                    PackageId = packageId,
                    ReturnUrl = "https://example.com/return",
                    CancelUrl = "https://example.com/cancel"
                };

                var package = new Package
                {
                    Id = packageId,
                    Name = testCase.Name,
                    Price = testCase.Price,
                    BillingCycle = 30,
                    Currency = "VND",
                    Limitations = new List<Limitation>()
                };

                var paymentLink = new PaymentResponse
                {
                    OrderCode = 123456789L,
                    CheckoutUrl = "https://payos.vn/checkout",
                    QrCode = "https://payos.vn/qr",
                    Status = "PENDING"
                };

                Subscription capturedSubscription = null;

                _mockPackageRepository
                    .Setup(x => x.GetPackageByIdAsync(packageId))
                    .ReturnsAsync(package);

                _mockPaymentService
                    .Setup(x => x.CreatePaymentLinkAsync(It.IsAny<CreatePaymentLinkRequest>()))
                    .ReturnsAsync(paymentLink);

                _mockSubscriptionRepository
                    .Setup(x => x.AddAsync(It.IsAny<Subscription>()))
                    .Callback<Subscription>(s => capturedSubscription = s)
                    .ReturnsAsync((Subscription s) => s);

                _mockSubscriptionRepository
                    .Setup(x => x.SaveChangesAsync())
                    .Returns(Task.CompletedTask);

                // Act
                var result = await _subscriptionService.CreateSubscriptionAsync(createRequest);

                // Assert
                Assert.NotNull(capturedSubscription);
                Assert.Equal(testCase.Price, capturedSubscription.TotalPrice);
                Assert.Equal(testCase.Price, result.Data.TotalAmount);
            }
        }

        [Fact]
        public async Task CreateSubscriptionAsync_SetsExpiresAtTo15MinutesFromNow()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var packageId = Guid.NewGuid();
            var beforeTest = DateTime.UtcNow;

            var createRequest = new CreateSubscriptionRequest
            {
                UserId = userId,
                PackageId = packageId,
                ReturnUrl = "https://example.com/return",
                CancelUrl = "https://example.com/cancel"
            };

            var package = new Package
            {
                Id = packageId,
                Name = "Test Package",
                Price = 100000,
                BillingCycle = 30,
                Currency = "VND",
                Limitations = new List<Limitation>()
            };

            var paymentLink = new PaymentResponse
            {
                OrderCode = 123456789L,
                CheckoutUrl = "https://payos.vn/checkout",
                QrCode = "https://payos.vn/qr",
                Status = "PENDING"
            };

            _mockPackageRepository
                .Setup(x => x.GetPackageByIdAsync(packageId))
                .ReturnsAsync(package);

            _mockPaymentService
                .Setup(x => x.CreatePaymentLinkAsync(It.IsAny<CreatePaymentLinkRequest>()))
                .ReturnsAsync(paymentLink);

            _mockSubscriptionRepository
                .Setup(x => x.AddAsync(It.IsAny<Subscription>()))
                .ReturnsAsync((Subscription s) => s);

            _mockSubscriptionRepository
                .Setup(x => x.SaveChangesAsync())
                .Returns(Task.CompletedTask);

            // Act
            var result = await _subscriptionService.CreateSubscriptionAsync(createRequest);
            var afterTest = DateTime.UtcNow;

            // Assert
            var expectedMinExpiry = beforeTest.AddMinutes(15);
            var expectedMaxExpiry = afterTest.AddMinutes(15);

            Assert.True(result.Data.ExpiresAt >= expectedMinExpiry && result.Data.ExpiresAt <= expectedMaxExpiry);
        }

        [Fact]
        public async Task CreateSubscriptionAsync_WithPackageHavingLimitations_CapturesSnapshot()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var packageId = Guid.NewGuid();

            var createRequest = new CreateSubscriptionRequest
            {
                UserId = userId,
                PackageId = packageId,
                ReturnUrl = "https://example.com/return",
                CancelUrl = "https://example.com/cancel"
            };

            var limitations = new List<Limitation>
            {
                new Limitation
                {
                    Id = Guid.NewGuid(),
                    Name = "Projects",
                    Description = "Number of projects",
                    LimitationType = "NumberProject",
                    IsUnlimited = false,
                    LimitValue = 10,
                    LimitUnit = "projects"
                },
                new Limitation
                {
                    Id = Guid.NewGuid(),
                    Name = "Meetings",
                    Description = "Number of meetings",
                    LimitationType = "NumberMeeting",
                    IsUnlimited = true,
                    LimitValue = null,
                    LimitUnit = "meetings"
                }
            };

            var package = new Package
            {
                Id = packageId,
                Name = "Premium Package",
                Price = 199.99m,
                Description = "Premium with limitations",
                BillingCycle = 30,
                Currency = "VND",
                Limitations = limitations
            };

            var paymentLink = new PaymentResponse
            {
                OrderCode = 123456789L,
                CheckoutUrl = "https://payos.vn/checkout",
                QrCode = "https://payos.vn/qr",
                Status = "PENDING"
            };

            Subscription capturedSubscription = null;

            _mockPackageRepository
                .Setup(x => x.GetPackageByIdAsync(packageId))
                .ReturnsAsync(package);

            _mockPaymentService
                .Setup(x => x.CreatePaymentLinkAsync(It.IsAny<CreatePaymentLinkRequest>()))
                .ReturnsAsync(paymentLink);

            _mockSubscriptionRepository
                .Setup(x => x.AddAsync(It.IsAny<Subscription>()))
                .Callback<Subscription>(s => capturedSubscription = s)
                .ReturnsAsync((Subscription s) => s);

            _mockSubscriptionRepository
                .Setup(x => x.SaveChangesAsync())
                .Returns(Task.CompletedTask);

            // Act
            await _subscriptionService.CreateSubscriptionAsync(createRequest);

            // Assert
            Assert.NotNull(capturedSubscription);
            Assert.NotNull(capturedSubscription.SnapshotPackageJson);
            Assert.NotNull(capturedSubscription.SnapshotLimitationsJson);
            Assert.Contains("Premium Package", capturedSubscription.SnapshotPackageJson);
            Assert.Contains("Projects", capturedSubscription.SnapshotLimitationsJson);
            Assert.Contains("Meetings", capturedSubscription.SnapshotLimitationsJson);
        }

        #endregion
    }
}
