using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Moq;
using MSP.Application.Abstracts;
using MSP.Application.Exceptions;
using MSP.Application.Models.Requests.Auth;
using MSP.Application.Services.Implementations.Auth;
using MSP.Application.Services.Interfaces.Auth;
using MSP.Application.Services.Interfaces.Notification;
using MSP.Application.Services.Interfaces.OrganizationInvitation;
using MSP.Domain.Entities;
using MSP.Shared.Enums;
using Xunit;

namespace MSP.Tests.Services.AccountServicesTest
{
    public class RegisterAccountTest
    {
        private readonly Mock<UserManager<User>> _mockUserManager;
        private readonly Mock<INotificationService> _mockNotificationService;
        private readonly Mock<IConfiguration> _mockConfiguration;
        private readonly Mock<IAuthTokenProcessor> _mockAuthTokenProcessor;
        private readonly Mock<IUserRepository> _mockUserRepository;
        private readonly Mock<IGoogleTokenValidator> _mockGoogleTokenValidator;
        private readonly Mock<IOrganizationInvitationService> _mockOrgInvitationService;

        private readonly AccountService _accountService;

        public RegisterAccountTest()
        {
            var store = new Mock<IUserStore<User>>();

            // ✅ Tạo PasswordHasher thật thay vì mock
            var passwordHasher = new PasswordHasher<User>();

            _mockUserManager = new Mock<UserManager<User>>(
                store.Object,
                null, // IOptions<IdentityOptions>
                passwordHasher, // ✅ Truyền passwordHasher thật vào đây
                null, // IEnumerable<IUserValidator<User>>
                null, // IEnumerable<IPasswordValidator<User>>
                null, // ILookupNormalizer
                null, // IdentityErrorDescriber
                null, // IServiceProvider
                null  // ILogger<UserManager<User>>
            );

            _mockNotificationService = new Mock<INotificationService>();
            _mockConfiguration = new Mock<IConfiguration>();
            _mockAuthTokenProcessor = new Mock<IAuthTokenProcessor>();
            _mockUserRepository = new Mock<IUserRepository>();
            _mockGoogleTokenValidator = new Mock<IGoogleTokenValidator>();
            _mockOrgInvitationService = new Mock<IOrganizationInvitationService>();

            _mockConfiguration
                .Setup(c => c["AppSettings:ClientUrl"])
                .Returns("https://client.msp.com");

            _accountService = new AccountService(
                _mockAuthTokenProcessor.Object,
                _mockUserManager.Object,
                _mockUserRepository.Object,
                _mockNotificationService.Object,
                _mockConfiguration.Object,
                _mockGoogleTokenValidator.Object,
                _mockOrgInvitationService.Object
            );
        }

        #region TC_Register_01 - Email already exists
        [Fact]
        public async Task RegisterAsync_EmailAlreadyExists_ReturnsError()
        {
            // Arrange
            var request = new RegisterRequest
            {
                FullName = "Test User",
                Email = "existing@example.com",
                Password = "Password123!",
                PhoneNumber = "0123456789",
                Role = "Member"
            };

            var existingUser = new User { Email = request.Email, FullName = "Test User" };
            _mockUserManager
                .Setup(x => x.FindByEmailAsync(request.Email))
                .ReturnsAsync(existingUser);

            // Act
            var result = await _accountService.RegisterAsync(request);

            // Assert
            Assert.False(result.Success);
            Assert.Equal("This email is already in use.", result.Message);

            _mockUserManager.Verify(x => x.FindByEmailAsync(request.Email), Times.Once);
            _mockUserManager.Verify(x => x.CreateAsync(It.IsAny<User>()), Times.Never);
            _mockNotificationService.Verify(
                x => x.SendEmailNotification(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()),
                Times.Never);
        }
        #endregion

        #region TC_Register_02 - Success register as Member
        [Fact]
        public async Task RegisterAsync_ValidMember_CreatesUserAndSendsEmail()
        {
            // Arrange
            var request = new RegisterRequest
            {
                FullName = "John Member",
                Email = "member@example.com",
                Password = "Password123!",
                PhoneNumber = "0987654321",
                Role = "Member"
            };

            _mockUserManager
                .Setup(x => x.FindByEmailAsync(request.Email))
                .ReturnsAsync((User)null);

            _mockUserManager
                .Setup(x => x.CreateAsync(It.IsAny<User>()))
                .ReturnsAsync(IdentityResult.Success);

            _mockUserManager
                .Setup(x => x.AddToRoleAsync(It.IsAny<User>(), UserRoleEnum.Member.ToString()))
                .ReturnsAsync(IdentityResult.Success);

            _mockUserManager
                .Setup(x => x.GenerateEmailConfirmationTokenAsync(It.IsAny<User>()))
                .ReturnsAsync("confirmation-token-123");

            // Act
            var result = await _accountService.RegisterAsync(request);

            // Assert
            Assert.True(result.Success);

            // ✅ Kiểm tra Message (chuỗi cố định)
            Assert.Equal("Account registration successful! Please check your email to confirm your account.", result.Message);

            // ✅ Kiểm tra Data (message động dựa trên role)
            Assert.Contains("User created successfully!", result.Data);
            Assert.Contains("Please check your email", result.Data);
            Assert.DoesNotContain("reviewed by an admin", result.Data); // Member không cần admin approval

            _mockUserManager.Verify(x => x.FindByEmailAsync(request.Email), Times.Once);
            _mockUserManager.Verify(
                x => x.CreateAsync(It.Is<User>(u =>
                    u.Email == request.Email &&
                    u.FullName == request.FullName &&
                    u.IsApproved == true && // Member auto-approved
                    !string.IsNullOrEmpty(u.PasswordHash)
                )),
                Times.Once);

            _mockUserManager.Verify(
                x => x.AddToRoleAsync(It.IsAny<User>(), UserRoleEnum.Member.ToString()),
                Times.Once);

            _mockUserManager.Verify(
                x => x.GenerateEmailConfirmationTokenAsync(It.IsAny<User>()),
                Times.Once);

            _mockNotificationService.Verify(
                x => x.SendEmailNotification(
                    request.Email,
                    "Email Confirmation - Meeting Support Platform",
                    It.Is<string>(body => body.Contains("confirm-email"))),
                Times.Once);
        }
        #endregion

        #region TC_Register_03 - Success register as BusinessOwner
        [Fact]
        public async Task RegisterAsync_ValidBusinessOwner_CreatesUserWithOrgAndRequiresApproval()
        {
            // Arrange
            var request = new RegisterRequest
            {
                FullName = "Jane Owner",
                Email = "owner@example.com",
                Password = "Password123!",
                PhoneNumber = "0123456789",
                Role = "BusinessOwner",
                Organization = "ABC Corp",
                BusinessLicense = "BL123456"
            };

            _mockUserManager
                .Setup(x => x.FindByEmailAsync(request.Email))
                .ReturnsAsync((User)null);

            _mockUserManager
                .Setup(x => x.CreateAsync(It.IsAny<User>()))
                .ReturnsAsync(IdentityResult.Success);

            _mockUserManager
                .Setup(x => x.AddToRoleAsync(It.IsAny<User>(), UserRoleEnum.BusinessOwner.ToString()))
                .ReturnsAsync(IdentityResult.Success);

            _mockUserManager
                .Setup(x => x.GenerateEmailConfirmationTokenAsync(It.IsAny<User>()))
                .ReturnsAsync("confirmation-token-456");

            // Act
            var result = await _accountService.RegisterAsync(request);

            // Assert
            Assert.True(result.Success);

            // ✅ Kiểm tra Message (chuỗi cố định)
            Assert.Equal("Account registration successful! Please check your email to confirm your account.", result.Message);

            // ✅ Kiểm tra Data (message có thông báo admin approval)
            Assert.Contains("User created successfully!", result.Data);
            Assert.Contains("reviewed by an admin", result.Data);
            Assert.Contains("before you can log in", result.Data);

            _mockUserManager.Verify(
                x => x.CreateAsync(It.Is<User>(u =>
                    u.Email == request.Email &&
                    u.Organization == "ABC Corp" &&
                    u.BusinessLicense == "BL123456" &&
                    u.IsApproved == false && // BusinessOwner needs approval
                    !string.IsNullOrEmpty(u.PasswordHash)
                )),
                Times.Once);

            _mockUserManager.Verify(
                x => x.AddToRoleAsync(It.IsAny<User>(), UserRoleEnum.BusinessOwner.ToString()),
                Times.Once);

            _mockNotificationService.Verify(
                x => x.SendEmailNotification(
                    request.Email,
                    "Email Confirmation - Meeting Support Platform",
                    It.IsAny<string>()),
                Times.Once);
        }
        #endregion

        #region TC_Register_04 - Invalid role throws exception
        [Fact]
        public async Task RegisterAsync_InvalidRole_ThrowsArgumentException()
        {
            // Arrange
            var request = new RegisterRequest
            {
                FullName = "Test User",
                Email = "test@example.com",
                Password = "Password123!",
                PhoneNumber = "0123456789",
                Role = "InvalidRole"
            };

            _mockUserManager
                .Setup(x => x.FindByEmailAsync(request.Email))
                .ReturnsAsync((User)null);

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(
                async () => await _accountService.RegisterAsync(request));

            _mockUserManager.Verify(x => x.CreateAsync(It.IsAny<User>()), Times.Never);
        }
        #endregion

        #region TC_Register_05 - CreateAsync fails throws RegistrationFailedException
        [Fact]
        public async Task RegisterAsync_CreateFails_ThrowsRegistrationFailedException()
        {
            // Arrange
            var request = new RegisterRequest
            {
                FullName = "Test User",
                Email = "test@example.com",
                Password = "weak",
                PhoneNumber = "0123456789",
                Role = "Member"
            };

            _mockUserManager
                .Setup(x => x.FindByEmailAsync(request.Email))
                .ReturnsAsync((User)null);

            var errors = new[]
            {
                new IdentityError { Description = "Password too weak" },
                new IdentityError { Description = "Email format invalid" }
            };

            _mockUserManager
                .Setup(x => x.CreateAsync(It.IsAny<User>()))
                .ReturnsAsync(IdentityResult.Failed(errors));

            // Act & Assert
            var exception = await Assert.ThrowsAsync<RegistrationFailedException>(
                async () => await _accountService.RegisterAsync(request));

            Assert.Contains("Password too weak", exception.Message);

            _mockUserManager.Verify(x => x.AddToRoleAsync(It.IsAny<User>(), It.IsAny<string>()), Times.Never);
            _mockNotificationService.Verify(
                x => x.SendEmailNotification(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()),
                Times.Never);
        }
        #endregion

        #region TC_Register_06 - Register with InviteToken includes token in URL
        [Fact]
        public async Task RegisterAsync_WithInviteToken_IncludesTokenInConfirmationUrl()
        {
            // Arrange
            var request = new RegisterRequest
            {
                FullName = "Invited User",
                Email = "invited@example.com",
                Password = "Password123!",
                PhoneNumber = "0123456789",
                Role = "Member",
                InviteToken = "invite-abc-123"
            };

            _mockUserManager
                .Setup(x => x.FindByEmailAsync(request.Email))
                .ReturnsAsync((User)null);

            _mockUserManager
                .Setup(x => x.CreateAsync(It.IsAny<User>()))
                .ReturnsAsync(IdentityResult.Success);

            _mockUserManager
                .Setup(x => x.AddToRoleAsync(It.IsAny<User>(), It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Success);

            _mockUserManager
                .Setup(x => x.GenerateEmailConfirmationTokenAsync(It.IsAny<User>()))
                .ReturnsAsync("confirm-token-789");

            // Act
            var result = await _accountService.RegisterAsync(request);

            // Assert
            Assert.True(result.Success);

            _mockNotificationService.Verify(
                x => x.SendEmailNotification(
                    request.Email,
                    "Email Confirmation - Meeting Support Platform",
                    It.Is<string>(body => body.Contains("inviteToken="))),
                Times.Once);
        }
        #endregion

        #region TC_Register_07 - Missing required fields
        [Fact]
        public async Task RegisterAsync_EmptyEmail_ReturnsError()
        {
            // Arrange
            var request = new RegisterRequest
            {
                FullName = "Test",
                Email = "",
                Password = "Password123!",
                PhoneNumber = "0123456789",
                Role = "Member"
            };

            _mockUserManager
                .Setup(x => x.FindByEmailAsync(request.Email))
                .ReturnsAsync((User)null);

            _mockUserManager
                .Setup(x => x.CreateAsync(It.IsAny<User>()))
                .ReturnsAsync(IdentityResult.Failed(new IdentityError { Description = "Email is required" }));

            // Act & Assert
            await Assert.ThrowsAsync<RegistrationFailedException>(
                async () => await _accountService.RegisterAsync(request));
        }
        #endregion
    }
}
