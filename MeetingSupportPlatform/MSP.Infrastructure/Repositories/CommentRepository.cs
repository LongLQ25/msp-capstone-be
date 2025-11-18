using Microsoft.EntityFrameworkCore;
using MSP.Application.Repositories;
using MSP.Domain.Entities;
using MSP.Infrastructure.Persistence.DBContext;

namespace MSP.Infrastructure.Repositories
{
    public class CommentRepository : GenericRepository<Comment, Guid>, ICommentRepository
    {
        public CommentRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<Comment?> GetCommentByIdAsync(Guid commentId)
        {
            return await _dbSet
                .Include(c => c.User)
                .Include(c => c.ProjectTask)
                .FirstOrDefaultAsync(c => c.Id == commentId && !c.IsDeleted);
        }

        public async Task<IEnumerable<Comment>> GetCommentsByTaskIdAsync(Guid taskId)
        {
            return await _dbSet
                .Include(c => c.User)
                .Where(c => c.TaskId == taskId && !c.IsDeleted)
                .OrderByDescending(c => c.CreatedAt)
                .ToListAsync();
        }

        public async Task<int> CountCommentsByTaskIdAsync(Guid taskId)
        {
            return await _dbSet
                .CountAsync(c => c.TaskId == taskId && !c.IsDeleted);
        }
    }
}
