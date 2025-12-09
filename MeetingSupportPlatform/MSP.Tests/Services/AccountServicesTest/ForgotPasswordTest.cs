using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Moq;
using MSP.Domain.Entities;
using MSP.Application.Abstracts;
using MSP.Application.Models.Requests.User;
using MSP.Application.Services.Implementations.Users;
using MSP.Application.Services.Interfaces.Notification;
using Xunit;
using MSP.Application.Services.Implementations.Auth;
using MSP.Application.Services.Implementations.Notification;
using MSP.Application.Services.Interfaces.Auth;
using MSP.Application.Services.Interfaces.OrganizationInvitation;

namespace MSP.Tests.Services.AccountServicesTest
{
    public class ForgotPasswordTest
    {
        private readonly Mock<UserManager<User>> _mockUserManager;
        private readonly Mock<INotificationService> _mockNotificationService;
        private readonly Mock<IConfiguration> _mockConfiguration;
        private readonly Mock<IAuthTokenProcessor> _mockAuthTokenProcessor;
        private readonly Mock<IUserRepository> _mockUserRepository;
        private readonly Mock<IGoogleTokenValidator> _mockGoogleTokenValidator;
        private Mock<IOrganizationInvitationService> _mockOrganizationInvitationService;
        private readonly IAccountService _accountService;

        public ForgotPasswordTest()
        {
            _mockUserManager = new Mock<UserManager<User>>(
                new Mock<IUserStore<User>>().Object,
                null, null, null, null, null, null, null, null
            );

            _mockNotificationService = new Mock<INotificationService>();
            _mockConfiguration = new Mock<IConfiguration>();

            _mockConfiguration
                .Setup(c => c["AppSettings:ClientUrl"])
                .Returns("https://client.msp.com");

            _mockAuthTokenProcessor = new Mock<IAuthTokenProcessor>();
            _mockUserRepository = new Mock<IUserRepository>();
            _mockGoogleTokenValidator = new Mock<IGoogleTokenValidator>();
            _mockOrganizationInvitationService = new Mock<IOrganizationInvitationService>();

            _accountService = new AccountService(
                _mockAuthTokenProcessor.Object,
                _mockUserManager.Object,
                _mockUserRepository.Object,
                _mockNotificationService.Object,
                _mockConfiguration.Object,
                _mockGoogleTokenValidator.Object,
                _mockOrganizationInvitationService.Object
            );
        }

        #region TC_Forgot_01 - User not found
        [Fact]
        public async Task ForgotPasswordAsync_UserNotFound_ReturnsError()
        {
            // Arrange
            var request = new Application.Models.Requests.Auth.ForgotPasswordRequest { Email = "notfound@example.com" };

            _mockUserManager
                .Setup(x => x.FindByEmailAsync(request.Email))
                .ReturnsAsync((User)null);

            // Act
            var result = await _accountService.ForgotPasswordAsync(request);

            // Assert
            Assert.False(result.Success);
            Assert.Equal("User not found.", result.Message);
            _mockUserManager.Verify(x => x.FindByEmailAsync(request.Email), Times.Once);
            _mockUserManager.Verify(
                x => x.GeneratePasswordResetTokenAsync(It.IsAny<User>()),
                Times.Never);
            _mockNotificationService.Verify(
                x => x.SendEmailNotification(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()),
                Times.Never);
        }
        #endregion

        #region TC_Forgot_02 - Success sends email
        [Fact]
        public async Task ForgotPasswordAsync_ValidUser_SendsResetEmail()
        {
            // Arrange
            var request = new Application.Models.Requests.Auth.ForgotPasswordRequest { Email = "user@example.com" };
            var user = new User { Id = Guid.NewGuid(), Email = request.Email, FullName = "Test User" };

            _mockUserManager
                .Setup(x => x.FindByEmailAsync(request.Email))
                .ReturnsAsync(user);

            _mockUserManager
                .Setup(x => x.GeneratePasswordResetTokenAsync(user))
                .ReturnsAsync("reset-token-123");

            // Act
            var result = await _accountService.ForgotPasswordAsync(request);

            // Assert
            Assert.True(result.Success);
            Assert.Equal("Password reset email sent successfully! Please check your email.", result.Message);

            _mockUserManager.Verify(x => x.FindByEmailAsync(request.Email), Times.Once);
            _mockUserManager.Verify(x => x.GeneratePasswordResetTokenAsync(user), Times.Once);

            _mockNotificationService.Verify(
                x => x.SendEmailNotification(
                    user.Email,
                    "Password Reset - Meeting Support Platform",
                    It.Is<string>(body => body.Contains("reset-password"))),
                Times.Once);
        }
        #endregion

        #region TC_Forgot_03 - Missing client URL config
        [Fact]
        public async Task ForgotPasswordAsync_MissingClientUrl_StillSendsEmailWithDefaultUrl()
        {
            // Arrange
            var request = new Application.Models.Requests.Auth.ForgotPasswordRequest { Email = "user@example.com" };
            var user = new User { Id = Guid.NewGuid(), Email = request.Email, FullName = "Test User" };

            _mockUserManager
                .Setup(x => x.FindByEmailAsync(request.Email))
                .ReturnsAsync(user);

            _mockUserManager
                .Setup(x => x.GeneratePasswordResetTokenAsync(user))
                .ReturnsAsync("reset-token-123");

            _mockConfiguration
                .Setup(c => c["AppSettings:ClientUrl"])
                .Returns((string)null);

            // Act
            var result = await _accountService.ForgotPasswordAsync(request);

            // Assert
            Assert.True(result.Success);

            _mockNotificationService.Verify(
                x => x.SendEmailNotification(
                    user.Email,
                    "Password Reset - Meeting Support Platform",
                    It.IsAny<string>()),
                Times.Once);
        }
        #endregion

        #region TC_Forgot_04 - Invalid email format (optional nếu có validate)
        [Fact]
        public async Task ForgotPasswordAsync_InvalidEmail_ReturnsModelErrorOrNoCallToService()
        {
            // Arrange
            var request = new Application.Models.Requests.Auth.ForgotPasswordRequest { Email = "" };

            // Giả sử validate ở Controller, ở đây chỉ cần đảm bảo không crash khi Email rỗng
            _mockUserManager
                .Setup(x => x.FindByEmailAsync(request.Email))
                .ReturnsAsync((User)null);

            // Act
            var result = await _accountService.ForgotPasswordAsync(request);

            // Assert
            Assert.False(result.Success);
        }
        #endregion
    }
}
