using System.ComponentModel.DataAnnotations;

namespace MSP.Application.Models.Requests.Document
{
    public class CreateDocumentRequest
    {
        [Required]
        public string Name { get; set; } = string.Empty;

        [Required]
        public Guid OwnerId { get; set; }

        [Required]
        public Guid ProjectId { get; set; }

        [Required]
        public string FileUrl { get; set; } = string.Empty;

        public string? Description { get; set; }

        [Required]
        public long Size { get; set; }
    }
}
