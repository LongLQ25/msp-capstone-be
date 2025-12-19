using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using MSP.Application.Models.Requests.Notification;
using MSP.Application.Repositories;
using MSP.Application.Services.Interfaces.Notification;
using MSP.Domain.Entities;
using MSP.Shared.Enums;
using System.Text.Json;

namespace MSP.Application.Services.Implementations.Meeting
{
    /// <summary>
    /// Cron job service to send meeting reminder notifications
    /// Runs every 10 minutes and sends reminder ~1 hour before meeting starts
    /// </summary>
    public class MeetingReminderCronJobService
    {
        private readonly IMeetingRepository _meetingRepository;
        private readonly INotificationService _notificationService;
        private readonly UserManager<User> _userManager;
        private readonly ILogger<MeetingReminderCronJobService> _logger;

        private static readonly TimeZoneInfo VietnamTimeZone =
            TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");

        public MeetingReminderCronJobService(
            IMeetingRepository meetingRepository,
            INotificationService notificationService,
            UserManager<User> userManager,
            ILogger<MeetingReminderCronJobService> logger)
        {
            _meetingRepository = meetingRepository;
            _notificationService = notificationService;
            _userManager = userManager;
            _logger = logger;
        }

        public async Task SendMeetingRemindersAsync()
        {
            try
            {
                var nowUtc = DateTime.UtcNow;
                int notificationsSent = 0;

                _logger.LogInformation(
                    "Checking upcoming meetings for reminder at {TimeUtc}",
                    nowUtc);

                // Window 50–70 minutes (cron runs every 10 minutes)
                var upcomingMeetings =
                    await _meetingRepository.GetUpcomingMeetingsForReminderAsync(
                        nowUtc.AddMinutes(50),
                        nowUtc.AddMinutes(70),
                        MeetingEnum.Scheduled.ToString());

                if (!upcomingMeetings.Any())
                {
                    _logger.LogInformation("No meetings eligible for reminder");
                    return;
                }

                _logger.LogInformation(
                    "Found {Count} meeting(s) eligible for reminder",
                    upcomingMeetings.Count());

                foreach (var meeting in upcomingMeetings)
                {
                    try
                    {
                        // Meeting time must be UTC
                        var minutesUntilStart =
                            (int)(meeting.StartTime - nowUtc).TotalMinutes;

                        var startTimeGmt7 =
                            TimeZoneInfo.ConvertTimeFromUtc(
                                meeting.StartTime,
                                VietnamTimeZone);

                        _logger.LogInformation(
                            "Processing meeting {MeetingId} ('{Title}') starting in {Minutes} minutes at {StartTimeUtc}",
                            meeting.Id,
                            meeting.Title,
                            minutesUntilStart,
                            meeting.StartTime);

                        var attendees = meeting.Attendees?.ToList() ?? new List<User>();

                        if (meeting.CreatedBy != null &&
                            !attendees.Any(a => a.Id == meeting.CreatedById))
                        {
                            attendees.Add(meeting.CreatedBy);
                        }

                        if (!attendees.Any())
                        {
                            _logger.LogWarning(
                                "Meeting {MeetingId} has no attendees",
                                meeting.Id);
                            continue;
                        }

                        foreach (var attendee in attendees)
                        {
                            try
                            {
                                // ---------- In-app notification ----------
                                var notificationRequest = new CreateNotificationRequest
                                {
                                    UserId = attendee.Id,
                                    Title = "Meeting Reminder",
                                    Message =
                                        $"Meeting '{meeting.Title}' will start in about 1 hour at {startTimeGmt7:HH:mm}.",
                                    Type = NotificationTypeEnum.MeetingReminder.ToString(),
                                    EntityId = meeting.Id.ToString(),
                                    Data = JsonSerializer.Serialize(new
                                    {
                                        MeetingId = meeting.Id,
                                        MeetingTitle = meeting.Title,
                                        StartTimeUtc = meeting.StartTime,
                                        ProjectId = meeting.ProjectId,
                                        ProjectName = meeting.Project?.Name,
                                        ReminderType = "OneHourReminder"
                                    })
                                };

                                await _notificationService
                                    .CreateInAppNotificationAsync(notificationRequest);

                                // ---------- Email notification ----------
                                if (!string.IsNullOrWhiteSpace(attendee.Email))
                                {
                                    _notificationService.SendEmailNotification(
                                        attendee.Email,
                                        "Meeting Reminder",
                                        $"""
                                        Hello {attendee.FullName},<br/><br/>
                                        This is a reminder that you have an upcoming meeting:<br/><br/>
                                        <strong>Meeting:</strong> {meeting.Title}<br/>
                                        <strong>Start Time:</strong> {startTimeGmt7:dd/MM/yyyy HH:mm} (GMT+7)<br/>
                                        <strong>Project:</strong> {meeting.Project?.Name ?? "N/A"}<br/>
                                        {(string.IsNullOrEmpty(meeting.Description)
                                            ? ""
                                            : $"<strong>Description:</strong> {meeting.Description}<br/>")}
                                        <br/>
                                        The meeting will start in about <strong>1 hour</strong>.<br/><br/>
                                        Please be prepared and join on time.
                                        """);
                                }

                                notificationsSent++;
                            }
                            catch (Exception attendeeEx)
                            {
                                _logger.LogError(
                                    attendeeEx,
                                    "Failed to send reminder to user {UserId} for meeting {MeetingId}",
                                    attendee.Id,
                                    meeting.Id);
                            }
                        }
                    }
                    catch (Exception meetingEx)
                    {
                        _logger.LogError(
                            meetingEx,
                            "Failed to process reminder for meeting {MeetingId}",
                            meeting.Id);
                    }
                }

                _logger.LogInformation(
                    "Meeting reminder job finished. Notifications sent: {Count}",
                    notificationsSent);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Meeting reminder cron job failed");
                throw; // let Hangfire retry
            }
        }
    }
}
