namespace MSP.Application.Models.Requests.Project
{
    public class CreateProjectRequest
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Status { get; set; } = string.Empty;
        public Guid CreatedById { get; set; }
    }
}
