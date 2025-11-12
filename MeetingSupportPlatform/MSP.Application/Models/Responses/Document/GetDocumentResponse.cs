using MSP.Application.Models.Responses.Auth;

namespace MSP.Application.Models.Responses.Document
{
    public class GetDocumentResponse
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public Guid OwnerId { get; set; }
        public Guid ProjectId { get; set; }
        public string FileUrl { get; set; } = string.Empty;
        public string? Description { get; set; }
        public long Size { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public GetUserResponse? Owner { get; set; }
    }
}
