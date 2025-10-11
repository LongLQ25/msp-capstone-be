using MSP.Domain.Base;

namespace MSP.Domain.Entities
{
    public class Document : BaseEntity<Guid>
    {
        public string Name { get; set; }

        public Guid OwnerId { get; set; }
        public virtual User Owner { get; set; }

        public Guid ProjectId { get; set; }
        public virtual Project Project { get; set; }

        public string FileUrl { get; set; }
        public string? Description { get; set; }
        public long Size { get; set; }
    }
}
