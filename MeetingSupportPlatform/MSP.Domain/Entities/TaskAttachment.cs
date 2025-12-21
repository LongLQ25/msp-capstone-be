using MSP.Domain.Base;

namespace MSP.Domain.Entities
{
    /// <summary>
    /// Represents a file attachment for a ProjectTask
    /// </summary>
    public class TaskAttachment : BaseEntity<Guid>
    {
        public Guid TaskId { get; set; }
        public virtual ProjectTask Task { get; set; } = null!;

        public string FileName { get; set; } = string.Empty;
        public string OriginalFileName { get; set; } = string.Empty;
        public long FileSize { get; set; }
        public string ContentType { get; set; } = string.Empty;
        public string FileUrl { get; set; } = string.Empty;
    }
}
