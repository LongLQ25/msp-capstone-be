using MSP.Application.Models.Responses.Auth;

namespace MSP.Application.Models.Responses.Project
{
    public class GetProjectResponse
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string Status { get; set; } = string.Empty;
        public Guid OwnerId { get; set; }
        public GetUserResponse? Owner { get; set; }
        public Guid CreatedById { get; set; }
        public GetUserResponse? CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
