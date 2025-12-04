using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using MSP.Application.Models.Requests.Notification;
using MSP.Application.Repositories;
using MSP.Application.Services.Interfaces.Notification;
using MSP.Domain.Entities;
using MSP.Shared.Enums;

namespace MSP.Application.Services.Implementations.Project
{
    /// <summary>
    /// Service to send completion reminder notifications for overdue projects using Hangfire
    /// Sends notifications to all Project Managers in the project
    /// </summary>
    public class ProjectCompletionReminderCronJobService
    {
        private readonly IProjectRepository _projectRepository;
        private readonly INotificationService _notificationService;
        private readonly UserManager<User> _userManager;
        private readonly ILogger<ProjectCompletionReminderCronJobService> _logger;

        public ProjectCompletionReminderCronJobService(
            IProjectRepository projectRepository,
            INotificationService notificationService,
            UserManager<User> userManager,
            ILogger<ProjectCompletionReminderCronJobService> logger)
        {
            _projectRepository = projectRepository;
            _notificationService = notificationService;
            _userManager = userManager;
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

                            // Get all Project Managers from ProjectMembers
                            var projectManagers = new List<(Guid UserId, User User)>();
                            if (project.ProjectMembers?.Any() == true)
                            {
                                var activeMembers = project.ProjectMembers
                                    .Where(pm => !pm.LeftAt.HasValue)
                                    .ToList();

                                foreach (var projectMember in activeMembers)
                                {
                                    var roles = await _userManager.GetRolesAsync(projectMember.Member);
                                    if (roles.Contains(UserRoleEnum.ProjectManager.ToString()))
                                    {
                                        projectManagers.Add((projectMember.MemberId, projectMember.Member));
                                    }
                                }
                            }

                            if (!projectManagers.Any())
                            {
                                _logger.LogWarning(
                                    "No Project Managers found for project {ProjectId}. Skipping notification.",
                                    project.Id);
                                continue;
                            }

                            _logger.LogInformation(
                                "Found {Count} Project Manager(s) for project {ProjectId}",
                                projectManagers.Count,
                                project.Id);

                            // Send notifications to all Project Managers
                            foreach (var (pmUserId, pmUser) in projectManagers)
                            {
                                try
                                {
                                    // Send in-app notification
                                    var notificationRequest = new CreateNotificationRequest
                                    {
                                        UserId = pmUserId,
                                        Title = "Project Completion Reminder",
                                        Message = $"Project '{project.Name}' is overdue by {daysOverdue} day(s) (Deadline: {project.EndDate:dd/MM/yyyy}). Please review and mark as completed if finished.",
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

                                    // Send email notification
                                    if (pmUser != null && !string.IsNullOrEmpty(pmUser.Email))
                                    {
                                        _notificationService.SendEmailNotification(
                                            pmUser.Email,
                                            "Project Completion Reminder",
                                            $"Hello {pmUser.FullName},<br/><br/>" +
                                            $"Project <strong>{project.Name}</strong> has exceeded its expected end date by {daysOverdue} day(s).<br/><br/>" +
                                            $"<strong>Expected End Date:</strong> {project.EndDate:dd/MM/yyyy}<br/>" +
                                            $"<strong>Current Date:</strong> {now:dd/MM/yyyy}<br/>" +
                                            $"<strong>Days Overdue:</strong> {daysOverdue} day(s)<br/>" +
                                            $"<strong>Current Status:</strong> In Progress<br/><br/>" +
                                            $"If the project is completed, please update its status to 'Completed' in the system.<br/>" +
                                            $"If the project needs more time, please consider adjusting the end date.");
                                    }

                                    notificationsSent++;

                                    _logger.LogInformation(
                                        "Sent completion reminder for project {ProjectId} to PM {UserId}. Overdue by {Days} days",
                                        project.Id,
                                        pmUserId,
                                        daysOverdue);
                                }
                                catch (Exception innerEx)
                                {
                                    _logger.LogError(innerEx,
                                        "Failed to send completion reminder to PM {UserId} for project {ProjectId}",
                                        pmUserId,
                                        project.Id);
                                    // Continue with other PMs
                                }
                            }
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
