namespace MSP.Application.Models.Responses.TaskAttachment
{
    /// <summary>
    /// Response containing task attachment metadata
    /// </summary>
    public class TaskAttachmentResponse
    {
        public Guid Id { get; set; }
        public Guid TaskId { get; set; }
        public string FileName { get; set; } = string.Empty;
        public string OriginalFileName { get; set; } = string.Empty;
        public long FileSize { get; set; }
        public string ContentType { get; set; } = string.Empty;
        public string FileUrl { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }
}
