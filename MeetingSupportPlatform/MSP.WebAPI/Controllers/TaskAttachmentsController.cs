using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MSP.Application.Models.Requests.TaskAttachment;
using MSP.Application.Services.Interfaces.TaskAttachment;

namespace MSP.WebAPI.Controllers
{
    /// <summary>
    /// API Controller for managing task file attachments
    /// </summary>
    [ApiController]
    [Route("api/v1/tasks")]
    //[Authorize]
    public class TaskAttachmentsController : ControllerBase
    {
        private readonly ITaskAttachmentService _attachmentService;

        public TaskAttachmentsController(
            ITaskAttachmentService attachmentService)
        {
            _attachmentService = attachmentService;
        }

        /// <summary>
        /// Create attachment metadata for a task (file already uploaded to Cloudinary by frontend)
        /// </summary>
        /// <param name="taskId">Task ID</param>
        /// <param name="request">Attachment metadata</param>
        /// <returns>Created attachment metadata</returns>
        [HttpPost("{taskId}/attachments")]
        public async Task<IActionResult> CreateAttachment(
            [FromRoute] Guid taskId,
            [FromBody] CreateTaskAttachmentRequest request)
        {
            var response = await _attachmentService.CreateTaskAttachmentAsync(taskId, request);
            return Ok(response);
        }

        /// <summary>
        /// Get all attachments for a task
        /// </summary>
        /// <param name="taskId">Task ID</param>
        /// <returns>List of task attachments</returns>
        [HttpGet("{taskId}/attachments")]
        public async Task<IActionResult> GetTaskAttachments([FromRoute] Guid taskId)
        {
            var response = await _attachmentService.GetTaskAttachmentsAsync(taskId);
            return Ok(response);
        }

        /// <summary>
        /// Delete an attachment
        /// </summary>
        /// <param name="attachmentId">Attachment ID</param>
        /// <returns>Success message</returns>
        [HttpDelete("attachments/{attachmentId}")]
        public async Task<IActionResult> DeleteAttachment([FromRoute] Guid attachmentId)
        {
            var response = await _attachmentService.DeleteAttachmentAsync(attachmentId);
            return Ok(response);
        }
    }
}
