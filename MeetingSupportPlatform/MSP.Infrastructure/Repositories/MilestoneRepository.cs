using Microsoft.EntityFrameworkCore;
using MSP.Application.Repositories;
using MSP.Domain.Entities;
using MSP.Infrastructure.Persistence.DBContext;

namespace MSP.Infrastructure.Repositories
{
    public class MilestoneRepository(ApplicationDbContext context) : GenericRepository<Milestone, Guid>(context), IMilestoneRepository
    {
        public async Task<IEnumerable<Milestone>> GetMilestonesByProjectIdAsync(Guid projectId)
        {
            return await _context.Milestones
                .AsNoTracking()
                .Where(m => m.ProjectId == projectId && !m.IsDeleted)
                .Include(m => m.ProjectTasks)
                .Include(m => m.User)
                .OrderBy(m => m.DueDate)
                .ToListAsync();
        }

        public async Task<Milestone?> GetMilestoneByIdAsync(Guid id)
        {
            return await _context.Milestones
                .AsNoTracking()
                .Include(m => m.ProjectTasks)
                .Include(m => m.User)
                .FirstOrDefaultAsync(m => m.Id == id && !m.IsDeleted);
        }

    }
}
