using MSP.Application.Models.Responses.Auth;

namespace MSP.Application.Models.Responses.Comment
{
    public class GetCommentResponse
    {
        public Guid Id { get; set; }
        public Guid TaskId { get; set; }
        public Guid UserId { get; set; }
        public string Content { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public GetUserResponse? User { get; set; }
    }
}
