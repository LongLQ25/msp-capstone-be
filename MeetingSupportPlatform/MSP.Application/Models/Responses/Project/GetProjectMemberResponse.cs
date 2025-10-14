using MSP.Application.Models.Responses.Auth;

namespace MSP.Application.Models.Responses.Project
{
    public class GetProjectMemberResponse
    {
        public Guid Id { get; set; }
        public Guid ProjectId { get; set; }
        public Guid UserId { get; set; }
        public GetUserResponse? Member { get; set; }
        public DateTime JoinedAt { get; set; }
        public DateTime? LeftAt { get; set; }
    }
}
