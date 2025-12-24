using Microsoft.AspNetCore.Identity;
using Moq;
using MSP.Application.Repositories;
using MSP.Application.Services.Implementations.ProjectTask;
using MSP.Application.Services.Interfaces.Notification;
using MSP.Application.Services.Interfaces.ProjectTask;
using MSP.Application.Services.Interfaces.TaskHistory;
using MSP.Domain.Entities;
using MSP.Shared.Common;
using Xunit;

namespace MSP.Tests.Services.TaskServicesTest
{
    public class GetListTaskTest
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

        public GetListTaskTest()
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

        [Fact]
        public async Task GetTasksByProjectIdAsync_WithValidProjectId_ReturnsTaskList()
        {
            // Arrange
            var projectId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var ownerId = Guid.NewGuid();

            var project = new Project
            {
                Id = projectId,
                Name = "Test Project",
                StartDate = DateTime.UtcNow,
                EndDate = DateTime.UtcNow.AddMonths(1),
                Status = "Active",
                OwnerId = ownerId,
                CreatedById = ownerId,
                IsDeleted = false
            };

            var user = new User
            {
                Id = userId,
                Email = "test@example.com",
                FullName = "Test User",
                AvatarUrl = "https://example.com/avatar.jpg"
            };

            var tasks = new List<ProjectTask>
            {
                new ProjectTask
                {
                    Id = Guid.NewGuid(),
                    ProjectId = projectId,
                    UserId = userId,
                    ReviewerId = null,
                    Title = "Task 1",
                    Description = "Description 1",
                    Status = "Todo",
                    StartDate = DateTime.UtcNow,
                    EndDate = DateTime.UtcNow.AddDays(5),
                    IsOverdue = false,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    IsDeleted = false,
                    User = user,
                    Reviewer = null,
                    Milestones = new List<Milestone>()
                },
                new ProjectTask
                {
                    Id = Guid.NewGuid(),
                    ProjectId = projectId,
                    UserId = userId,
                    ReviewerId = null,
                    Title = "Task 2",
                    Description = "Description 2",
                    Status = "InProgress",
                    StartDate = DateTime.UtcNow,
                    EndDate = DateTime.UtcNow.AddDays(7),
                    IsOverdue = false,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    IsDeleted = false,
                    User = user,
                    Reviewer = null,
                    Milestones = new List<Milestone>()
                }
            };

            var pagingRequest = new PagingRequest { PageIndex = 1, PageSize = 10 };

            _mockProjectRepository.Setup(x => x.GetByIdAsync(projectId)).ReturnsAsync(project);
            _mockProjectTaskRepository.Setup(x => x.FindWithIncludePagedAsync(
                It.IsAny<System.Linq.Expressions.Expression<System.Func<ProjectTask, bool>>>(),
                It.IsAny<System.Func<IQueryable<ProjectTask>, IQueryable<ProjectTask>>>(),
                It.IsAny<System.Func<IQueryable<ProjectTask>, IOrderedQueryable<ProjectTask>>>(),
                It.IsAny<int>(),
                It.IsAny<int>(),
                It.IsAny<bool>()
            )).ReturnsAsync(tasks);
            _mockProjectTaskRepository.Setup(x => x.CountAsync(
                It.IsAny<System.Linq.Expressions.Expression<System.Func<ProjectTask, bool>>>()))
                .ReturnsAsync(tasks.Count);

            // Act
            var result = await _projectTaskService.GetTasksByProjectIdAsync(pagingRequest, projectId);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.Success);
            Assert.NotNull(result.Data);
            Assert.Equal(2, result.Data.Items.Count());
            Assert.Equal(2, result.Data.TotalItems);
            Assert.Equal(1, result.Data.PageIndex);
            Assert.Equal(10, result.Data.PageSize);

            var taskList = result.Data.Items.ToList();
            Assert.Equal("Task 1", taskList[0].Title);
            Assert.Equal("Description 1", taskList[0].Description);
            Assert.Equal("Todo", taskList[0].Status);
            Assert.Equal(userId, taskList[0].UserId);
            Assert.NotNull(taskList[0].User);
            Assert.Equal("Test User", taskList[0].User.FullName);

            Assert.Equal("Task 2", taskList[1].Title);
            Assert.Equal("InProgress", taskList[1].Status);

            _mockProjectRepository.Verify(x => x.GetByIdAsync(projectId), Times.Once);
            _mockProjectTaskRepository.Verify(x => x.FindWithIncludePagedAsync(
                It.IsAny<System.Linq.Expressions.Expression<System.Func<ProjectTask, bool>>>(),
                It.IsAny<System.Func<IQueryable<ProjectTask>, IQueryable<ProjectTask>>>(),
                It.IsAny<System.Func<IQueryable<ProjectTask>, IOrderedQueryable<ProjectTask>>>(),
                It.IsAny<int>(),
                It.IsAny<int>(),
                It.IsAny<bool>()), Times.Once);
            _mockProjectTaskRepository.Verify(x => x.CountAsync(
                It.IsAny<System.Linq.Expressions.Expression<System.Func<ProjectTask, bool>>>()), Times.Once);
        }

        [Fact]
        public async Task GetTasksByProjectIdAsync_WithNonExistentProject_ReturnsErrorResponse()
        {
            // Arrange
            var projectId = Guid.NewGuid();
            var pagingRequest = new PagingRequest { PageIndex = 1, PageSize = 10 };

            _mockProjectRepository.Setup(x => x.GetByIdAsync(projectId)).ReturnsAsync((Project)null);

            // Act
            var result = await _projectTaskService.GetTasksByProjectIdAsync(pagingRequest, projectId);

            // Assert
            Assert.False(result.Success);
            Assert.Equal("Project not found", result.Message);
            Assert.Null(result.Data);

            _mockProjectRepository.Verify(x => x.GetByIdAsync(projectId), Times.Once);
            _mockProjectTaskRepository.Verify(x => x.FindWithIncludePagedAsync(
                It.IsAny<System.Linq.Expressions.Expression<System.Func<ProjectTask, bool>>>(),
                It.IsAny<System.Func<IQueryable<ProjectTask>, IQueryable<ProjectTask>>>(),
                It.IsAny<System.Func<IQueryable<ProjectTask>, IOrderedQueryable<ProjectTask>>>(),
                It.IsAny<int>(),
                It.IsAny<int>(),
                It.IsAny<bool>()), Times.Never);
        }

        [Fact]
        public async Task GetTasksByProjectIdAsync_WithNoTasks_ReturnsErrorResponse()
        {
            // Arrange
            var projectId = Guid.NewGuid();
            var ownerId = Guid.NewGuid();

            var project = new Project
            {
                Id = projectId,
                Name = "Test Project",
                StartDate = DateTime.UtcNow,
                EndDate = DateTime.UtcNow.AddMonths(1),
                Status = "Active",
                OwnerId = ownerId,
                CreatedById = ownerId,
                IsDeleted = false
            };

            var pagingRequest = new PagingRequest { PageIndex = 1, PageSize = 10 };

            _mockProjectRepository.Setup(x => x.GetByIdAsync(projectId)).ReturnsAsync(project);
            _mockProjectTaskRepository.Setup(x => x.FindWithIncludePagedAsync(
                It.IsAny<System.Linq.Expressions.Expression<System.Func<ProjectTask, bool>>>(),
                It.IsAny<System.Func<IQueryable<ProjectTask>, IQueryable<ProjectTask>>>(),
                It.IsAny<System.Func<IQueryable<ProjectTask>, IOrderedQueryable<ProjectTask>>>(),
                It.IsAny<int>(),
                It.IsAny<int>(),
                It.IsAny<bool>()
            )).ReturnsAsync(new List<ProjectTask>());

            // Act
            var result = await _projectTaskService.GetTasksByProjectIdAsync(pagingRequest, projectId);

            // Assert
            Assert.False(result.Success);
            Assert.Equal("No tasks found for the project", result.Message);
            Assert.Null(result.Data);

            _mockProjectRepository.Verify(x => x.GetByIdAsync(projectId), Times.Once);
            _mockProjectTaskRepository.Verify(x => x.FindWithIncludePagedAsync(
                It.IsAny<System.Linq.Expressions.Expression<System.Func<ProjectTask, bool>>>(),
                It.IsAny<System.Func<IQueryable<ProjectTask>, IQueryable<ProjectTask>>>(),
                It.IsAny<System.Func<IQueryable<ProjectTask>, IOrderedQueryable<ProjectTask>>>(),
                It.IsAny<int>(),
                It.IsAny<int>(),
                It.IsAny<bool>()), Times.Once);
        }

        [Fact]
        public async Task GetTasksByProjectIdAsync_WithTasksAndReviewers_ReturnsTasksWithReviewerInfo()
        {
            // Arrange
            var projectId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var reviewerId = Guid.NewGuid();
            var ownerId = Guid.NewGuid();

            var project = new Project
            {
                Id = projectId,
                Name = "Test Project",
                StartDate = DateTime.UtcNow,
                EndDate = DateTime.UtcNow.AddMonths(1),
                Status = "Active",
                OwnerId = ownerId,
                CreatedById = ownerId,
                IsDeleted = false
            };

            var user = new User
            {
                Id = userId,
                Email = "test@example.com",
                FullName = "Test User",
                AvatarUrl = "https://example.com/user-avatar.jpg"
            };

            var reviewer = new User
            {
                Id = reviewerId,
                Email = "reviewer@example.com",
                FullName = "Reviewer User",
                AvatarUrl = "https://example.com/reviewer-avatar.jpg"
            };

            var tasks = new List<ProjectTask>
            {
                new ProjectTask
                {
                    Id = Guid.NewGuid(),
                    ProjectId = projectId,
                    UserId = userId,
                    ReviewerId = reviewerId,
                    Title = "Task with Reviewer",
                    Description = "Description",
                    Status = "ReadyToReview",
                    StartDate = DateTime.UtcNow,
                    EndDate = DateTime.UtcNow.AddDays(5),
                    IsOverdue = false,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    IsDeleted = false,
                    User = user,
                    Reviewer = reviewer,
                    Milestones = new List<Milestone>()
                }
            };

            var pagingRequest = new PagingRequest { PageIndex = 1, PageSize = 10 };

            _mockProjectRepository.Setup(x => x.GetByIdAsync(projectId)).ReturnsAsync(project);
            _mockProjectTaskRepository.Setup(x => x.FindWithIncludePagedAsync(
                It.IsAny<System.Linq.Expressions.Expression<System.Func<ProjectTask, bool>>>(),
                It.IsAny<System.Func<IQueryable<ProjectTask>, IQueryable<ProjectTask>>>(),
                It.IsAny<System.Func<IQueryable<ProjectTask>, IOrderedQueryable<ProjectTask>>>(),
                It.IsAny<int>(),
                It.IsAny<int>(),
                It.IsAny<bool>()
            )).ReturnsAsync(tasks);
            _mockProjectTaskRepository.Setup(x => x.CountAsync(
                It.IsAny<System.Linq.Expressions.Expression<System.Func<ProjectTask, bool>>>()))
                .ReturnsAsync(tasks.Count);

            // Act
            var result = await _projectTaskService.GetTasksByProjectIdAsync(pagingRequest, projectId);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.Success);
            Assert.NotNull(result.Data);
            Assert.Single(result.Data.Items);

            var task = result.Data.Items.First();
            Assert.Equal("Task with Reviewer", task.Title);
            Assert.Equal("ReadyToReview", task.Status);
            Assert.Equal(reviewerId, task.ReviewerId);

            Assert.NotNull(task.Reviewer);
            Assert.Equal(reviewerId, task.Reviewer.Id);
            Assert.Equal("Reviewer User", task.Reviewer.FullName);
            Assert.Equal("reviewer@example.com", task.Reviewer.Email);
            Assert.Equal("https://example.com/reviewer-avatar.jpg", task.Reviewer.AvatarUrl);

            Assert.NotNull(task.User);
            Assert.Equal("Test User", task.User.FullName);
        }

        [Fact]
        public async Task GetTasksByProjectIdAsync_WithMilestones_ReturnsMilestoneInfo()
        {
            // Arrange
            var projectId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var milestoneId = Guid.NewGuid();
            var ownerId = Guid.NewGuid();

            var project = new Project
            {
                Id = projectId,
                Name = "Test Project",
                StartDate = DateTime.UtcNow,
                EndDate = DateTime.UtcNow.AddMonths(1),
                Status = "Active",
                OwnerId = ownerId,
                CreatedById = ownerId,
                IsDeleted = false
            };

            var user = new User
            {
                Id = userId,
                Email = "test@example.com",
                FullName = "Test User",
                AvatarUrl = null
            };

            var dueDate = DateTime.UtcNow.AddDays(10);
            var milestone = new Milestone
            {
                Id = milestoneId,
                ProjectId = projectId,
                Name = "Milestone 1",
                DueDate = dueDate,
                IsDeleted = false
            };

            var tasks = new List<ProjectTask>
            {
                new ProjectTask
                {
                    Id = Guid.NewGuid(),
                    ProjectId = projectId,
                    UserId = userId,
                    ReviewerId = null,
                    Title = "Task with Milestone",
                    Description = "Description",
                    Status = "Todo",
                    StartDate = DateTime.UtcNow,
                    EndDate = DateTime.UtcNow.AddDays(5),
                    IsOverdue = false,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    IsDeleted = false,
                    User = user,
                    Reviewer = null,
                    Milestones = new List<Milestone> { milestone }
                }
            };

            var pagingRequest = new PagingRequest { PageIndex = 1, PageSize = 10 };

            _mockProjectRepository.Setup(x => x.GetByIdAsync(projectId)).ReturnsAsync(project);
            _mockProjectTaskRepository.Setup(x => x.FindWithIncludePagedAsync(
                It.IsAny<System.Linq.Expressions.Expression<System.Func<ProjectTask, bool>>>(),
                It.IsAny<System.Func<IQueryable<ProjectTask>, IQueryable<ProjectTask>>>(),
                It.IsAny<System.Func<IQueryable<ProjectTask>, IOrderedQueryable<ProjectTask>>>(),
                It.IsAny<int>(),
                It.IsAny<int>(),
                It.IsAny<bool>()
            )).ReturnsAsync(tasks);
            _mockProjectTaskRepository.Setup(x => x.CountAsync(
                It.IsAny<System.Linq.Expressions.Expression<System.Func<ProjectTask, bool>>>()))
                .ReturnsAsync(tasks.Count);

            // Act
            var result = await _projectTaskService.GetTasksByProjectIdAsync(pagingRequest, projectId);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.Success);
            Assert.NotNull(result.Data);
            Assert.Single(result.Data.Items);

            var task = result.Data.Items.First();
            Assert.Equal("Task with Milestone", task.Title);
            Assert.NotNull(task.Milestones);
            Assert.Single(task.Milestones);

            var returnedMilestone = task.Milestones.First();
            Assert.Equal(milestoneId, returnedMilestone.Id);
            Assert.Equal("Milestone 1", returnedMilestone.Name);
            Assert.Equal(projectId, returnedMilestone.ProjectId);
            Assert.Equal(dueDate, returnedMilestone.DueDate);
        }

        [Fact]
        public async Task GetTasksByProjectIdAsync_ReturnsPagingInfo()
        {
            // Arrange
            var projectId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var ownerId = Guid.NewGuid();

            var project = new Project
            {
                Id = projectId,
                Name = "Test Project",
                StartDate = DateTime.UtcNow,
                EndDate = DateTime.UtcNow.AddMonths(1),
                Status = "Active",
                OwnerId = ownerId,
                CreatedById = ownerId,
                IsDeleted = false
            };

            var user = new User
            {
                Id = userId,
                Email = "test@example.com",
                FullName = "Test User",
                AvatarUrl = null
            };

            var tasks = new List<ProjectTask>
            {
                new ProjectTask
                {
                    Id = Guid.NewGuid(),
                    ProjectId = projectId,
                    UserId = userId,
                    ReviewerId = null,
                    Title = "Task 1",
                    Description = "Description 1",
                    Status = "Todo",
                    StartDate = DateTime.UtcNow,
                    EndDate = DateTime.UtcNow.AddDays(5),
                    IsOverdue = false,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    IsDeleted = false,
                    User = user,
                    Reviewer = null,
                    Milestones = new List<Milestone>()
                }
            };

            var pagingRequest = new PagingRequest { PageIndex = 2, PageSize = 5 };

            _mockProjectRepository.Setup(x => x.GetByIdAsync(projectId)).ReturnsAsync(project);
            _mockProjectTaskRepository.Setup(x => x.FindWithIncludePagedAsync(
                It.IsAny<System.Linq.Expressions.Expression<System.Func<ProjectTask, bool>>>(),
                It.IsAny<System.Func<IQueryable<ProjectTask>, IQueryable<ProjectTask>>>(),
                It.IsAny<System.Func<IQueryable<ProjectTask>, IOrderedQueryable<ProjectTask>>>(),
                It.IsAny<int>(),
                It.IsAny<int>(),
                It.IsAny<bool>()
            )).ReturnsAsync(tasks);
            _mockProjectTaskRepository.Setup(x => x.CountAsync(
                It.IsAny<System.Linq.Expressions.Expression<System.Func<ProjectTask, bool>>>()))
                .ReturnsAsync(15);

            // Act
            var result = await _projectTaskService.GetTasksByProjectIdAsync(pagingRequest, projectId);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.Success);
            Assert.NotNull(result.Data);
            Assert.Single(result.Data.Items);
            Assert.Equal(2, result.Data.PageIndex);
            Assert.Equal(5, result.Data.PageSize);
            Assert.Equal(15, result.Data.TotalItems);

            // Verify the total would be 3 pages (15 items / 5 per page)
            var totalPages = (int)Math.Ceiling(15.0 / 5);
            Assert.Equal(3, totalPages);
        }

        [Fact]
        public async Task GetTasksByProjectIdAsync_WithUnassignedTask_ReturnsTaskWithNullUser()
        {
            // Arrange
            var projectId = Guid.NewGuid();
            var ownerId = Guid.NewGuid();

            var project = new Project
            {
                Id = projectId,
                Name = "Test Project",
                StartDate = DateTime.UtcNow,
                EndDate = DateTime.UtcNow.AddMonths(1),
                Status = "Active",
                OwnerId = ownerId,
                CreatedById = ownerId,
                IsDeleted = false
            };

            var tasks = new List<ProjectTask>
            {
                new ProjectTask
                {
                    Id = Guid.NewGuid(),
                    ProjectId = projectId,
                    UserId = null, // Unassigned task
                    ReviewerId = null,
                    Title = "Unassigned Task",
                    Description = "No one assigned yet",
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
                }
            };

            var pagingRequest = new PagingRequest { PageIndex = 1, PageSize = 10 };

            _mockProjectRepository.Setup(x => x.GetByIdAsync(projectId)).ReturnsAsync(project);
            _mockProjectTaskRepository.Setup(x => x.FindWithIncludePagedAsync(
                It.IsAny<System.Linq.Expressions.Expression<System.Func<ProjectTask, bool>>>(),
                It.IsAny<System.Func<IQueryable<ProjectTask>, IQueryable<ProjectTask>>>(),
                It.IsAny<System.Func<IQueryable<ProjectTask>, IOrderedQueryable<ProjectTask>>>(),
                It.IsAny<int>(),
                It.IsAny<int>(),
                It.IsAny<bool>()
            )).ReturnsAsync(tasks);
            _mockProjectTaskRepository.Setup(x => x.CountAsync(
                It.IsAny<System.Linq.Expressions.Expression<System.Func<ProjectTask, bool>>>()))
                .ReturnsAsync(tasks.Count);

            // Act
            var result = await _projectTaskService.GetTasksByProjectIdAsync(pagingRequest, projectId);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.Success);
            Assert.Single(result.Data.Items);

            var task = result.Data.Items.First();
            Assert.Equal("Unassigned Task", task.Title);
            Assert.Null(task.UserId);
            Assert.Null(task.User);
        }

        [Fact]
        public async Task GetTasksByProjectIdAsync_WithOverdueTasks_ReturnsIsOverdueFlag()
        {
            // Arrange
            var projectId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var ownerId = Guid.NewGuid();

            var project = new Project
            {
                Id = projectId,
                Name = "Test Project",
                StartDate = DateTime.UtcNow.AddMonths(-2),
                EndDate = DateTime.UtcNow.AddMonths(1),
                Status = "Active",
                OwnerId = ownerId,
                CreatedById = ownerId,
                IsDeleted = false
            };

            var user = new User
            {
                Id = userId,
                Email = "test@example.com",
                FullName = "Test User",
                AvatarUrl = null
            };

            var tasks = new List<ProjectTask>
            {
                new ProjectTask
                {
                    Id = Guid.NewGuid(),
                    ProjectId = projectId,
                    UserId = userId,
                    Title = "Overdue Task",
                    Description = "Past deadline",
                    Status = "InProgress",
                    StartDate = DateTime.UtcNow.AddDays(-10),
                    EndDate = DateTime.UtcNow.AddDays(-2),
                    IsOverdue = true,
                    CreatedAt = DateTime.UtcNow.AddDays(-10),
                    UpdatedAt = DateTime.UtcNow,
                    IsDeleted = false,
                    User = user,
                    Reviewer = null,
                    Milestones = new List<Milestone>()
                },
                new ProjectTask
                {
                    Id = Guid.NewGuid(),
                    ProjectId = projectId,
                    UserId = userId,
                    Title = "On Time Task",
                    Description = "Still has time",
                    Status = "InProgress",
                    StartDate = DateTime.UtcNow,
                    EndDate = DateTime.UtcNow.AddDays(5),
                    IsOverdue = false,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    IsDeleted = false,
                    User = user,
                    Reviewer = null,
                    Milestones = new List<Milestone>()
                }
            };

            var pagingRequest = new PagingRequest { PageIndex = 1, PageSize = 10 };

            _mockProjectRepository.Setup(x => x.GetByIdAsync(projectId)).ReturnsAsync(project);
            _mockProjectTaskRepository.Setup(x => x.FindWithIncludePagedAsync(
                It.IsAny<System.Linq.Expressions.Expression<System.Func<ProjectTask, bool>>>(),
                It.IsAny<System.Func<IQueryable<ProjectTask>, IQueryable<ProjectTask>>>(),
                It.IsAny<System.Func<IQueryable<ProjectTask>, IOrderedQueryable<ProjectTask>>>(),
                It.IsAny<int>(),
                It.IsAny<int>(),
                It.IsAny<bool>()
            )).ReturnsAsync(tasks);
            _mockProjectTaskRepository.Setup(x => x.CountAsync(
                It.IsAny<System.Linq.Expressions.Expression<System.Func<ProjectTask, bool>>>()))
                .ReturnsAsync(tasks.Count);

            // Act
            var result = await _projectTaskService.GetTasksByProjectIdAsync(pagingRequest, projectId);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.Success);
            Assert.Equal(2, result.Data.Items.Count());

            var taskList = result.Data.Items.ToList();
            var overdueTask = taskList.First(t => t.Title == "Overdue Task");
            Assert.True(overdueTask.IsOverdue);

            var onTimeTask = taskList.First(t => t.Title == "On Time Task");
            Assert.False(onTimeTask.IsOverdue);
        }
    }
}
