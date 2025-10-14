using MSP.Application.Abstracts;
using MSP.Domain.Entities;

namespace MSP.Application.Repositories
{
    public interface IProjectMemberRepository : IGenericRepository<ProjectMember, Guid>
    {
        Task<IEnumerable<ProjectMember>> GetProjectMembersByProjectIdAsync(Guid projectId);
    }
}
