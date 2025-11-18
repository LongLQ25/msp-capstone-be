using System.ComponentModel.DataAnnotations;

namespace MSP.Application.Models.Requests.Comment
{
    public class UpdateCommentRequest
    {
        [Required]
        public Guid Id { get; set; }

        [Required]
        [StringLength(2000, MinimumLength = 1, ErrorMessage = "Content must be between 1 and 2000 characters")]
        public string Content { get; set; } = string.Empty;
    }
}
