using System;

namespace MSP.Application.Models.Responses.Meeting
{
    public class MeetingResponse
    {
        public Guid Id { get; set; }

        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }

        public DateTime StartTime { get; set; }
        public DateTime? EndTime { get; set; }

        public string Status { get; set; } = string.Empty;

        public Guid CreatedById { get; set; }

        public Guid ProjectId { get; set; }
        public Guid? MilestoneId { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public List<AttendeeResponse> Attendees { get; set; } = new();
    }

    public class AttendeeResponse
    {
        public Guid Id { get; set; }
        public string Email { get; set; } = string.Empty;
        public string? AvatarUrl { get; set; } 
    }
}
