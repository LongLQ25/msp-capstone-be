using MSP.Application.Models.Requests.TaskAttachment;
using MSP.Application.Models.Responses.TaskAttachment;
using MSP.Shared.Common;

namespace MSP.Application.Services.Interfaces.TaskAttachment
{
    /// <summary>
    /// Service interface for TaskAttachment operations
    /// </summary>
    public interface ITaskAttachmentService
    {
        /// <summary>
        /// Add attachment metadata for a task (file already uploaded to Cloudinary)
        /// </summary>
        Task<ApiResponse<TaskAttachmentResponse>> CreateTaskAttachmentAsync(Guid taskId, CreateTaskAttachmentRequest request);

        /// <summary>
        /// Get all attachments for a task
        /// </summary>
        Task<ApiResponse<List<TaskAttachmentResponse>>> GetTaskAttachmentsAsync(Guid taskId);

        /// <summary>
        /// Delete an attachment metadata (file on Cloudinary should be deleted by frontend)
        /// </summary>
        Task<ApiResponse<string>> DeleteAttachmentAsync(Guid attachmentId);
    }
}
