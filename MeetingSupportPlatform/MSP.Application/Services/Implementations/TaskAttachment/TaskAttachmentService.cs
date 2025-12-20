using MSP.Application.Models.Requests.TaskAttachment;
using MSP.Application.Models.Responses.TaskAttachment;
using MSP.Application.Repositories;
using MSP.Application.Services.Interfaces.TaskAttachment;
using MSP.Shared.Common;

namespace MSP.Application.Services.Implementations.TaskAttachment
{
    /// <summary>
    /// Service implementation for TaskAttachment operations
    /// </summary>
    public class TaskAttachmentService : ITaskAttachmentService
    {
        private readonly ITaskAttachmentRepository _attachmentRepository;
        private readonly IProjectTaskRepository _taskRepository;

        public TaskAttachmentService(
            ITaskAttachmentRepository attachmentRepository,
            IProjectTaskRepository taskRepository)
        {
            _attachmentRepository = attachmentRepository;
            _taskRepository = taskRepository;
        }

        /// <summary>
        /// Create attachment metadata for a task
        /// </summary>
        public async Task<ApiResponse<TaskAttachmentResponse>> CreateTaskAttachmentAsync(Guid taskId, CreateTaskAttachmentRequest request)
        {
            // Validate task exists
            var task = await _taskRepository.GetByIdAsync(taskId);
            if (task == null || task.IsDeleted)
            {
                return ApiResponse<TaskAttachmentResponse>.ErrorResponse(null, "Task not found");
            }

            var attachment = new Domain.Entities.TaskAttachment
            {
                TaskId = taskId,
                FileName = request.FileName,
                OriginalFileName = request.OriginalFileName,
                FileSize = request.FileSize,
                ContentType = request.ContentType,
                FileUrl = request.FileUrl,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await _attachmentRepository.AddAsync(attachment);
            await _attachmentRepository.SaveChangesAsync();

            var response = new TaskAttachmentResponse
            {
                Id = attachment.Id,
                TaskId = attachment.TaskId,
                FileName = attachment.FileName,
                OriginalFileName = attachment.OriginalFileName,
                FileSize = attachment.FileSize,
                ContentType = attachment.ContentType,
                FileUrl = attachment.FileUrl,
                CreatedAt = attachment.CreatedAt
            };

            return ApiResponse<TaskAttachmentResponse>.SuccessResponse(response, "Attachment created successfully");
        }

        /// <summary>
        /// Get all attachments for a task
        /// </summary>
        public async Task<ApiResponse<List<TaskAttachmentResponse>>> GetTaskAttachmentsAsync(Guid taskId)
        {
            // Validate task exists
            var task = await _taskRepository.GetByIdAsync(taskId);
            if (task == null || task.IsDeleted)
            {
                return ApiResponse<List<TaskAttachmentResponse>>.ErrorResponse(null, "Task not found");
            }

            var attachments = await _attachmentRepository.GetAttachmentsByTaskIdAsync(taskId);

            var responses = attachments.Select(a => new TaskAttachmentResponse
            {
                Id = a.Id,
                TaskId = a.TaskId,
                FileName = a.FileName,
                OriginalFileName = a.OriginalFileName,
                FileSize = a.FileSize,
                ContentType = a.ContentType,
                FileUrl = a.FileUrl,
                CreatedAt = a.CreatedAt
            }).ToList();

            return ApiResponse<List<TaskAttachmentResponse>>.SuccessResponse(
                responses,
                $"Retrieved {responses.Count} attachment(s)");
        }

        /// <summary>
        /// Delete an attachment
        /// </summary>
        public async Task<ApiResponse<string>> DeleteAttachmentAsync(Guid attachmentId)
        {
            var attachment = await _attachmentRepository.GetByIdAsync(attachmentId);
            if (attachment == null || attachment.IsDeleted)
            {
                return ApiResponse<string>.ErrorResponse(null, "Attachment not found");
            }

            // Soft delete from database only
            // Frontend should handle Cloudinary file deletion
            await _attachmentRepository.SoftDeleteAsync(attachment);
            await _attachmentRepository.SaveChangesAsync();

            return ApiResponse<string>.SuccessResponse(null, "Attachment deleted successfully");
        }
    }
}
