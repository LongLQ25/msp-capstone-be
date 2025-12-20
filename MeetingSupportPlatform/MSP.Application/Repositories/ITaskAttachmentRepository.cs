using MSP.Application.Abstracts;
using MSP.Domain.Entities;

namespace MSP.Application.Repositories
{
    /// <summary>
    /// Repository interface for TaskAttachment operations
    /// </summary>
    public interface ITaskAttachmentRepository : IGenericRepository<TaskAttachment, Guid>
    {
        /// <summary>
        /// Get all attachments for a specific task
        /// </summary>
        Task<IEnumerable<TaskAttachment>> GetAttachmentsByTaskIdAsync(Guid taskId);

        /// <summary>
        /// Get attachment by Id with Task included
        /// </summary>
        Task<TaskAttachment?> GetAttachmentByIdWithTaskAsync(Guid attachmentId);
    }
}
