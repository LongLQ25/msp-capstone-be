using MSP.Domain.Base;

namespace MSP.Domain.Entities
{
    public class Milestone : BaseEntity<Guid>
    {
        public Guid UserId { get; set; }
        public virtual User User { get; set; }

        public Guid ProjectId { get; set; }
        public virtual Project Project { get; set; }

        public string Name { get; set; }
        public string? Description { get; set; }
        public DateTime? DueDate { get; set; }

        public virtual ICollection<ProjectTask> ProjectTasks { get; set; } = new List<ProjectTask>();
    }
}
