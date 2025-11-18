using MSP.Application.Models.Requests.Comment;
using MSP.Application.Models.Responses.Comment;
using MSP.Shared.Common;

namespace MSP.Application.Services.Interfaces.Comment
{
    public interface ICommentService
    {
        Task<ApiResponse<GetCommentResponse>> CreateCommentAsync(CreateCommentRequest request);
        Task<ApiResponse<GetCommentResponse>> UpdateCommentAsync(UpdateCommentRequest request);
        Task<ApiResponse<string>> DeleteCommentAsync(Guid commentId, Guid userId);
        Task<ApiResponse<GetCommentResponse>> GetCommentByIdAsync(Guid commentId);
        Task<ApiResponse<PagingResponse<GetCommentResponse>>> GetCommentsByTaskIdAsync(PagingRequest request, Guid taskId);
        Task<ApiResponse<int>> GetCommentCountByTaskIdAsync(Guid taskId);
    }
}
