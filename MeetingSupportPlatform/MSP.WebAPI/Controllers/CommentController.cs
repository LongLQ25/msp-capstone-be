using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MSP.Application.Models.Requests.Comment;
using MSP.Application.Services.Interfaces.Comment;
using MSP.Shared.Common;

namespace MSP.WebAPI.Controllers
{
    [Route("api/v1/comments")]
    [ApiController]
    [Authorize]
    public class CommentController : ControllerBase
    {
        private readonly ICommentService _commentService;
        private readonly ILogger<CommentController> _logger;

        public CommentController(ICommentService commentService, ILogger<CommentController> logger)
        {
            _commentService = commentService;
            _logger = logger;
        }

        /// <summary>
        /// Create a new comment on a task
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> CreateComment([FromBody] CreateCommentRequest request)
        {
            var response = await _commentService.CreateCommentAsync(request);

            if (!response.Success)
            {
                _logger.LogError("CreateComment failed: {Message}", response.Message);
                return Ok(response);
            }

            return Ok(response);
        }

        /// <summary>
        /// Update an existing comment
        /// </summary>
        [HttpPut]
        public async Task<IActionResult> UpdateComment([FromBody] UpdateCommentRequest request)
        {
            var response = await _commentService.UpdateCommentAsync(request);
            return Ok(response);
        }

        /// <summary>
        /// Delete a comment (soft delete)
        /// </summary>
        [HttpDelete("{commentId}")]
        public async Task<IActionResult> DeleteComment([FromRoute] Guid commentId, [FromQuery] Guid userId)
        {
            var response = await _commentService.DeleteCommentAsync(commentId, userId);
            return Ok(response);
        }

        /// <summary>
        /// Get a comment by ID
        /// </summary>
        [HttpGet("{commentId}")]
        public async Task<IActionResult> GetCommentById([FromRoute] Guid commentId)
        {
            var response = await _commentService.GetCommentByIdAsync(commentId);
            return Ok(response);
        }

        /// <summary>
        /// Get all comments for a specific task (with pagination)
        /// </summary>
        [HttpGet("task/{taskId}")]
        public async Task<IActionResult> GetCommentsByTaskId([FromQuery] PagingRequest request, [FromRoute] Guid taskId)
        {
            var response = await _commentService.GetCommentsByTaskIdAsync(request, taskId);
            return Ok(response);
        }

        /// <summary>
        /// Get comment count for a specific task
        /// </summary>
        [HttpGet("task/{taskId}/count")]
        public async Task<IActionResult> GetCommentCountByTaskId([FromRoute] Guid taskId)
        {
            var response = await _commentService.GetCommentCountByTaskIdAsync(taskId);
            return Ok(response);
        }
    }
}
