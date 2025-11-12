using MSP.Application.Abstracts;
using MSP.Domain.Entities;

namespace MSP.Application.Repositories
{
    public interface IDocumentRepository : IGenericRepository<Document, Guid>
    {
        Task<Document?> GetDocumentByIdAsync(Guid documentId);
        Task<IEnumerable<Document>> GetDocumentsByProjectIdAsync(Guid projectId);
        Task<IEnumerable<Document>> GetDocumentsByOwnerIdAsync(Guid ownerId);
    }
}
