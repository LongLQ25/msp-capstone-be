using Microsoft.Extensions.Logging;
using MSP.Application.Models.Requests.Notification;
using MSP.Application.Repositories;
using MSP.Application.Services.Interfaces.Notification;
using MSP.Shared.Enums;

namespace MSP.Application.Services.Implementations.Project
{
    /// <summary>
    /// Service to automatically update project statuses using Hangfire
    /// - Scheduled ? InProgress when StartDate is reached
    /// - Send notifications to Owner and Members when project is nearing deadline
    /// </summary>
    public class ProjectStatusCronJobService
    {
        private readonly IProjectRepository _projectRepository;
        private readonly INotificationService _notificationService;
        private readonly ILogger<ProjectStatusCronJobService> _logger;

        public ProjectStatusCronJobService(
            IProjectRepository projectRepository,
            INotificationService notificationService,
            ILogger<ProjectStatusCronJobService> logger)
        {
            _projectRepository = projectRepository;
            _notificationService = notificationService;
            _logger = logger;
        }

        /// <summary>
        /// Update project statuses and send deadline warnings
        /// This method will be called by Hangfire Recurring Job
        /// </summary>
        public async Task UpdateProjectStatusesAsync()
        {
            try
            {
                _logger.LogInformation("Starting to update project statuses at {Time}", DateTime.UtcNow);

                var now = DateTime.UtcNow;
                int updatedCount = 0;
                int notificationsSent = 0;

                // 1. Update projects from Scheduled ? InProgress
                var scheduledProjects = await _projectRepository.GetScheduledProjectsToStartAsync(
                    now,
                    ProjectStatusEnum.Scheduled.ToString());

                if (scheduledProjects.Any())
                {
                    _logger.LogInformation("Found {Count} scheduled projects to start", scheduledProjects.Count());

                    foreach (var project in scheduledProjects)
                    {
                        project.Status = ProjectStatusEnum.InProgress.ToString();
                        project.UpdatedAt = now;
                        await _projectRepository.UpdateAsync(project);

                        _logger.LogInformation(
                            "Updated project {ProjectId} ('{Name}') from Scheduled to InProgress. StartDate was {StartDate}",
                            project.Id,
                            project.Name,
                            project.StartDate);

                        updatedCount++;
                    }

                    await _projectRepository.SaveChangesAsync();
                }

                // 2. Send alerts for projects nearing deadline (7 days before EndDate)
                var projectsNearingDeadline = await _projectRepository.GetProjectsNearingDeadlineAsync(
                    now,
                    now.AddDays(7),
                    ProjectStatusEnum.InProgress.ToString());

                if (projectsNearingDeadline.Any())
                {
                    _logger.LogWarning("Found {Count} projects nearing deadline (within 7 days)", projectsNearingDeadline.Count());

                    foreach (var project in projectsNearingDeadline)
                    {
                        var daysRemaining = (project.EndDate!.Value - now).Days;
                        var deadlineText = daysRemaining == 0 
                            ? "today" 
                            : $"in {daysRemaining} day{(daysRemaining > 1 ? "s" : "")}";

                        _logger.LogWarning(
                            "Project {ProjectId} ('{Name}') is nearing deadline. EndDate: {EndDate} ({DeadlineText})",
                            project.Id,
                            project.Name,
                            project.EndDate,
                            deadlineText);

                        // 2.1. Send notification to Project Owner
                        try
                        {
                            await _notificationService.CreateInAppNotificationAsync(new CreateNotificationRequest
                            {
                                UserId = project.OwnerId,
                                Title = "?? Project Deadline Warning",
                                Message = $"Project '{project.Name}' is approaching its deadline {deadlineText} (End Date: {project.EndDate:yyyy-MM-dd}). Please review the project progress.",
                                Type = NotificationTypeEnum.InApp.ToString(),
                                Data = $"{{\"eventType\":\"ProjectDeadlineWarning\",\"projectId\":\"{project.Id}\",\"projectName\":\"{project.Name}\",\"endDate\":\"{project.EndDate:yyyy-MM-dd}\",\"daysRemaining\":{daysRemaining}}}"
                            });

                            notificationsSent++;
                            
                            _logger.LogInformation(
                                "Sent deadline notification to Owner {OwnerId} for project {ProjectId}",
                                project.OwnerId,
                                project.Id);
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, 
                                "Failed to send notification to Owner {OwnerId} for project {ProjectId}",
                                project.OwnerId,
                                project.Id);
                        }

                        // 2.2. Send notifications to all active Project Members
                        if (project.ProjectMembers?.Any() == true)
                        {
                            var activeMembers = project.ProjectMembers
                                .Where(pm => !pm.LeftAt.HasValue) // Only active members
                                .ToList();

                            _logger.LogInformation(
                                "Sending deadline notifications to {Count} active members of project {ProjectId}",
                                activeMembers.Count,
                                project.Id);

                            foreach (var projectMember in activeMembers)
                            {
                                try
                                {
                                    await _notificationService.CreateInAppNotificationAsync(new CreateNotificationRequest
                                    {
                                        UserId = projectMember.MemberId,
                                        Title = "?? Project Deadline Warning",
                                        Message = $"Project '{project.Name}' is approaching its deadline {deadlineText} (End Date: {project.EndDate:yyyy-MM-dd}). Please complete your tasks on time.",
                                        Type = NotificationTypeEnum.InApp.ToString(),
                                        Data = $"{{\"eventType\":\"ProjectDeadlineWarning\",\"projectId\":\"{project.Id}\",\"projectName\":\"{project.Name}\",\"endDate\":\"{project.EndDate:yyyy-MM-dd}\",\"daysRemaining\":{daysRemaining}}}"
                                    });

                                    notificationsSent++;
                                }
                                catch (Exception ex)
                                {
                                    _logger.LogError(ex,
                                        "Failed to send notification to Member {MemberId} for project {ProjectId}",
                                        projectMember.MemberId,
                                        project.Id);
                                }
                            }
                        }
                    }
                }

                // Summary log
                if (updatedCount > 0 || notificationsSent > 0)
                {
                    _logger.LogInformation(
                        "Job completed: Updated {UpdatedCount} projects, sent {NotificationCount} deadline notifications",
                        updatedCount,
                        notificationsSent);
                }
                else
                {
                    _logger.LogInformation("No projects to update and no deadline warnings to send");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating project statuses");
                throw; // Rethrow for Hangfire to retry if needed
            }
        }
    }
}
