using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MSP.Application.Services.Interfaces.OrganizationInvitation;

namespace MSP.WebAPI.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class OrganizationInvitationsController : ControllerBase
    {
        private readonly IOrganizationInvitationService _organizationInvitationService;
        public OrganizationInvitationsController(IOrganizationInvitationService organizationInvitationService)
        {
            _organizationInvitationService = organizationInvitationService;
        }

        [HttpPost("request-join")]
        [Authorize(Roles = "Member")]
        public async Task<IActionResult> RequestJoinOrganization([FromQuery] Guid memberId, [FromQuery] Guid businessOwnerId)
        {
            var result = await _organizationInvitationService.RequestJoinOrganizeAsync(memberId, businessOwnerId);
            return Ok(result);
        }

        [HttpPost("send-invitation")]
        [Authorize(Roles = "BusinessOwner")]
        public async Task<IActionResult> SendInvitation([FromQuery] Guid businessOwnerId, [FromQuery] string memberEmail)
        {
            var result = await _organizationInvitationService.SendInvitationAsync(businessOwnerId, memberEmail);
            return Ok(result);
        }

        [HttpGet("sent-invitations/{businessOwnerId}")]
        [Authorize(Roles = "BusinessOwner")]
        public async Task<IActionResult> GetSentInvitationsByBusinessOwnerId(Guid businessOwnerId)
        {
            var result = await _organizationInvitationService.GetSentInvitationsByBusinessOwnerIdAsync(businessOwnerId);
            return Ok(result);
        }
        [HttpGet("pending-requests/{businessOwnerId}")]
        [Authorize(Roles = "BusinessOwner")]
        public async Task<IActionResult> GetPendingRequestsByBusinessOwnerId(Guid businessOwnerId)
        {
            var result = await _organizationInvitationService.GetPendingRequestsByBusinessOwnerIdAsync(businessOwnerId);
            return Ok(result);
        }
        [HttpGet("received-invitations/{memberId}")]
        [Authorize(Roles = "Member")]
        public async Task<IActionResult> GetReceivedInvitationsByMemberId(Guid memberId)
        {
            var result = await _organizationInvitationService.GetReceivedInvitationsByMemberIdAsync(memberId);
            return Ok(result);
        }
        [HttpGet("sent-requests/{memberId}")]
        [Authorize(Roles = "Member")]
        public async Task<IActionResult> GetSentRequestsByMemberId(Guid memberId)
        {
            var result = await _organizationInvitationService.GetSentRequestsByMemberIdAsync(memberId);
            return Ok(result);
        }

        [HttpPost("accept-invitation/{invitationId}")]
        [Authorize(Roles = "Member")]
        public async Task<IActionResult> AcceptInvitation(Guid invitationId)
        {
            var curUserId = Guid.Parse(User.Claims.First(c => c.Type == "userId").Value);
            var result = await _organizationInvitationService.MemberAcceptInvitationAsync(curUserId, invitationId);

            return Ok(result);
        }

        [HttpPost("accept-request/{invitationId}")]
        [Authorize(Roles = "BusinessOwner")]
        public async Task<IActionResult> AcceptRequest(Guid invitationId)
        {
            var curUserId = Guid.Parse(User.Claims.First(c => c.Type == "userId").Value);
            var result = await _organizationInvitationService.BusinessOwnerAcceptRequestAsync(curUserId, invitationId);
            return Ok(result);
        }

        [HttpPost("leave-organization")]
        [Authorize(Roles = "Member")]
        public async Task<IActionResult> LeaveOrganization()
        {
            var curUserId = Guid.Parse(User.Claims.First(c => c.Type == "userId").Value);
            var result = await _organizationInvitationService.MemberLeaveOrganizationAsync(curUserId);

            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }
    }
}
