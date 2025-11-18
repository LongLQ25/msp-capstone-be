using MSP.Application.Abstracts;
using MSP.Domain.Entities;

namespace MSP.Application.Repositories
{
    public interface ICommentRepository : IGenericRepository<Comment, Guid>
    {
        Task<Comment?> GetCommentByIdAsync(Guid commentId);
        Task<IEnumerable<Comment>> GetCommentsByTaskIdAsync(Guid taskId);
        Task<int> CountCommentsByTaskIdAsync(Guid taskId);
    }
}
