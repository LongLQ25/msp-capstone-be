using Microsoft.EntityFrameworkCore;
using MSP.Application.Repositories;
using MSP.Domain.Entities;
using MSP.Infrastructure.Persistence.DBContext;

namespace MSP.Infrastructure.Repositories
{
    public class DocumentRepository : GenericRepository<Document, Guid>, IDocumentRepository
    {
        public DocumentRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<Document?> GetDocumentByIdAsync(Guid documentId)
        {
            return await _dbSet
                .Include(d => d.Owner)
                .Include(d => d.Project)
                .FirstOrDefaultAsync(d => d.Id == documentId && !d.IsDeleted);
        }

        public async Task<IEnumerable<Document>> GetDocumentsByProjectIdAsync(Guid projectId)
        {
            return await _dbSet
                .Include(d => d.Owner)
                .Include(d => d.Project)
                .Where(d => d.ProjectId == projectId && !d.IsDeleted)
                .ToListAsync();
        }

        public async Task<IEnumerable<Document>> GetDocumentsByOwnerIdAsync(Guid ownerId)
        {
            return await _dbSet
                .Include(d => d.Owner)
                .Include(d => d.Project)
                .Where(d => d.OwnerId == ownerId && !d.IsDeleted)
                .ToListAsync();
        }
    }
}
