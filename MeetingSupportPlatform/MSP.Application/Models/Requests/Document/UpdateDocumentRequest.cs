using System.ComponentModel.DataAnnotations;

namespace MSP.Application.Models.Requests.Document
{
    public class UpdateDocumentRequest
    {
        [Required]
        public Guid Id { get; set; }

        [Required]
        public string Name { get; set; } = string.Empty;

        public string? Description { get; set; }
    }
}
