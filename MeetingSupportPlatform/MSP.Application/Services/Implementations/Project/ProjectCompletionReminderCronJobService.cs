using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using MSP.Application.Models.Requests.Notification;
using MSP.Application.Repositories;
using MSP.Application.Services.Interfaces.Notification;
using MSP.Shared.Enums;

namespace MSP.Application.Services.Implementations.Project
{
    /// <summary>
    /// Service to send completion reminder notifications for overdue projects using Hangfire
    /// </summary>
    public class ProjectCompletionReminderCronJobService
    {
        private readonly IProjectRepository _projectRepository;
        private readonly INotificationService _notificationService;
        private readonly ILogger<ProjectCompletionReminderCronJobService> _logger;

        public ProjectCompletionReminderCronJobService(
            IProjectRepository projectRepository,
            INotificationService notificationService,
            ILogger<ProjectCompletionReminderCronJobService> logger)
        {
            _projectRepository = projectRepository;
            _notificationService = notificationService;
            _logger = logger;
        }

        /// <summary>
        /// Send completion reminder notifications for projects that are past EndDate but still InProgress
        /// This method will be called by Hangfire Recurring Job
        /// </summary>
        public async Task SendCompletionRemindersAsync()
        {
            try
            {
                _logger.LogInformation("Starting to check for overdue projects needing completion at {Time}", DateTime.UtcNow);

                var now = DateTime.UtcNow;
                int notificationsSent = 0;

                // Get projects that are past EndDate and still InProgress
                var overdueProjects = await _projectRepository.GetOverdueInProgressProjectsAsync(
                    now,
                    ProjectStatusEnum.InProgress.ToString());

                if (overdueProjects.Any())
                {
                    _logger.LogInformation("Found {Count} overdue InProgress projects", overdueProjects.Count());

                    foreach (var project in overdueProjects)
                    {
                        try
                        {
                            // Calculate days overdue
                            var daysOverdue = (now.Date - project.EndDate!.Value.Date).Days;
                            
                            _logger.LogWarning(
                                "Project {ProjectId} ('{Name}') is {Days} days overdue. EndDate was {EndDate}",
                                project.Id,
                                project.Name,
                                daysOverdue,
                                project.EndDate);

                            // Send notification to Project Owner
                            var notificationRequest = new CreateNotificationRequest
                            {
                                UserId = project.OwnerId,
                                Title = "? Nh?c nh? hoàn thành d? án",
                                Message = $"D? án '{project.Name}' ?ã quá h?n {daysOverdue} ngày (H?n chót: {project.EndDate:dd/MM/yyyy}). Vui lòng xem xét và ?ánh d?u hoàn thành n?u d? án ?ã xong.",
                                Type = NotificationTypeEnum.ProjectUpdate.ToString(),
                                EntityId = project.Id.ToString(),
                                Data = System.Text.Json.JsonSerializer.Serialize(new
                                {
                                    ProjectId = project.Id,
                                    ProjectName = project.Name,
                                    EndDate = project.EndDate,
                                    DaysOverdue = daysOverdue,
                                    EventType = "ProjectCompletionReminder"
                                })
                            };

                            await _notificationService.CreateInAppNotificationAsync(notificationRequest);

                            // Send email notification to owner
                            var owner = project.Owner;
                            if (owner != null)
                            {
                                _notificationService.SendEmailNotification(
                                    owner.Email!,
                                    "Nh?c nh? hoàn thành d? án",
                                    $"Xin chào {owner.FullName},<br/><br/>" +
                                    $"D? án <strong>{project.Name}</strong> ?ã v??t quá ngày k?t thúc d? ki?n {daysOverdue} ngày.<br/><br/>" +
                                    $"<strong>Ngày k?t thúc d? ki?n:</strong> {project.EndDate:dd/MM/yyyy}<br/>" +
                                    $"<strong>Ngày hi?n t?i:</strong> {now:dd/MM/yyyy}<br/>" +
                                    $"<strong>S? ngày quá h?n:</strong> {daysOverdue} ngày<br/>" +
                                    $"<strong>Tr?ng thái hi?n t?i:</strong> ?ang th?c hi?n<br/><br/>" +
                                    $"N?u d? án ?ã hoàn thành, vui lòng c?p nh?t tr?ng thái thành 'Hoàn thành' trong h? th?ng.<br/>" +
                                    $"N?u d? án c?n thêm th?i gian, vui lòng xem xét ?i?u ch?nh ngày k?t thúc.");
                            }

                            notificationsSent++;

                            _logger.LogInformation(
                                "Sent completion reminder for project {ProjectId} to owner {OwnerId}. Overdue by {Days} days",
                                project.Id,
                                project.OwnerId,
                                daysOverdue);
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex,
                                "Failed to send completion reminder for project {ProjectId}",
                                project.Id);
                            // Continue processing other projects
                        }
                    }

                    _logger.LogInformation("Successfully sent {Count} completion reminder notifications", notificationsSent);
                }
                else
                {
                    _logger.LogInformation("No overdue InProgress projects found");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while sending project completion reminders");
                throw; // Rethrow for Hangfire to retry if needed
            }
        }
    }
}
