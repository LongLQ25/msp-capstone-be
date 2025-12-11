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
    public class DeleteTodoListTest
    {
        private readonly Mock<ITodoRepository> _mockTodoRepository;
        private readonly Mock<IMeetingRepository> _mockMeetingRepository;
        private readonly Mock<IProjectTaskRepository> _mockProjectTaskRepository;
        private readonly Mock<UserManager<User>> _mockUserManager;
        private readonly ITodoService _todoService;

        public DeleteTodoListTest()
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

        #region DeleteTodoAsync Tests

        [Fact]
        public async Task DeleteTodoAsync_WithValidTodoId_ReturnsSuccessResponse()
        {
            // Arrange
            var todoId = Guid.NewGuid();
            var meetingId = Guid.NewGuid();

            var existingTodo = new Todo
            {
                Id = todoId,
                MeetingId = meetingId,
                Title = "Test Todo",
                Description = "Test Description",
                Status = TodoStatus.Generated,
                IsDeleted = false,
                ReferencedTasks = new List<ProjectTask>()
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
            var result = await _todoService.DeleteTodoAsync(todoId);

            // Assert
            Assert.True(result.Success);
            Assert.Equal("Delete todo item successfully", result.Message);

            _mockTodoRepository.Verify(x => x.GetByIdAsync(todoId), Times.Once);
            _mockTodoRepository.Verify(x => x.UpdateAsync(It.IsAny<Todo>()), Times.Once);
            _mockTodoRepository.Verify(x => x.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task DeleteTodoAsync_WithNonExistentTodoId_ReturnsErrorResponse()
        {
            // Arrange
            var todoId = Guid.NewGuid();

            _mockTodoRepository
                .Setup(x => x.GetByIdAsync(todoId))
                .ReturnsAsync((Todo?)null);

            // Act
            var result = await _todoService.DeleteTodoAsync(todoId);

            // Assert
            Assert.False(result.Success);
            Assert.Equal("Todo not found", result.Message);

            _mockTodoRepository.Verify(x => x.GetByIdAsync(todoId), Times.Once);
            _mockTodoRepository.Verify(x => x.UpdateAsync(It.IsAny<Todo>()), Times.Never);
            _mockTodoRepository.Verify(x => x.SaveChangesAsync(), Times.Never);
        }

        [Fact]
        public async Task DeleteTodoAsync_SetsStatusToDeleted()
        {
            // Arrange
            var todoId = Guid.NewGuid();
            var meetingId = Guid.NewGuid();

            var existingTodo = new Todo
            {
                Id = todoId,
                MeetingId = meetingId,
                Title = "Test Todo",
                Status = TodoStatus.Generated,
                IsDeleted = false,
                ReferencedTasks = new List<ProjectTask>()
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
            await _todoService.DeleteTodoAsync(todoId);

            // Assert
            Assert.NotNull(capturedTodo);
            Assert.Equal(TodoStatus.Deleted, capturedTodo.Status);
        }

        [Fact]
        public async Task DeleteTodoAsync_SetsIsDeletedToTrue()
        {
            // Arrange
            var todoId = Guid.NewGuid();
            var meetingId = Guid.NewGuid();

            var existingTodo = new Todo
            {
                Id = todoId,
                MeetingId = meetingId,
                Title = "Test Todo",
                Status = TodoStatus.Generated,
                IsDeleted = false,
                ReferencedTasks = new List<ProjectTask>()
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
            await _todoService.DeleteTodoAsync(todoId);

            // Assert
            Assert.NotNull(capturedTodo);
            Assert.True(capturedTodo.IsDeleted);
        }

        [Fact]
        public async Task DeleteTodoAsync_WithGeneratedStatus_DeletesSuccessfully()
        {
            // Arrange
            var todoId = Guid.NewGuid();
            var meetingId = Guid.NewGuid();

            var existingTodo = new Todo
            {
                Id = todoId,
                MeetingId = meetingId,
                Title = "Generated Todo",
                Status = TodoStatus.Generated,
                IsDeleted = false,
                ReferencedTasks = new List<ProjectTask>()
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
            var result = await _todoService.DeleteTodoAsync(todoId);

            // Assert
            Assert.True(result.Success);
            Assert.Equal("Delete todo item successfully", result.Message);
        }

        [Fact]
        public async Task DeleteTodoAsync_WithUnderReviewStatus_DeletesSuccessfully()
        {
            // Arrange
            var todoId = Guid.NewGuid();
            var meetingId = Guid.NewGuid();

            var existingTodo = new Todo
            {
                Id = todoId,
                MeetingId = meetingId,
                Title = "UnderReview Todo",
                Status = TodoStatus.UnderReview,
                IsDeleted = false,
                ReferencedTasks = new List<ProjectTask>()
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
            var result = await _todoService.DeleteTodoAsync(todoId);

            // Assert
            Assert.True(result.Success);
            Assert.Equal("Delete todo item successfully", result.Message);
        }

        [Fact]
        public async Task DeleteTodoAsync_WithConvertedToTaskStatus_DeletesSuccessfully()
        {
            // Arrange
            var todoId = Guid.NewGuid();
            var meetingId = Guid.NewGuid();

            var existingTodo = new Todo
            {
                Id = todoId,
                MeetingId = meetingId,
                Title = "ConvertedToTask Todo",
                Status = TodoStatus.ConvertedToTask,
                IsDeleted = false,
                ReferencedTasks = new List<ProjectTask>()
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
            var result = await _todoService.DeleteTodoAsync(todoId);

            // Assert
            Assert.True(result.Success);
            Assert.Equal("Delete todo item successfully", result.Message);
        }

        [Fact]
        public async Task DeleteTodoAsync_UsesSoftDeleteNotHardDelete()
        {
            // Arrange
            var todoId = Guid.NewGuid();
            var meetingId = Guid.NewGuid();

            var existingTodo = new Todo
            {
                Id = todoId,
                MeetingId = meetingId,
                Title = "Test Todo",
                Status = TodoStatus.Generated,
                IsDeleted = false,
                ReferencedTasks = new List<ProjectTask>()
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
            await _todoService.DeleteTodoAsync(todoId);

            // Assert - Verify UpdateAsync is called instead of HardDeleteAsync
            _mockTodoRepository.Verify(x => x.UpdateAsync(It.IsAny<Todo>()), Times.Once);
            _mockTodoRepository.Verify(x => x.HardDeleteAsync(It.IsAny<Todo>()), Times.Never);
        }

        [Fact]
        public async Task DeleteTodoAsync_VerifiesSaveChangesIsCalled()
        {
            // Arrange
            var todoId = Guid.NewGuid();
            var meetingId = Guid.NewGuid();

            var existingTodo = new Todo
            {
                Id = todoId,
                MeetingId = meetingId,
                Title = "Test Todo",
                Status = TodoStatus.Generated,
                IsDeleted = false,
                ReferencedTasks = new List<ProjectTask>()
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
            await _todoService.DeleteTodoAsync(todoId);

            // Assert
            _mockTodoRepository.Verify(x => x.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task DeleteTodoAsync_ReturnsNullData()
        {
            // Arrange
            var todoId = Guid.NewGuid();
            var meetingId = Guid.NewGuid();

            var existingTodo = new Todo
            {
                Id = todoId,
                MeetingId = meetingId,
                Title = "Test Todo",
                Status = TodoStatus.Generated,
                IsDeleted = false,
                ReferencedTasks = new List<ProjectTask>()
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
            var result = await _todoService.DeleteTodoAsync(todoId);

            // Assert
            Assert.True(result.Success);
            Assert.Null(result.Data);
        }

        [Fact]
        public async Task DeleteTodoAsync_WithTodoHavingReferencedTasks_DeletesSuccessfully()
        {
            // Arrange
            var todoId = Guid.NewGuid();
            var meetingId = Guid.NewGuid();

            var referencedTasks = new List<ProjectTask>
            {
                new ProjectTask { Id = Guid.NewGuid(), Title = "Task 1" },
                new ProjectTask { Id = Guid.NewGuid(), Title = "Task 2" }
            };

            var existingTodo = new Todo
            {
                Id = todoId,
                MeetingId = meetingId,
                Title = "Todo with Referenced Tasks",
                Status = TodoStatus.Generated,
                IsDeleted = false,
                ReferencedTasks = referencedTasks
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
            var result = await _todoService.DeleteTodoAsync(todoId);

            // Assert
            Assert.True(result.Success);
            Assert.Equal("Delete todo item successfully", result.Message);
        }

        [Fact]
        public async Task DeleteTodoAsync_WithTodoHavingAssignee_DeletesSuccessfully()
        {
            // Arrange
            var todoId = Guid.NewGuid();
            var meetingId = Guid.NewGuid();
            var userId = Guid.NewGuid();

            var user = new User
            {
                Id = userId,
                Email = "user@example.com",
                FullName = "Test User"
            };

            var existingTodo = new Todo
            {
                Id = todoId,
                MeetingId = meetingId,
                UserId = userId,
                User = user,
                Title = "Todo with Assignee",
                Status = TodoStatus.Generated,
                IsDeleted = false,
                ReferencedTasks = new List<ProjectTask>()
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
            var result = await _todoService.DeleteTodoAsync(todoId);

            // Assert
            Assert.True(result.Success);
            Assert.Equal("Delete todo item successfully", result.Message);
        }

        [Fact]
        public async Task DeleteTodoAsync_SetsStatusAndIsDeletedTogether()
        {
            // Arrange
            var todoId = Guid.NewGuid();
            var meetingId = Guid.NewGuid();

            var existingTodo = new Todo
            {
                Id = todoId,
                MeetingId = meetingId,
                Title = "Test Todo",
                Status = TodoStatus.Generated,
                IsDeleted = false,
                ReferencedTasks = new List<ProjectTask>()
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
            await _todoService.DeleteTodoAsync(todoId);

            // Assert
            Assert.NotNull(capturedTodo);
            Assert.Equal(TodoStatus.Deleted, capturedTodo.Status);
            Assert.True(capturedTodo.IsDeleted);
        }

        #endregion
    }
}
