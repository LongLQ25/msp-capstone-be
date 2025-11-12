using MSP.Application.Models.Requests.Document;
using MSP.Application.Models.Responses.Document;
using MSP.Shared.Common;

namespace MSP.Application.Services.Interfaces.Document
{
    public interface IDocumentService
    {
        Task<ApiResponse<GetDocumentResponse>> CreateDocumentAsync(CreateDocumentRequest request);
        Task<ApiResponse<GetDocumentResponse>> UpdateDocumentAsync(UpdateDocumentRequest request);
        Task<ApiResponse<string>> DeleteDocumentAsync(Guid documentId);
        Task<ApiResponse<GetDocumentResponse>> GetDocumentByIdAsync(Guid documentId);
        Task<ApiResponse<PagingResponse<GetDocumentResponse>>> GetAllDocumentsAsync(PagingRequest request);
        Task<ApiResponse<PagingResponse<GetDocumentResponse>>> GetDocumentsByProjectIdAsync(PagingRequest request, Guid projectId);
        Task<ApiResponse<PagingResponse<GetDocumentResponse>>> GetDocumentsByOwnerIdAsync(PagingRequest request, Guid ownerId);
    }
}
