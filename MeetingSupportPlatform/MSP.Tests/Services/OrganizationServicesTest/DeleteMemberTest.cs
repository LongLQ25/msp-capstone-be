using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Moq;
using MSP.Application.Abstracts;
using MSP.Application.Models.Requests.Notification;
using MSP.Application.Models.Responses.Notification;
using MSP.Application.Repositories;
using MSP.Application.Services.Implementations.Users;
using MSP.Application.Services.Interfaces.Notification;
using MSP.Application.Services.Interfaces.Users;
using MSP.Domain.Entities;
using MSP.Shared.Common;
using MSP.Shared.Enums;
using Xunit;

namespace MSP.Tests.Services.UserServicesTest
{
    public class DeleteMemberTest
    {
        private readonly Mock<IUserRepository> _mockUserRepository;
        private readonly Mock<UserManager<User>> _mockUserManager;
        private readonly Mock<INotificationService> _mockNotificationService;
        private readonly Mock<IOrganizationInviteRepository> _mockOrganizationInviteRepository;
        private readonly Mock<IProjectMemberRepository> _mockProjectMemberRepository;
        private readonly Mock<IProjectRepository> _mockProjectRepository;
        private readonly Mock<IProjectTaskRepository> _mockProjectTaskRepository;
        private readonly Mock<ISubscriptionRepository> _mockSubscriptionRepository;
        private readonly Mock<IPackageRepository> _mockPackageRepository;
        private readonly Mock<IConfiguration> _mockConfiguration;
        private readonly IUserService _userService;

        public DeleteMemberTest()
        {
            _mockUserRepository = new Mock<IUserRepository>();
            _mockNotificationService = new Mock<INotificationService>();
            _mockOrganizationInviteRepository = new Mock<IOrganizationInviteRepository>();
            _mockProjectMemberRepository = new Mock<IProjectMemberRepository>();
            _mockProjectRepository = new Mock<IProjectRepository>();
            _mockProjectTaskRepository = new Mock<IProjectTaskRepository>();
            _mockSubscriptionRepository = new Mock<ISubscriptionRepository>();
            _mockPackageRepository = new Mock<IPackageRepository>();
            _mockConfiguration = new Mock<IConfiguration>();

            // Setup configuration
            _mockConfiguration.Setup(x => x["AppSettings:ClientUrl"]).Returns("http://localhost:3000");

            var mockUserStore = new Mock<IUserStore<User>>();
            _mockUserManager = new Mock<UserManager<User>>(
                mockUserStore.Object,
                null, null, null, null, null, null, null, null
            );

            _userService = new UserService(
                _mockUserManager.Object,
                _mockUserRepository.Object,
                _mockNotificationService.Object,
                _mockOrganizationInviteRepository.Object,
                _mockProjectMemberRepository.Object,
                _mockProjectRepository.Object,
                _mockProjectTaskRepository.Object,
                _mockSubscriptionRepository.Object,
                _mockPackageRepository.Object,
                _mockConfiguration.Object
            );
        }

        [Fact]
        public async Task RemoveMemberFromOrganizationAsync_WithValidMember_ReturnsSuccessResponse()
        {
            // Arrange
            var businessOwnerId = Guid.NewGuid();
            var memberId = Guid.NewGuid();
            var projectId = Guid.NewGuid();

            var member = new User
            {
                Id = memberId,
                Email = "member@example.com",
                FullName = "Member User",
                Organization = "Test Organization",
                ManagedById = businessOwnerId
            };

            var projectMemberships = new List<ProjectMember>
            {
                new ProjectMember
                {
                    Id = Guid.NewGuid(),
                    ProjectId = projectId,
                    MemberId = memberId,
                    JoinedAt = DateTime.UtcNow,
                    LeftAt = null
                }
            };

            _mockUserManager
                .Setup(x => x.FindByIdAsync(memberId.ToString()))
                .ReturnsAsync(member);

            _mockUserManager
                .Setup(x => x.UpdateAsync(It.IsAny<User>()))
                .ReturnsAsync(IdentityResult.Success);

            _mockProjectRepository
                .Setup(x => x.GetProjectIdsByOwnerIdAsync(businessOwnerId))
                .ReturnsAsync(new List<Guid> { projectId });

            _mockProjectMemberRepository
                .Setup(x => x.GetActiveMembershipsByMemberAndProjectsAsync(memberId, It.IsAny<List<Guid>>()))
                .ReturnsAsync(projectMemberships);

            _mockProjectMemberRepository
                .Setup(x => x.UpdateRangeAsync(It.IsAny<List<ProjectMember>>()))
                .Returns(Task.CompletedTask);


            _mockProjectTaskRepository
                .Setup(x => x.GetTasksByProjectIdAsync(projectId))
                .ReturnsAsync(new List<ProjectTask>());

            // Act
            var result = await _userService.RemoveMemberFromOrganizationAsync(businessOwnerId, memberId);

            // Assert
            Assert.True(result.Success);
            Assert.Equal("Member removed successfully.", result.Message);
            Assert.Contains("Removed member from organization", result.Data);
            Assert.Contains("1 project(s)", result.Data);

            _mockUserManager.Verify(
                x => x.UpdateAsync(It.Is<User>(u =>
                    u.Id == memberId &&
                    u.Organization == null &&
                    u.ManagedById == null
                )),
                Times.Once
            );

            _mockProjectMemberRepository.Verify(
                x => x.UpdateRangeAsync(It.Is<List<ProjectMember>>(list =>
                    list.Count == 1 &&
                    list.All(pm => pm.LeftAt.HasValue)
                )),
                Times.Once
            );
        }

        [Fact]
        public async Task RemoveMemberFromOrganizationAsync_WithNonExistentMember_ReturnsErrorResponse()
        {
            // Arrange
            var businessOwnerId = Guid.NewGuid();
            var memberId = Guid.NewGuid();

            _mockUserManager
                .Setup(x => x.FindByIdAsync(memberId.ToString()))
                .ReturnsAsync((User?)null);

            // Act
            var result = await _userService.RemoveMemberFromOrganizationAsync(businessOwnerId, memberId);

            // Assert
            Assert.False(result.Success);
            Assert.Equal("Member not found.", result.Data);
        }

        [Fact]
        public async Task RemoveMemberFromOrganizationAsync_WithMemberNotBelongingToOrganization_ReturnsErrorResponse()
        {
            // Arrange
            var businessOwnerId = Guid.NewGuid();
            var memberId = Guid.NewGuid();
            var differentBusinessOwnerId = Guid.NewGuid();

            var member = new User
            {
                Id = memberId,
                Email = "member@example.com",
                FullName = "Member User",
                Organization = "Different Organization",
                ManagedById = differentBusinessOwnerId
            };

            _mockUserManager
                .Setup(x => x.FindByIdAsync(memberId.ToString()))
                .ReturnsAsync(member);

            // Act
            var result = await _userService.RemoveMemberFromOrganizationAsync(businessOwnerId, memberId);

            // Assert
            Assert.False(result.Success);
            Assert.Equal("This member does not belong to your organization.", result.Data);
        }

        [Fact]
        public async Task RemoveMemberFromOrganizationAsync_WithMemberWithoutOrganization_ReturnsErrorResponse()
        {
            // Arrange
            var businessOwnerId = Guid.NewGuid();
            var memberId = Guid.NewGuid();

            var member = new User
            {
                Id = memberId,
                Email = "member@example.com",
                FullName = "Member User",
                Organization = null,
                ManagedById = null
            };

            _mockUserManager
                .Setup(x => x.FindByIdAsync(memberId.ToString()))
                .ReturnsAsync(member);

            // Act
            var result = await _userService.RemoveMemberFromOrganizationAsync(businessOwnerId, memberId);

            // Assert
            Assert.False(result.Success);
            Assert.Equal("This member does not belong to your organization.", result.Data);
        }

        [Fact]
        public async Task RemoveMemberFromOrganizationAsync_WithIncompleteTasks_UnassignsTasks()
        {
            // Arrange
            var businessOwnerId = Guid.NewGuid();
            var memberId = Guid.NewGuid();
            var reviewerId = Guid.NewGuid();
            var projectId = Guid.NewGuid();
            var taskId = Guid.NewGuid();

            var member = new User
            {
                Id = memberId,
                Email = "member@example.com",
                FullName = "Member User",
                Organization = "Test Organization",
                ManagedById = businessOwnerId
            };

            var projectMemberships = new List<ProjectMember>
            {
                new ProjectMember
                {
                    Id = Guid.NewGuid(),
                    ProjectId = projectId,
                    MemberId = memberId,
                    JoinedAt = DateTime.UtcNow,
                    LeftAt = null
                }
            };

            var incompleteTasks = new List<ProjectTask>
            {
                new ProjectTask
                {
                    Id = taskId,
                    ProjectId = projectId,
                    UserId = memberId,
                    Title = "Incomplete Task",
                    Status = TaskEnum.InProgress.ToString(),
                    ReviewerId = reviewerId
                }
            };

            _mockUserManager
                .Setup(x => x.FindByIdAsync(memberId.ToString()))
                .ReturnsAsync(member);

            _mockUserManager
                .Setup(x => x.UpdateAsync(It.IsAny<User>()))
                .ReturnsAsync(IdentityResult.Success);

            _mockProjectRepository
                .Setup(x => x.GetProjectIdsByOwnerIdAsync(businessOwnerId))
                .ReturnsAsync(new List<Guid> { projectId });

            _mockProjectMemberRepository
                .Setup(x => x.GetActiveMembershipsByMemberAndProjectsAsync(memberId, It.IsAny<List<Guid>>()))
                .ReturnsAsync(projectMemberships);

            _mockProjectMemberRepository
                .Setup(x => x.UpdateRangeAsync(It.IsAny<List<ProjectMember>>()))
                .Returns(Task.CompletedTask);

            _mockProjectTaskRepository
                .Setup(x => x.GetTasksByProjectIdAsync(projectId))
                .ReturnsAsync(incompleteTasks);

            _mockProjectTaskRepository
                .Setup(x => x.UpdateAsync(It.IsAny<ProjectTask>()))
                .Returns(Task.CompletedTask);


            var notificationResponse = new NotificationResponse { Id = Guid.NewGuid() };
            _mockNotificationService
                .Setup(x => x.CreateInAppNotificationAsync(It.IsAny<CreateNotificationRequest>()))
                .ReturnsAsync(ApiResponse<NotificationResponse>.SuccessResponse(notificationResponse, "Notification sent"));

            // Act
            var result = await _userService.RemoveMemberFromOrganizationAsync(businessOwnerId, memberId);

            // Assert
            Assert.True(result.Success);

            _mockProjectTaskRepository.Verify(
                x => x.UpdateAsync(It.Is<ProjectTask>(t =>
                    t.Id == taskId &&
                    t.UserId == null
                )),
                Times.Once
            );

            _mockNotificationService.Verify(
                x => x.CreateInAppNotificationAsync(It.Is<CreateNotificationRequest>(n =>
                    n.UserId == reviewerId &&
                    n.Title == "Task Unassigned - Member Removed"
                )),
                Times.Once
            );
        }

        [Fact]
        public async Task RemoveMemberFromOrganizationAsync_WithCompletedTasks_DoesNotUnassignTasks()
        {
            // Arrange
            var businessOwnerId = Guid.NewGuid();
            var memberId = Guid.NewGuid();
            var projectId = Guid.NewGuid();

            var member = new User
            {
                Id = memberId,
                Email = "member@example.com",
                FullName = "Member User",
                Organization = "Test Organization",
                ManagedById = businessOwnerId
            };

            var projectMemberships = new List<ProjectMember>
            {
                new ProjectMember
                {
                    Id = Guid.NewGuid(),
                    ProjectId = projectId,
                    MemberId = memberId,
                    JoinedAt = DateTime.UtcNow,
                    LeftAt = null
                }
            };

            var completedTasks = new List<ProjectTask>
            {
                new ProjectTask
                {
                    Id = Guid.NewGuid(),
                    ProjectId = projectId,
                    UserId = memberId,
                    Title = "Completed Task",
                    Status = TaskEnum.Done.ToString()
                }
            };

            _mockUserManager
                .Setup(x => x.FindByIdAsync(memberId.ToString()))
                .ReturnsAsync(member);

            _mockUserManager
                .Setup(x => x.UpdateAsync(It.IsAny<User>()))
                .ReturnsAsync(IdentityResult.Success);

            _mockProjectRepository
                .Setup(x => x.GetProjectIdsByOwnerIdAsync(businessOwnerId))
                .ReturnsAsync(new List<Guid> { projectId });

            _mockProjectMemberRepository
                .Setup(x => x.GetActiveMembershipsByMemberAndProjectsAsync(memberId, It.IsAny<List<Guid>>()))
                .ReturnsAsync(projectMemberships);

            _mockProjectMemberRepository
                .Setup(x => x.UpdateRangeAsync(It.IsAny<List<ProjectMember>>()))
                .Returns(Task.CompletedTask);


            _mockProjectTaskRepository
                .Setup(x => x.GetTasksByProjectIdAsync(projectId))
                .ReturnsAsync(completedTasks);

            // Act
            var result = await _userService.RemoveMemberFromOrganizationAsync(businessOwnerId, memberId);

            // Assert
            Assert.True(result.Success);

            // Verify task was NOT updated (no unassignment for completed tasks)
            _mockProjectTaskRepository.Verify(
                x => x.UpdateAsync(It.IsAny<ProjectTask>()),
                Times.Never
            );

            _mockNotificationService.Verify(
                x => x.CreateInAppNotificationAsync(It.IsAny<CreateNotificationRequest>()),
                Times.Never
            );
        }

        [Fact]
        public async Task RemoveMemberFromOrganizationAsync_WhenUpdateFails_ReturnsErrorResponse()
        {
            // Arrange
            var businessOwnerId = Guid.NewGuid();
            var memberId = Guid.NewGuid();

            var member = new User
            {
                Id = memberId,
                Email = "member@example.com",
                FullName = "Member User",
                Organization = "Test Organization",
                ManagedById = businessOwnerId
            };

            _mockUserManager
                .Setup(x => x.FindByIdAsync(memberId.ToString()))
                .ReturnsAsync(member);

            _mockUserManager
                .Setup(x => x.UpdateAsync(It.IsAny<User>()))
                .ReturnsAsync(IdentityResult.Failed(new IdentityError { Description = "Update failed" }));

            // Act
            var result = await _userService.RemoveMemberFromOrganizationAsync(businessOwnerId, memberId);

            // Assert
            Assert.False(result.Success);
            Assert.Equal("Failed to remove member from organization.", result.Data);
        }
    }
}
