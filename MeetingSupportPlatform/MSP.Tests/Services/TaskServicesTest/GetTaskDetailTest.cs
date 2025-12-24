using Microsoft.AspNetCore.Identity;
using Moq;
using MSP.Application.Repositories;
using MSP.Application.Services.Implementations.ProjectTask;
using MSP.Application.Services.Interfaces.Notification;
using MSP.Application.Services.Interfaces.ProjectTask;
using MSP.Application.Services.Interfaces.TaskHistory;
using MSP.Domain.Entities;
using Xunit;

namespace MSP.Tests.Services.TaskServicesTest
{
    public class GetTaskDetailTest
    {
        private readonly Mock<IProjectTaskRepository> _mockProjectTaskRepository;
        private readonly Mock<IProjectRepository> _mockProjectRepository;
        private readonly Mock<IMilestoneRepository> _mockMilestoneRepository;
        private readonly Mock<ITodoRepository> _mockTodoRepository;
        private readonly Mock<ITaskHistoryService> _mockTaskHistoryService;
        private readonly Mock<UserManager<User>> _mockUserManager;
        private readonly Mock<INotificationService> _mockNotificationService;
        private readonly Mock<IProjectMemberRepository> _mockProjectMemberRepository;
        private readonly IProjectTaskService _projectTaskService;

        public GetTaskDetailTest()
        {
            _mockProjectTaskRepository = new Mock<IProjectTaskRepository>();
            _mockProjectRepository = new Mock<IProjectRepository>();
            _mockMilestoneRepository = new Mock<IMilestoneRepository>();
            _mockTodoRepository = new Mock<ITodoRepository>();
            _mockTaskHistoryService = new Mock<ITaskHistoryService>();

            _mockUserManager = new Mock<UserManager<User>>(
                new Mock<IUserStore<User>>().Object,
                null, null, null, null, null, null, null, null
            );

            _mockNotificationService = new Mock<INotificationService>();
            _mockProjectMemberRepository = new Mock<IProjectMemberRepository>();

            _projectTaskService = new ProjectTaskService(
                _mockProjectTaskRepository.Object,
                _mockProjectRepository.Object,
                _mockMilestoneRepository.Object,
                _mockUserManager.Object,
                _mockTodoRepository.Object,
                _mockTaskHistoryService.Object,
                _mockNotificationService.Object,
                _mockProjectMemberRepository.Object
            );
        }

        private User CreateValidUser(Guid id, string email = "test@example.com", string fullName = "Test User", string? avatarUrl = null)
        {
            return new User
            {
                Id = id,
                Email = email,
                FullName = fullName,
                AvatarUrl = avatarUrl
            };
        }

        private ProjectTask CreateValidTask(Guid taskId, Guid projectId, Guid userId, Guid? reviewerId = null)
        {
            return new ProjectTask
            {
                Id = taskId,
                ProjectId = projectId,
                UserId = userId,
                ReviewerId = reviewerId,
                Title = "Test Task",
                Description = "Test Description",
                Status = "Todo",
                StartDate = DateTime.UtcNow,
                EndDate = DateTime.UtcNow.AddDays(5),
                IsOverdue = false,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                IsDeleted = false,
                User = CreateValidUser(userId),
                Reviewer = reviewerId.HasValue ? CreateValidUser(reviewerId.Value, "reviewer@example.com", "Reviewer") : null,
                Milestones = new List<Milestone>()
            };
        }

        [Fact]
        public async Task GetTaskByIdAsync_WithValidId_ReturnsSuccessResponse()
        {
            // Arrange
            var taskId = Guid.NewGuid();
            var projectId = Guid.NewGuid();
            var userId = Guid.NewGuid();

            var task = CreateValidTask(taskId, projectId, userId);

            _mockProjectTaskRepository
                .Setup(x => x.GetTaskByIdAsync(taskId))
                .ReturnsAsync(task);

            // Act
            var result = await _projectTaskService.GetTaskByIdAsync(taskId);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.Success);
            Assert.Equal("Task retrieved successfully", result.Message);
            Assert.NotNull(result.Data);
            Assert.Equal(taskId, result.Data.Id);
            Assert.Equal("Test Task", result.Data.Title);
            Assert.Equal("Test Description", result.Data.Description);
            Assert.Equal("Todo", result.Data.Status);
            Assert.Equal(projectId, result.Data.ProjectId);
            Assert.Equal(userId, result.Data.UserId);
            Assert.False(result.Data.IsOverdue);
            Assert.NotNull(result.Data.StartDate);
            Assert.NotNull(result.Data.EndDate);
            Assert.NotNull(result.Data.CreatedAt);

            _mockProjectTaskRepository.Verify(x => x.GetTaskByIdAsync(taskId), Times.Once);
        }

        [Fact]
        public async Task GetTaskByIdAsync_WithNonExistentId_ReturnsErrorResponse()
        {
            // Arrange
            var taskId = Guid.NewGuid();

            _mockProjectTaskRepository
                .Setup(x => x.GetTaskByIdAsync(taskId))
                .ReturnsAsync((ProjectTask)null);

            // Act
            var result = await _projectTaskService.GetTaskByIdAsync(taskId);

            // Assert
            Assert.NotNull(result);
            Assert.False(result.Success);
            Assert.Equal("Task not found", result.Message);
            Assert.Null(result.Data);

            _mockProjectTaskRepository.Verify(x => x.GetTaskByIdAsync(taskId), Times.Once);
        }

        [Fact]
        public async Task GetTaskByIdAsync_IncludesUserDetails()
        {
            // Arrange
            var taskId = Guid.NewGuid();
            var projectId = Guid.NewGuid();
            var userId = Guid.NewGuid();

            var user = CreateValidUser(userId, "assignee@example.com", "Assignee Name", "https://example.com/avatar.jpg");
            var task = CreateValidTask(taskId, projectId, userId);
            task.User = user;

            _mockProjectTaskRepository
                .Setup(x => x.GetTaskByIdAsync(taskId))
                .ReturnsAsync(task);

            // Act
            var result = await _projectTaskService.GetTaskByIdAsync(taskId);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.Success);
            Assert.NotNull(result.Data);
            Assert.NotNull(result.Data.User);
            Assert.Equal(userId, result.Data.User.Id);
            Assert.Equal("assignee@example.com", result.Data.User.Email);
            Assert.Equal("Assignee Name", result.Data.User.FullName);
            Assert.Equal("https://example.com/avatar.jpg", result.Data.User.AvatarUrl);
        }

        [Fact]
        public async Task GetTaskByIdAsync_IncludesReviewerDetails()
        {
            // Arrange
            var taskId = Guid.NewGuid();
            var projectId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var reviewerId = Guid.NewGuid();

            var reviewer = CreateValidUser(reviewerId, "reviewer@example.com", "Reviewer Name", "https://example.com/reviewer-avatar.jpg");
            var task = CreateValidTask(taskId, projectId, userId, reviewerId);
            task.Reviewer = reviewer;

            _mockProjectTaskRepository
                .Setup(x => x.GetTaskByIdAsync(taskId))
                .ReturnsAsync(task);

            // Act
            var result = await _projectTaskService.GetTaskByIdAsync(taskId);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.Success);
            Assert.NotNull(result.Data);
            Assert.NotNull(result.Data.Reviewer);
            Assert.Equal(reviewerId, result.Data.Reviewer.Id);
            Assert.Equal("reviewer@example.com", result.Data.Reviewer.Email);
            Assert.Equal("Reviewer Name", result.Data.Reviewer.FullName);
            Assert.Equal("https://example.com/reviewer-avatar.jpg", result.Data.Reviewer.AvatarUrl);
            Assert.Equal(reviewerId, result.Data.ReviewerId);
        }

        [Fact]
        public async Task GetTaskByIdAsync_IncludesMilestones()
        {
            // Arrange
            var taskId = Guid.NewGuid();
            var projectId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var milestoneId = Guid.NewGuid();
            var dueDate = DateTime.UtcNow.AddMonths(1);

            var milestone = new Milestone
            {
                Id = milestoneId,
                ProjectId = projectId,
                Name = "Test Milestone",
                DueDate = dueDate,
                IsDeleted = false
            };

            var task = CreateValidTask(taskId, projectId, userId);
            task.Milestones = new List<Milestone> { milestone };

            _mockProjectTaskRepository
                .Setup(x => x.GetTaskByIdAsync(taskId))
                .ReturnsAsync(task);

            // Act
            var result = await _projectTaskService.GetTaskByIdAsync(taskId);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.Success);
            Assert.NotNull(result.Data);
            Assert.NotNull(result.Data.Milestones);
            Assert.Single(result.Data.Milestones);

            var returnedMilestone = result.Data.Milestones[0];
            Assert.Equal(milestoneId, returnedMilestone.Id);
            Assert.Equal("Test Milestone", returnedMilestone.Name);
            Assert.Equal(projectId, returnedMilestone.ProjectId);
            Assert.Equal(dueDate, returnedMilestone.DueDate);
        }

        [Fact]
        public async Task GetTaskByIdAsync_WithMultipleMilestones_ReturnsAllMilestones()
        {
            // Arrange
            var taskId = Guid.NewGuid();
            var projectId = Guid.NewGuid();
            var userId = Guid.NewGuid();

            var milestones = new List<Milestone>
            {
                new Milestone
                {
                    Id = Guid.NewGuid(),
                    ProjectId = projectId,
                    Name = "Milestone 1",
                    DueDate = DateTime.UtcNow.AddDays(10),
                    IsDeleted = false
                },
                new Milestone
                {
                    Id = Guid.NewGuid(),
                    ProjectId = projectId,
                    Name = "Milestone 2",
                    DueDate = DateTime.UtcNow.AddDays(20),
                    IsDeleted = false
                },
                new Milestone
                {
                    Id = Guid.NewGuid(),
                    ProjectId = projectId,
                    Name = "Milestone 3",
                    DueDate = DateTime.UtcNow.AddDays(30),
                    IsDeleted = false
                }
            };

            var task = CreateValidTask(taskId, projectId, userId);
            task.Milestones = milestones;

            _mockProjectTaskRepository
                .Setup(x => x.GetTaskByIdAsync(taskId))
                .ReturnsAsync(task);

            // Act
            var result = await _projectTaskService.GetTaskByIdAsync(taskId);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.Success);
            Assert.NotNull(result.Data.Milestones);
            Assert.Equal(3, result.Data.Milestones.Length);
            Assert.Contains(result.Data.Milestones, m => m.Name == "Milestone 1");
            Assert.Contains(result.Data.Milestones, m => m.Name == "Milestone 2");
            Assert.Contains(result.Data.Milestones, m => m.Name == "Milestone 3");
        }

        [Fact]
        public async Task GetTaskByIdAsync_WithoutAssignee_ReturnsNullUser()
        {
            // Arrange
            var taskId = Guid.NewGuid();
            var projectId = Guid.NewGuid();

            var task = new ProjectTask
            {
                Id = taskId,
                ProjectId = projectId,
                UserId = null,
                ReviewerId = null,
                Title = "Unassigned Task",
                Description = "Test Description",
                Status = "Todo",
                StartDate = DateTime.UtcNow,
                EndDate = DateTime.UtcNow.AddDays(5),
                IsOverdue = false,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                IsDeleted = false,
                User = null,
                Reviewer = null,
                Milestones = new List<Milestone>()
            };

            _mockProjectTaskRepository
                .Setup(x => x.GetTaskByIdAsync(taskId))
                .ReturnsAsync(task);

            // Act
            var result = await _projectTaskService.GetTaskByIdAsync(taskId);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.Success);
            Assert.NotNull(result.Data);
            Assert.Null(result.Data.UserId);
            Assert.Null(result.Data.User);
            Assert.Equal("Unassigned Task", result.Data.Title);
        }

        [Fact]
        public async Task GetTaskByIdAsync_WithoutReviewer_ReturnsNullReviewer()
        {
            // Arrange
            var taskId = Guid.NewGuid();
            var projectId = Guid.NewGuid();
            var userId = Guid.NewGuid();

            var task = CreateValidTask(taskId, projectId, userId, null);
            task.ReviewerId = null;
            task.Reviewer = null;

            _mockProjectTaskRepository
                .Setup(x => x.GetTaskByIdAsync(taskId))
                .ReturnsAsync(task);

            // Act
            var result = await _projectTaskService.GetTaskByIdAsync(taskId);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.Success);
            Assert.NotNull(result.Data);
            Assert.Null(result.Data.ReviewerId);
            Assert.Null(result.Data.Reviewer);
        }

        [Fact]
        public async Task GetTaskByIdAsync_ReturnsCompleteTaskDetails()
        {
            // Arrange
            var taskId = Guid.NewGuid();
            var projectId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var reviewerId = Guid.NewGuid();
            var createdAt = DateTime.UtcNow.AddDays(-10);
            var updatedAt = DateTime.UtcNow.AddDays(-2);
            var startDate = DateTime.UtcNow.AddDays(-5);
            var endDate = DateTime.UtcNow.AddDays(-1);

            var user = CreateValidUser(userId, "user@example.com", "Task User", "https://example.com/user.jpg");
            var reviewer = CreateValidUser(reviewerId, "reviewer@example.com", "Task Reviewer", "https://example.com/reviewer.jpg");

            var task = new ProjectTask
            {
                Id = taskId,
                ProjectId = projectId,
                UserId = userId,
                ReviewerId = reviewerId,
                Title = "Complete Task",
                Description = "Complete Description",
                Status = "InProgress",
                StartDate = startDate,
                EndDate = endDate,
                IsOverdue = true,
                CreatedAt = createdAt,
                UpdatedAt = updatedAt,
                IsDeleted = false,
                User = user,
                Reviewer = reviewer,
                Milestones = new List<Milestone>()
            };

            _mockProjectTaskRepository
                .Setup(x => x.GetTaskByIdAsync(taskId))
                .ReturnsAsync(task);

            // Act
            var result = await _projectTaskService.GetTaskByIdAsync(taskId);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.Success);
            Assert.NotNull(result.Data);

            // Task properties
            Assert.Equal(taskId, result.Data.Id);
            Assert.Equal(projectId, result.Data.ProjectId);
            Assert.Equal("Complete Task", result.Data.Title);
            Assert.Equal("Complete Description", result.Data.Description);
            Assert.Equal("InProgress", result.Data.Status);
            Assert.True(result.Data.IsOverdue);
            Assert.Equal(startDate, result.Data.StartDate);
            Assert.Equal(endDate, result.Data.EndDate);
            Assert.Equal(createdAt, result.Data.CreatedAt);
            Assert.Equal(updatedAt, result.Data.UpdatedAt);

            // User details
            Assert.NotNull(result.Data.User);
            Assert.Equal(userId, result.Data.UserId);
            Assert.Equal("user@example.com", result.Data.User.Email);
            Assert.Equal("Task User", result.Data.User.FullName);
            Assert.Equal("https://example.com/user.jpg", result.Data.User.AvatarUrl);

            // Reviewer details
            Assert.NotNull(result.Data.Reviewer);
            Assert.Equal(reviewerId, result.Data.ReviewerId);
            Assert.Equal("reviewer@example.com", result.Data.Reviewer.Email);
            Assert.Equal("Task Reviewer", result.Data.Reviewer.FullName);
            Assert.Equal("https://example.com/reviewer.jpg", result.Data.Reviewer.AvatarUrl);
        }

        [Fact]
        public async Task GetTaskByIdAsync_WithEmptyMilestoneList_ReturnsEmptyArray()
        {
            // Arrange
            var taskId = Guid.NewGuid();
            var projectId = Guid.NewGuid();
            var userId = Guid.NewGuid();

            var task = CreateValidTask(taskId, projectId, userId);
            task.Milestones = new List<Milestone>();

            _mockProjectTaskRepository
                .Setup(x => x.GetTaskByIdAsync(taskId))
                .ReturnsAsync(task);

            // Act
            var result = await _projectTaskService.GetTaskByIdAsync(taskId);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.Success);
            Assert.NotNull(result.Data);
            Assert.NotNull(result.Data.Milestones);
            Assert.Empty(result.Data.Milestones);
        }

        [Fact]
        public async Task GetTaskByIdAsync_WithDifferentStatuses_ReturnsCorrectStatus()
        {
            // Arrange
            var statuses = new[] { "Todo", "InProgress", "ReadyToReview", "Done", "Cancelled" };

            foreach (var status in statuses)
            {
                var taskId = Guid.NewGuid();
                var projectId = Guid.NewGuid();
                var userId = Guid.NewGuid();

                var task = CreateValidTask(taskId, projectId, userId);
                task.Status = status;

                _mockProjectTaskRepository
                    .Setup(x => x.GetTaskByIdAsync(taskId))
                    .ReturnsAsync(task);

                // Act
                var result = await _projectTaskService.GetTaskByIdAsync(taskId);

                // Assert
                Assert.NotNull(result);
                Assert.True(result.Success);
                Assert.Equal(status, result.Data.Status);
            }
        }

        [Fact]
        public async Task GetTaskByIdAsync_WithOverdueTask_ReturnsIsOverdueTrue()
        {
            // Arrange
            var taskId = Guid.NewGuid();
            var projectId = Guid.NewGuid();
            var userId = Guid.NewGuid();

            var task = CreateValidTask(taskId, projectId, userId);
            task.EndDate = DateTime.UtcNow.AddDays(-5);
            task.IsOverdue = true;
            task.Status = "InProgress";

            _mockProjectTaskRepository
                .Setup(x => x.GetTaskByIdAsync(taskId))
                .ReturnsAsync(task);

            // Act
            var result = await _projectTaskService.GetTaskByIdAsync(taskId);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.Success);
            Assert.NotNull(result.Data);
            Assert.True(result.Data.IsOverdue);
            Assert.True(result.Data.EndDate < DateTime.UtcNow);
        }

        [Fact]
        public async Task GetTaskByIdAsync_CallsRepositoryOnce()
        {
            // Arrange
            var taskId = Guid.NewGuid();
            var projectId = Guid.NewGuid();
            var userId = Guid.NewGuid();

            var task = CreateValidTask(taskId, projectId, userId);

            _mockProjectTaskRepository
                .Setup(x => x.GetTaskByIdAsync(taskId))
                .ReturnsAsync(task);

            // Act
            await _projectTaskService.GetTaskByIdAsync(taskId);

            // Assert
            _mockProjectTaskRepository.Verify(x => x.GetTaskByIdAsync(taskId), Times.Once);
            _mockProjectTaskRepository.VerifyNoOtherCalls();
        }
    }
}
