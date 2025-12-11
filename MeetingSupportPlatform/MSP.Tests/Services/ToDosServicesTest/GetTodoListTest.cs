using Microsoft.AspNetCore.Identity;
using Moq;
using MSP.Application.Repositories;
using MSP.Application.Services.Interfaces.Todos;
using MSP.Domain.Entities;
using MSP.Shared.Enums;
using Xunit;
using TodoServiceImpl = MSP.Application.Services.Implementations.Todos.TodoService;

namespace MSP.Tests.Services.ToDosServicesTest
{
    public class GetTodoListTest
    {
        private readonly Mock<ITodoRepository> _mockTodoRepository;
        private readonly Mock<IMeetingRepository> _mockMeetingRepository;
        private readonly Mock<IProjectTaskRepository> _mockProjectTaskRepository;
        private readonly Mock<UserManager<User>> _mockUserManager;
        private readonly ITodoService _todoService;

        public GetTodoListTest()
        {
            _mockTodoRepository = new Mock<ITodoRepository>();
            _mockMeetingRepository = new Mock<IMeetingRepository>();
            _mockProjectTaskRepository = new Mock<IProjectTaskRepository>();
            _mockUserManager = new Mock<UserManager<User>>(
                new Mock<IUserStore<User>>().Object,
                null, null, null, null, null, null, null, null
            );

            _todoService = new TodoServiceImpl(
                _mockUserManager.Object,
                _mockTodoRepository.Object,
                _mockMeetingRepository.Object,
                _mockProjectTaskRepository.Object
            );
        }

        #region GetTodoByMeetingIdAsync Tests

        [Fact]
        public async Task GetTodoByMeetingIdAsync_WithValidMeetingId_ReturnsSuccessResponse()
        {
            // Arrange
            var meetingId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var todoId1 = Guid.NewGuid();
            var todoId2 = Guid.NewGuid();

            var user = new User
            {
                Id = userId,
                Email = "user@example.com",
                FullName = "Test User",
                AvatarUrl = "avatar.png"
            };

            var todos = new List<Todo>
            {
                new Todo
                {
                    Id = todoId1,
                    MeetingId = meetingId,
                    UserId = userId,
                    User = user,
                    Title = "Todo 1",
                    Description = "Description 1",
                    StartDate = DateTime.UtcNow,
                    EndDate = DateTime.UtcNow.AddDays(1),
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    Status = TodoStatus.Generated,
                    ReferencedTasks = new List<ProjectTask>()
                },
                new Todo
                {
                    Id = todoId2,
                    MeetingId = meetingId,
                    UserId = userId,
                    User = user,
                    Title = "Todo 2",
                    Description = "Description 2",
                    StartDate = DateTime.UtcNow.AddDays(1),
                    EndDate = DateTime.UtcNow.AddDays(2),
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    Status = TodoStatus.UnderReview,
                    ReferencedTasks = new List<ProjectTask>()
                }
            };

            _mockTodoRepository
                .Setup(x => x.GetTodoByMeetingId(meetingId))
                .ReturnsAsync(todos);

            // Act
            var result = await _todoService.GetTodoByMeetingIdAsync(meetingId);

            // Assert
            Assert.True(result.Success);
            Assert.Equal("Get todos successfully", result.Message);
            Assert.NotNull(result.Data);
            Assert.Equal(2, result.Data.Count());

            var todoList = result.Data.ToList();
            Assert.Equal(todoId1, todoList[0].Id);
            Assert.Equal("Todo 1", todoList[0].Title);
            Assert.Equal(todoId2, todoList[1].Id);
            Assert.Equal("Todo 2", todoList[1].Title);

            _mockTodoRepository.Verify(x => x.GetTodoByMeetingId(meetingId), Times.Once);
        }

        [Fact]
        public async Task GetTodoByMeetingIdAsync_WithNoTodos_ReturnsEmptyList()
        {
            // Arrange
            var meetingId = Guid.NewGuid();

            _mockTodoRepository
                .Setup(x => x.GetTodoByMeetingId(meetingId))
                .ReturnsAsync(new List<Todo>());

            // Act
            var result = await _todoService.GetTodoByMeetingIdAsync(meetingId);

            // Assert
            Assert.True(result.Success);
            Assert.Equal("Get todos successfully", result.Message);
            Assert.NotNull(result.Data);
            Assert.Empty(result.Data);

            _mockTodoRepository.Verify(x => x.GetTodoByMeetingId(meetingId), Times.Once);
        }

        [Fact]
        public async Task GetTodoByMeetingIdAsync_WithAssignee_MapsAssigneeCorrectly()
        {
            // Arrange
            var meetingId = Guid.NewGuid();
            var userId = Guid.NewGuid();

            var user = new User
            {
                Id = userId,
                Email = "assignee@example.com",
                FullName = "Assignee User",
                AvatarUrl = "avatar_url.png"
            };

            var todos = new List<Todo>
            {
                new Todo
                {
                    Id = Guid.NewGuid(),
                    MeetingId = meetingId,
                    UserId = userId,
                    User = user,
                    Title = "Todo with Assignee",
                    Description = "Description",
                    Status = TodoStatus.Generated,
                    ReferencedTasks = new List<ProjectTask>()
                }
            };

            _mockTodoRepository
                .Setup(x => x.GetTodoByMeetingId(meetingId))
                .ReturnsAsync(todos);

            // Act
            var result = await _todoService.GetTodoByMeetingIdAsync(meetingId);

            // Assert
            Assert.True(result.Success);
            Assert.NotNull(result.Data);
            var todo = result.Data.First();
            Assert.NotNull(todo.Assignee);
            Assert.Equal(userId, todo.Assignee.Id);
            Assert.Equal("assignee@example.com", todo.Assignee.Email);
            Assert.Equal("Assignee User", todo.Assignee.FullName);
            Assert.Equal("avatar_url.png", todo.Assignee.AvatarUrl);
        }

        [Fact]
        public async Task GetTodoByMeetingIdAsync_WithNullAssignee_ReturnsNullAssignee()
        {
            // Arrange
            var meetingId = Guid.NewGuid();

            var todos = new List<Todo>
            {
                new Todo
                {
                    Id = Guid.NewGuid(),
                    MeetingId = meetingId,
                    UserId = null,
                    User = null,
                    Title = "Todo without Assignee",
                    Description = "Description",
                    Status = TodoStatus.Generated,
                    ReferencedTasks = new List<ProjectTask>()
                }
            };

            _mockTodoRepository
                .Setup(x => x.GetTodoByMeetingId(meetingId))
                .ReturnsAsync(todos);

            // Act
            var result = await _todoService.GetTodoByMeetingIdAsync(meetingId);

            // Assert
            Assert.True(result.Success);
            Assert.NotNull(result.Data);
            var todo = result.Data.First();
            Assert.Null(todo.UserId);
            Assert.Null(todo.Assignee);
        }

        [Fact]
        public async Task GetTodoByMeetingIdAsync_WithReferencedTasks_MapsReferencedTasksCorrectly()
        {
            // Arrange
            var meetingId = Guid.NewGuid();
            var taskId1 = Guid.NewGuid();
            var taskId2 = Guid.NewGuid();

            var referencedTasks = new List<ProjectTask>
            {
                new ProjectTask { Id = taskId1, Title = "Task 1" },
                new ProjectTask { Id = taskId2, Title = "Task 2" }
            };

            var todos = new List<Todo>
            {
                new Todo
                {
                    Id = Guid.NewGuid(),
                    MeetingId = meetingId,
                    UserId = null,
                    User = null,
                    Title = "Todo with Referenced Tasks",
                    Description = "Description",
                    Status = TodoStatus.Generated,
                    ReferencedTasks = referencedTasks
                }
            };

            _mockTodoRepository
                .Setup(x => x.GetTodoByMeetingId(meetingId))
                .ReturnsAsync(todos);

            // Act
            var result = await _todoService.GetTodoByMeetingIdAsync(meetingId);

            // Assert
            Assert.True(result.Success);
            Assert.NotNull(result.Data);
            var todo = result.Data.First();
            Assert.Equal(2, todo.ReferencedTasks.Count);
            Assert.Contains(taskId1, todo.ReferencedTasks);
            Assert.Contains(taskId2, todo.ReferencedTasks);
        }

        [Fact]
        public async Task GetTodoByMeetingIdAsync_WithDifferentStatuses_ReturnsAllTodos()
        {
            // Arrange
            var meetingId = Guid.NewGuid();

            var todos = new List<Todo>
            {
                new Todo
                {
                    Id = Guid.NewGuid(),
                    MeetingId = meetingId,
                    Title = "Generated Todo",
                    Status = TodoStatus.Generated,
                    ReferencedTasks = new List<ProjectTask>()
                },
                new Todo
                {
                    Id = Guid.NewGuid(),
                    MeetingId = meetingId,
                    Title = "UnderReview Todo",
                    Status = TodoStatus.UnderReview,
                    ReferencedTasks = new List<ProjectTask>()
                },
                new Todo
                {
                    Id = Guid.NewGuid(),
                    MeetingId = meetingId,
                    Title = "ConvertedToTask Todo",
                    Status = TodoStatus.ConvertedToTask,
                    ReferencedTasks = new List<ProjectTask>()
                }
            };

            _mockTodoRepository
                .Setup(x => x.GetTodoByMeetingId(meetingId))
                .ReturnsAsync(todos);

            // Act
            var result = await _todoService.GetTodoByMeetingIdAsync(meetingId);

            // Assert
            Assert.True(result.Success);
            Assert.NotNull(result.Data);
            Assert.Equal(3, result.Data.Count());

            var todoList = result.Data.ToList();
            Assert.Contains(todoList, t => t.Status == TodoStatus.Generated);
            Assert.Contains(todoList, t => t.Status == TodoStatus.UnderReview);
            Assert.Contains(todoList, t => t.Status == TodoStatus.ConvertedToTask);
        }

        [Fact]
        public async Task GetTodoByMeetingIdAsync_WithNullDates_ReturnsNullDates()
        {
            // Arrange
            var meetingId = Guid.NewGuid();

            var todos = new List<Todo>
            {
                new Todo
                {
                    Id = Guid.NewGuid(),
                    MeetingId = meetingId,
                    Title = "Todo without Dates",
                    Description = null,
                    StartDate = null,
                    EndDate = null,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    Status = TodoStatus.Generated,
                    ReferencedTasks = new List<ProjectTask>()
                }
            };

            _mockTodoRepository
                .Setup(x => x.GetTodoByMeetingId(meetingId))
                .ReturnsAsync(todos);

            // Act
            var result = await _todoService.GetTodoByMeetingIdAsync(meetingId);

            // Assert
            Assert.True(result.Success);
            Assert.NotNull(result.Data);
            var todo = result.Data.First();
            Assert.Null(todo.Description);
            Assert.Null(todo.StartDate);
            Assert.Null(todo.EndDate);
        }

        [Fact]
        public async Task GetTodoByMeetingIdAsync_MapsAllFieldsCorrectly()
        {
            // Arrange
            var meetingId = Guid.NewGuid();
            var todoId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var startDate = DateTime.UtcNow;
            var endDate = DateTime.UtcNow.AddDays(7);
            var createdAt = DateTime.UtcNow.AddDays(-1);
            var updatedAt = DateTime.UtcNow;

            var user = new User
            {
                Id = userId,
                Email = "user@example.com",
                FullName = "User Name",
                AvatarUrl = "avatar.png"
            };

            var todos = new List<Todo>
            {
                new Todo
                {
                    Id = todoId,
                    MeetingId = meetingId,
                    UserId = userId,
                    User = user,
                    Title = "Complete Task",
                    Description = "Full description here",
                    StartDate = startDate,
                    EndDate = endDate,
                    CreatedAt = createdAt,
                    UpdatedAt = updatedAt,
                    Status = TodoStatus.UnderReview,
                    ReferencedTasks = new List<ProjectTask>()
                }
            };

            _mockTodoRepository
                .Setup(x => x.GetTodoByMeetingId(meetingId))
                .ReturnsAsync(todos);

            // Act
            var result = await _todoService.GetTodoByMeetingIdAsync(meetingId);

            // Assert
            Assert.True(result.Success);
            Assert.NotNull(result.Data);
            var todo = result.Data.First();

            Assert.Equal(todoId, todo.Id);
            Assert.Equal(meetingId, todo.MeetingId);
            Assert.Equal(userId, todo.UserId);
            Assert.Equal("Complete Task", todo.Title);
            Assert.Equal("Full description here", todo.Description);
            Assert.Equal(startDate, todo.StartDate);
            Assert.Equal(endDate, todo.EndDate);
            Assert.Equal(createdAt, todo.CreatedAt);
            Assert.Equal(updatedAt, todo.UpdatedAt);
            Assert.Equal(TodoStatus.UnderReview, todo.Status);
        }

        [Fact]
        public async Task GetTodoByMeetingIdAsync_WithManyTodos_ReturnsAllTodos()
        {
            // Arrange
            var meetingId = Guid.NewGuid();
            var todos = new List<Todo>();

            for (int i = 1; i <= 20; i++)
            {
                todos.Add(new Todo
                {
                    Id = Guid.NewGuid(),
                    MeetingId = meetingId,
                    Title = $"Todo {i}",
                    Description = $"Description {i}",
                    Status = TodoStatus.Generated,
                    ReferencedTasks = new List<ProjectTask>()
                });
            }

            _mockTodoRepository
                .Setup(x => x.GetTodoByMeetingId(meetingId))
                .ReturnsAsync(todos);

            // Act
            var result = await _todoService.GetTodoByMeetingIdAsync(meetingId);

            // Assert
            Assert.True(result.Success);
            Assert.NotNull(result.Data);
            Assert.Equal(20, result.Data.Count());

            // Verify all titles are present
            for (int i = 1; i <= 20; i++)
            {
                Assert.Contains(result.Data, t => t.Title == $"Todo {i}");
            }
        }

        [Fact]
        public async Task GetTodoByMeetingIdAsync_VerifiesRepositoryCalledOnce()
        {
            // Arrange
            var meetingId = Guid.NewGuid();

            _mockTodoRepository
                .Setup(x => x.GetTodoByMeetingId(meetingId))
                .ReturnsAsync(new List<Todo>());

            // Act
            await _todoService.GetTodoByMeetingIdAsync(meetingId);

            // Assert
            _mockTodoRepository.Verify(x => x.GetTodoByMeetingId(meetingId), Times.Once);
            _mockTodoRepository.Verify(x => x.GetTodoByMeetingId(It.IsAny<Guid>()), Times.Once);
        }

        #endregion
    }
}
