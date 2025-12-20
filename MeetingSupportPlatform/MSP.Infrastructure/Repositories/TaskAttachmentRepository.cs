using Microsoft.EntityFrameworkCore;
using MSP.Application.Repositories;
using MSP.Domain.Entities;
using MSP.Infrastructure.Persistence.DBContext;

namespace MSP.Infrastructure.Repositories
{
    /// <summary>
    /// Repository implementation for TaskAttachment
    /// </summary>
    public class TaskAttachmentRepository : GenericRepository<TaskAttachment, Guid>, ITaskAttachmentRepository
    {
        public TaskAttachmentRepository(ApplicationDbContext context) : base(context)
        {
        }

        /// <summary>
        /// Get all attachments for a specific task
        /// </summary>
        public async Task<IEnumerable<TaskAttachment>> GetAttachmentsByTaskIdAsync(Guid taskId)
        {
            return await _dbSet
                .Where(a => a.TaskId == taskId && !a.IsDeleted)
                .OrderByDescending(a => a.CreatedAt)
                .AsNoTracking()
                .ToListAsync();
        }

        /// <summary>
        /// Get attachment by Id with Task included
        /// </summary>
        public async Task<TaskAttachment?> GetAttachmentByIdWithTaskAsync(Guid attachmentId)
        {
            return await _dbSet
                .Include(a => a.Task)
                .FirstOrDefaultAsync(a => a.Id == attachmentId && !a.IsDeleted);
        }
    }
}
