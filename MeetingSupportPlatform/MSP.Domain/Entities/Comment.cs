using MSP.Domain.Base;

namespace MSP.Domain.Entities
{
    public class Comment : BaseEntity<Guid>
    {
        public Guid UserId { get; set; }
        public virtual User User { get; set; }

        public Guid TaskId { get; set; }
        public virtual ProjectTask ProjectTask { get; set; }

        public string Content { get; set; }
    }
}
