using Microsoft.AspNetCore.Identity;
using Moq;
using MSP.Application.Models.Requests.Todo;
using MSP.Application.Repositories;
using MSP.Application.Services.Interfaces.Todos;
using MSP.Domain.Entities;
using MSP.Shared.Enums;
using Xunit;
using TodoServiceImpl = MSP.Application.Services.Implementations.Todos.TodoService;

namespace MSP.Tests.Services.ToDosServicesTest
{
    public class EditTodoListTest
    {
        private readonly Mock<ITodoRepository> _mockTodoRepository;
        private readonly Mock<IMeetingRepository> _mockMeetingRepository;
        private readonly Mock<IProjectTaskRepository> _mockProjectTaskRepository;
        private readonly Mock<UserManager<User>> _mockUserManager;
        private readonly ITodoService _todoService;

        public EditTodoListTest()
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

        #region UpdateTodoAsync Tests

        [Fact]
        public async Task UpdateTodoAsync_WithValidRequest_ReturnsSuccessResponse()
        {
            // Arrange
            var todoId = Guid.NewGuid();
            var meetingId = Guid.NewGuid();
            var userId = Guid.NewGuid();

            var existingTodo = new Todo
            {
                Id = todoId,
                MeetingId = meetingId,
                UserId = null,
                Title = "Original Title",
                Description = "Original Description",
                StartDate = DateTime.UtcNow,
                EndDate = DateTime.UtcNow.AddDays(1),
                Status = TodoStatus.Generated,
                CreatedAt = DateTime.UtcNow.AddDays(-1),
                UpdatedAt = DateTime.UtcNow.AddDays(-1),
                ReferencedTasks = new List<ProjectTask>()
            };

            var request = new UpdateTodoRequest
            {
                AssigneeId = userId,
                Title = "Updated Title",
                Description = "Updated Description",
                StartDate = DateTime.UtcNow.AddDays(1),
                EndDate = DateTime.UtcNow.AddDays(3)
            };

            var user = new User
            {
                Id = userId,
                Email = "user@example.com",
                FullName = "Test User",
                AvatarUrl = "avatar.png"
            };

            _mockTodoRepository
                .Setup(x => x.GetByIdAsync(todoId))
                .ReturnsAsync(existingTodo);

            _mockTodoRepository
                .Setup(x => x.UpdateAsync(It.IsAny<Todo>()))
                .Returns(Task.CompletedTask);

            _mockTodoRepository
                .Setup(x => x.SaveChangesAsync())
                .Returns(Task.CompletedTask);

            // Act
            var result = await _todoService.UpdateTodoAsync(todoId, request);

            // Assert
            Assert.True(result.Success);
            Assert.Equal("Update todo successfully", result.Message);
            Assert.NotNull(result.Data);
            Assert.Equal(todoId, result.Data.Id);
            Assert.Equal("Updated Title", result.Data.Title);
            Assert.Equal("Updated Description", result.Data.Description);
            Assert.Equal(userId, result.Data.UserId);

            _mockTodoRepository.Verify(x => x.GetByIdAsync(todoId), Times.Once);
            _mockTodoRepository.Verify(x => x.UpdateAsync(It.IsAny<Todo>()), Times.Once);
            _mockTodoRepository.Verify(x => x.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task UpdateTodoAsync_WithNonExistentTodo_ReturnsErrorResponse()
        {
            // Arrange
            var todoId = Guid.NewGuid();
            var request = new UpdateTodoRequest
            {
                Title = "Updated Title"
            };

            _mockTodoRepository
                .Setup(x => x.GetByIdAsync(todoId))
                .ReturnsAsync((Todo?)null);

            // Act
            var result = await _todoService.UpdateTodoAsync(todoId, request);

            // Assert
            Assert.False(result.Success);
            Assert.Equal("Todo not found", result.Message);
            Assert.Null(result.Data);

            _mockTodoRepository.Verify(x => x.GetByIdAsync(todoId), Times.Once);
            _mockTodoRepository.Verify(x => x.UpdateAsync(It.IsAny<Todo>()), Times.Never);
            _mockTodoRepository.Verify(x => x.SaveChangesAsync(), Times.Never);
        }

        [Fact]
        public async Task UpdateTodoAsync_WithEmptyTitle_ReturnsErrorResponse()
        {
            // Arrange
            var todoId = Guid.NewGuid();
            var meetingId = Guid.NewGuid();

            var existingTodo = new Todo
            {
                Id = todoId,
                MeetingId = meetingId,
                Title = "Original Title",
                Status = TodoStatus.Generated,
                ReferencedTasks = new List<ProjectTask>()
            };

            var request = new UpdateTodoRequest
            {
                Title = "",
                Description = "Updated Description"
            };

            _mockTodoRepository
                .Setup(x => x.GetByIdAsync(todoId))
                .ReturnsAsync(existingTodo);

            // Act
            var result = await _todoService.UpdateTodoAsync(todoId, request);

            // Assert
            Assert.False(result.Success);
            Assert.Equal("Todo Name cannot be empty!", result.Message);
            Assert.Null(result.Data);

            _mockTodoRepository.Verify(x => x.UpdateAsync(It.IsAny<Todo>()), Times.Never);
        }

        [Fact]
        public async Task UpdateTodoAsync_WithWhitespaceTitle_ReturnsErrorResponse()
        {
            // Arrange
            var todoId = Guid.NewGuid();
            var meetingId = Guid.NewGuid();

            var existingTodo = new Todo
            {
                Id = todoId,
                MeetingId = meetingId,
                Title = "Original Title",
                Status = TodoStatus.Generated,
                ReferencedTasks = new List<ProjectTask>()
            };

            var request = new UpdateTodoRequest
            {
                Title = "   ",
                Description = "Updated Description"
            };

            _mockTodoRepository
                .Setup(x => x.GetByIdAsync(todoId))
                .ReturnsAsync(existingTodo);

            // Act
            var result = await _todoService.UpdateTodoAsync(todoId, request);

            // Assert
            Assert.False(result.Success);
            Assert.Equal("Todo Name cannot be empty!", result.Message);
            Assert.Null(result.Data);
        }

        [Fact]
        public async Task UpdateTodoAsync_SetsStatusToUnderReview()
        {
            // Arrange
            var todoId = Guid.NewGuid();
            var meetingId = Guid.NewGuid();

            var existingTodo = new Todo
            {
                Id = todoId,
                MeetingId = meetingId,
                Title = "Original Title",
                Status = TodoStatus.Generated,
                ReferencedTasks = new List<ProjectTask>()
            };

            var request = new UpdateTodoRequest
            {
                Title = "Updated Title"
            };

            Todo? capturedTodo = null;

            _mockTodoRepository
                .Setup(x => x.GetByIdAsync(todoId))
                .ReturnsAsync(existingTodo);

            _mockTodoRepository
                .Setup(x => x.UpdateAsync(It.IsAny<Todo>()))
                .Callback<Todo>(t => capturedTodo = t)
                .Returns(Task.CompletedTask);

            _mockTodoRepository
                .Setup(x => x.SaveChangesAsync())
                .Returns(Task.CompletedTask);

            // Act
            var result = await _todoService.UpdateTodoAsync(todoId, request);

            // Assert
            Assert.True(result.Success);
            Assert.NotNull(capturedTodo);
            Assert.Equal(TodoStatus.UnderReview, capturedTodo.Status);
            Assert.Equal(TodoStatus.UnderReview, result.Data.Status);
        }

        [Fact]
        public async Task UpdateTodoAsync_UpdatesUpdatedAtTimestamp()
        {
            // Arrange
            var todoId = Guid.NewGuid();
            var meetingId = Guid.NewGuid();
            var beforeTest = DateTime.UtcNow;

            var existingTodo = new Todo
            {
                Id = todoId,
                MeetingId = meetingId,
                Title = "Original Title",
                Status = TodoStatus.Generated,
                UpdatedAt = DateTime.UtcNow.AddDays(-1),
                ReferencedTasks = new List<ProjectTask>()
            };

            var request = new UpdateTodoRequest
            {
                Title = "Updated Title"
            };

            Todo? capturedTodo = null;

            _mockTodoRepository
                .Setup(x => x.GetByIdAsync(todoId))
                .ReturnsAsync(existingTodo);

            _mockTodoRepository
                .Setup(x => x.UpdateAsync(It.IsAny<Todo>()))
                .Callback<Todo>(t => capturedTodo = t)
                .Returns(Task.CompletedTask);

            _mockTodoRepository
                .Setup(x => x.SaveChangesAsync())
                .Returns(Task.CompletedTask);

            // Act
            await _todoService.UpdateTodoAsync(todoId, request);
            var afterTest = DateTime.UtcNow;

            // Assert
            Assert.NotNull(capturedTodo);
            Assert.True(capturedTodo.UpdatedAt >= beforeTest && capturedTodo.UpdatedAt <= afterTest);
        }

        [Fact]
        public async Task UpdateTodoAsync_WithAssigneeId_UpdatesUserId()
        {
            // Arrange
            var todoId = Guid.NewGuid();
            var meetingId = Guid.NewGuid();
            var newAssigneeId = Guid.NewGuid();

            var existingTodo = new Todo
            {
                Id = todoId,
                MeetingId = meetingId,
                UserId = null,
                Title = "Original Title",
                Status = TodoStatus.Generated,
                ReferencedTasks = new List<ProjectTask>()
            };

            var request = new UpdateTodoRequest
            {
                AssigneeId = newAssigneeId,
                Title = "Updated Title"
            };

            Todo? capturedTodo = null;

            _mockTodoRepository
                .Setup(x => x.GetByIdAsync(todoId))
                .ReturnsAsync(existingTodo);

            _mockTodoRepository
                .Setup(x => x.UpdateAsync(It.IsAny<Todo>()))
                .Callback<Todo>(t => capturedTodo = t)
                .Returns(Task.CompletedTask);

            _mockTodoRepository
                .Setup(x => x.SaveChangesAsync())
                .Returns(Task.CompletedTask);

            // Act
            var result = await _todoService.UpdateTodoAsync(todoId, request);

            // Assert
            Assert.True(result.Success);
            Assert.NotNull(capturedTodo);
            Assert.Equal(newAssigneeId, capturedTodo.UserId);
            Assert.Equal(newAssigneeId, result.Data.UserId);
        }

        [Fact]
        public async Task UpdateTodoAsync_WithNullAssigneeId_SetsUserIdToNull()
        {
            // Arrange
            var todoId = Guid.NewGuid();
            var meetingId = Guid.NewGuid();
            var existingUserId = Guid.NewGuid();

            var existingTodo = new Todo
            {
                Id = todoId,
                MeetingId = meetingId,
                UserId = existingUserId,
                Title = "Original Title",
                Status = TodoStatus.Generated,
                ReferencedTasks = new List<ProjectTask>()
            };

            var request = new UpdateTodoRequest
            {
                AssigneeId = null,
                Title = "Updated Title"
            };

            Todo? capturedTodo = null;

            _mockTodoRepository
                .Setup(x => x.GetByIdAsync(todoId))
                .ReturnsAsync(existingTodo);

            _mockTodoRepository
                .Setup(x => x.UpdateAsync(It.IsAny<Todo>()))
                .Callback<Todo>(t => capturedTodo = t)
                .Returns(Task.CompletedTask);

            _mockTodoRepository
                .Setup(x => x.SaveChangesAsync())
                .Returns(Task.CompletedTask);

            // Act
            var result = await _todoService.UpdateTodoAsync(todoId, request);

            // Assert
            Assert.True(result.Success);
            Assert.NotNull(capturedTodo);
            Assert.Null(capturedTodo.UserId);
        }

        [Fact]
        public async Task UpdateTodoAsync_WithStartDate_UpdatesStartDate()
        {
            // Arrange
            var todoId = Guid.NewGuid();
            var meetingId = Guid.NewGuid();
            var newStartDate = DateTime.UtcNow.AddDays(5);

            var existingTodo = new Todo
            {
                Id = todoId,
                MeetingId = meetingId,
                Title = "Original Title",
                StartDate = DateTime.UtcNow,
                Status = TodoStatus.Generated,
                ReferencedTasks = new List<ProjectTask>()
            };

            var request = new UpdateTodoRequest
            {
                Title = "Updated Title",
                StartDate = newStartDate
            };

            Todo? capturedTodo = null;

            _mockTodoRepository
                .Setup(x => x.GetByIdAsync(todoId))
                .ReturnsAsync(existingTodo);

            _mockTodoRepository
                .Setup(x => x.UpdateAsync(It.IsAny<Todo>()))
                .Callback<Todo>(t => capturedTodo = t)
                .Returns(Task.CompletedTask);

            _mockTodoRepository
                .Setup(x => x.SaveChangesAsync())
                .Returns(Task.CompletedTask);

            // Act
            var result = await _todoService.UpdateTodoAsync(todoId, request);

            // Assert
            Assert.True(result.Success);
            Assert.NotNull(capturedTodo);
            Assert.Equal(newStartDate, capturedTodo.StartDate);
        }

        [Fact]
        public async Task UpdateTodoAsync_WithEndDate_UpdatesEndDate()
        {
            // Arrange
            var todoId = Guid.NewGuid();
            var meetingId = Guid.NewGuid();
            var newEndDate = DateTime.UtcNow.AddDays(10);

            var existingTodo = new Todo
            {
                Id = todoId,
                MeetingId = meetingId,
                Title = "Original Title",
                EndDate = DateTime.UtcNow.AddDays(1),
                Status = TodoStatus.Generated,
                ReferencedTasks = new List<ProjectTask>()
            };

            var request = new UpdateTodoRequest
            {
                Title = "Updated Title",
                EndDate = newEndDate
            };

            Todo? capturedTodo = null;

            _mockTodoRepository
                .Setup(x => x.GetByIdAsync(todoId))
                .ReturnsAsync(existingTodo);

            _mockTodoRepository
                .Setup(x => x.UpdateAsync(It.IsAny<Todo>()))
                .Callback<Todo>(t => capturedTodo = t)
                .Returns(Task.CompletedTask);

            _mockTodoRepository
                .Setup(x => x.SaveChangesAsync())
                .Returns(Task.CompletedTask);

            // Act
            var result = await _todoService.UpdateTodoAsync(todoId, request);

            // Assert
            Assert.True(result.Success);
            Assert.NotNull(capturedTodo);
            Assert.Equal(newEndDate, capturedTodo.EndDate);
        }

        [Fact]
        public async Task UpdateTodoAsync_WithAllFields_UpdatesAllFields()
        {
            // Arrange
            var todoId = Guid.NewGuid();
            var meetingId = Guid.NewGuid();
            var newAssigneeId = Guid.NewGuid();
            var newStartDate = DateTime.UtcNow.AddDays(2);
            var newEndDate = DateTime.UtcNow.AddDays(7);

            var existingTodo = new Todo
            {
                Id = todoId,
                MeetingId = meetingId,
                UserId = null,
                Title = "Original Title",
                Description = "Original Description",
                StartDate = DateTime.UtcNow,
                EndDate = DateTime.UtcNow.AddDays(1),
                Status = TodoStatus.Generated,
                CreatedAt = DateTime.UtcNow.AddDays(-1),
                ReferencedTasks = new List<ProjectTask>()
            };

            var request = new UpdateTodoRequest
            {
                AssigneeId = newAssigneeId,
                Title = "New Title",
                Description = "New Description",
                StartDate = newStartDate,
                EndDate = newEndDate
            };

            Todo? capturedTodo = null;

            _mockTodoRepository
                .Setup(x => x.GetByIdAsync(todoId))
                .ReturnsAsync(existingTodo);

            _mockTodoRepository
                .Setup(x => x.UpdateAsync(It.IsAny<Todo>()))
                .Callback<Todo>(t => capturedTodo = t)
                .Returns(Task.CompletedTask);

            _mockTodoRepository
                .Setup(x => x.SaveChangesAsync())
                .Returns(Task.CompletedTask);

            // Act
            var result = await _todoService.UpdateTodoAsync(todoId, request);

            // Assert
            Assert.True(result.Success);
            Assert.NotNull(capturedTodo);
            Assert.Equal(newAssigneeId, capturedTodo.UserId);
            Assert.Equal("New Title", capturedTodo.Title);
            Assert.Equal("New Description", capturedTodo.Description);
            Assert.Equal(newStartDate, capturedTodo.StartDate);
            Assert.Equal(newEndDate, capturedTodo.EndDate);
            Assert.Equal(TodoStatus.UnderReview, capturedTodo.Status);
        }

        [Fact]
        public async Task UpdateTodoAsync_PreservesCreatedAt()
        {
            // Arrange
            var todoId = Guid.NewGuid();
            var meetingId = Guid.NewGuid();
            var originalCreatedAt = DateTime.UtcNow.AddDays(-5);

            var existingTodo = new Todo
            {
                Id = todoId,
                MeetingId = meetingId,
                Title = "Original Title",
                Status = TodoStatus.Generated,
                CreatedAt = originalCreatedAt,
                ReferencedTasks = new List<ProjectTask>()
            };

            var request = new UpdateTodoRequest
            {
                Title = "Updated Title"
            };

            _mockTodoRepository
                .Setup(x => x.GetByIdAsync(todoId))
                .ReturnsAsync(existingTodo);

            _mockTodoRepository
                .Setup(x => x.UpdateAsync(It.IsAny<Todo>()))
                .Returns(Task.CompletedTask);

            _mockTodoRepository
                .Setup(x => x.SaveChangesAsync())
                .Returns(Task.CompletedTask);

            // Act
            var result = await _todoService.UpdateTodoAsync(todoId, request);

            // Assert
            Assert.True(result.Success);
            Assert.NotNull(result.Data);
            Assert.Equal(originalCreatedAt, result.Data.CreatedAt);
        }

        [Fact]
        public async Task UpdateTodoAsync_PreservesMeetingId()
        {
            // Arrange
            var todoId = Guid.NewGuid();
            var meetingId = Guid.NewGuid();

            var existingTodo = new Todo
            {
                Id = todoId,
                MeetingId = meetingId,
                Title = "Original Title",
                Status = TodoStatus.Generated,
                ReferencedTasks = new List<ProjectTask>()
            };

            var request = new UpdateTodoRequest
            {
                Title = "Updated Title"
            };

            _mockTodoRepository
                .Setup(x => x.GetByIdAsync(todoId))
                .ReturnsAsync(existingTodo);

            _mockTodoRepository
                .Setup(x => x.UpdateAsync(It.IsAny<Todo>()))
                .Returns(Task.CompletedTask);

            _mockTodoRepository
                .Setup(x => x.SaveChangesAsync())
                .Returns(Task.CompletedTask);

            // Act
            var result = await _todoService.UpdateTodoAsync(todoId, request);

            // Assert
            Assert.True(result.Success);
            Assert.NotNull(result.Data);
            Assert.Equal(meetingId, result.Data.MeetingId);
        }

        [Fact]
        public async Task UpdateTodoAsync_WithExistingUser_MapsAssigneeInResponse()
        {
            // Arrange
            var todoId = Guid.NewGuid();
            var meetingId = Guid.NewGuid();
            var userId = Guid.NewGuid();

            var user = new User
            {
                Id = userId,
                Email = "user@example.com",
                FullName = "Test User",
                AvatarUrl = "avatar.png"
            };

            var existingTodo = new Todo
            {
                Id = todoId,
                MeetingId = meetingId,
                UserId = userId,
                User = user,
                Title = "Original Title",
                Status = TodoStatus.Generated,
                ReferencedTasks = new List<ProjectTask>()
            };

            var request = new UpdateTodoRequest
            {
                AssigneeId = userId,
                Title = "Updated Title"
            };

            _mockTodoRepository
                .Setup(x => x.GetByIdAsync(todoId))
                .ReturnsAsync(existingTodo);

            _mockTodoRepository
                .Setup(x => x.UpdateAsync(It.IsAny<Todo>()))
                .Returns(Task.CompletedTask);

            _mockTodoRepository
                .Setup(x => x.SaveChangesAsync())
                .Returns(Task.CompletedTask);

            // Act
            var result = await _todoService.UpdateTodoAsync(todoId, request);

            // Assert
            Assert.True(result.Success);
            Assert.NotNull(result.Data);
            Assert.NotNull(result.Data.Assignee);
            Assert.Equal(userId, result.Data.Assignee.Id);
            Assert.Equal("user@example.com", result.Data.Assignee.Email);
            Assert.Equal("Test User", result.Data.Assignee.FullName);
            Assert.Equal("avatar.png", result.Data.Assignee.AvatarUrl);
        }

        #endregion
    }
}
