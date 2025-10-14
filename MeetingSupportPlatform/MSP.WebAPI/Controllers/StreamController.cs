using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MSP.Application.Models.Requests.Meeting;
using MSP.Application.Services.Interfaces.Meeting;
using MSP.Domain.Entities;

namespace MeetingService.API.Controllers
{
    [Route("api/v1/stream")]
    [ApiController]
    public class StreamController : ControllerBase
    {
        private readonly IStreamService _streamService;

        public StreamController(IStreamService streamService)
        {
            _streamService = streamService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> RegisterUser([FromBody] StreamUserRequest request)
        {
            // 1. Tạo user bên Stream
            await _streamService.CreateOrUpdateUserAsync(request);

            // 2. Generate user token cho client
            var userToken = _streamService.GenerateUserToken(request.Id);

            return Ok(new
            {
                request.Id,
                Token = userToken
            });
        }
        [HttpPost("call/create-call")]
        public async Task<IActionResult> CreateCall([FromBody] CreateMeetingRequest request)
        {
            try
            {
                await _streamService.CreateMeetingAsync(request);
                return Ok(new { Message = "Notification sent successfully" });

            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpPut("call/{id}/update")]
        public async Task<IActionResult> UpdateMeeting(Guid id, [FromBody] UpdateMeetingRequest request)
        {
            if (id != request.MeetingId)
                return BadRequest("Meeting ID mismatch.");

            try
            {
                await _streamService.UpdateMeetingAsync(request);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound("Meeting not found.");
            }
        }
        [HttpGet("call/{id}")]
        public async Task<IActionResult> GetMeetingById(Guid id)
        {
            var meeting = await _streamService.GetMeetingByIdAsync(id);
            if (meeting == null)
                return NotFound("Meeting not found.");

            return Ok(meeting);
        }

        [HttpGet("call/project/{projectId}")]
        public async Task<IActionResult> GetMeetingsByProjectId(Guid projectId)
        {
            var meetings = await _streamService.GetMeetingsByProjectIdAsync(projectId);
            return Ok(meetings);
        }

        [HttpPut("call/{id}/pause")]
        public async Task<IActionResult> PauseMeeting(Guid id)
        {
            var deleted = await _streamService.PauseMeetingAsync(id);
            if (!deleted)
                return NotFound();

            return NoContent();
        }

        [HttpPost("call/{type}/{id}/delete")]
        public async Task<IActionResult> DeleteCall(string type, string id, [FromQuery] bool hard = true)
        {
            await _streamService.DeleteCallAsync(type, id, hard);
            return NoContent(); // 204
        }

        [HttpGet("call/{type}/{id}/transcriptions")]
        public async Task<IActionResult> ListTranscriptions(string type, string id)
        {
            var result = await _streamService.ListTranscriptionsAsync(type, id);
            return Ok(result);
        }

    }
}