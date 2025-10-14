using Microsoft.EntityFrameworkCore;
using MSP.Application.Repositories;
using MSP.Domain.Entities;
using MSP.Infrastructure.Persistence.DBContext;

namespace MSP.Infrastructure.Repositories
{
    public class ProjectMemberRepository(ApplicationDbContext context) : GenericRepository<ProjectMember, Guid>(context), IProjectMemberRepository
    {
        public async Task<IEnumerable<ProjectMember>> GetProjectMembersByProjectIdAsync(Guid projectId)
        {
            return await _context.ProjectMembers
                .Where(pm => pm.ProjectId == projectId && !pm.IsDeleted)
                .Include(pm => pm.Project)
                .Include(pm => pm.Member)
                .ToListAsync();
        }
    }
}
