using System.ComponentModel.DataAnnotations;

namespace MSP.Application.Models.Requests.TaskAttachment
{
    /// <summary>
    /// Request to add file attachment metadata for a task
    /// (File already uploaded to Cloudinary by frontend)
    /// </summary>
    public class CreateTaskAttachmentRequest
    {
        [Required(ErrorMessage = "File name is required")]
        public string FileName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Original file name is required")]
        public string OriginalFileName { get; set; } = string.Empty;

        [Required(ErrorMessage = "File size is required")]
        public long FileSize { get; set; }

        [Required(ErrorMessage = "Content type is required")]
        public string ContentType { get; set; } = string.Empty;

        [Required(ErrorMessage = "File URL is required")]
        public string FileUrl { get; set; } = string.Empty;
    }
}
