namespace MSP.Application.Models.Requests.Project
{
    public class AddProjectMemeberRequest
    {
        public Guid ProjectId { get; set; }
        public Guid UserId { get; set; }
    }
}
