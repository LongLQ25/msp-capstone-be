using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using MSP.Application.Repositories;
using MSP.Shared.Enums;

namespace MSP.Application.Services.Implementations.ProjectTask
{
    /// <summary>
    /// Service to automatically check and update task status to OverDue using Hangfire
    /// </summary>
    public class TaskStatusCronJobService
    {
        private readonly IProjectTaskRepository _projectTaskRepository;
        private readonly ILogger<TaskStatusCronJobService> _logger;

        public TaskStatusCronJobService(
            IProjectTaskRepository projectTaskRepository,
            ILogger<TaskStatusCronJobService> logger)
        {
            _projectTaskRepository = projectTaskRepository;
            _logger = logger;
        }

        /// <summary>
        /// Check and update overdue tasks to OverDue status
        /// This method will be called by Hangfire Recurring Job
        /// </summary>
        public async Task UpdateOverdueTasksAsync()
        {
            try
            {
                _logger.LogInformation("Starting to check for overdue tasks at {Time}", DateTime.UtcNow);
                
                var now = DateTime.UtcNow;
                
                // Query overdue tasks directly from database
                var tasksToUpdate = await _projectTaskRepository.GetOverdueTasksAsync(
                    now,
                    TaskEnum.OverDue.ToString(),
                    TaskEnum.Completed.ToString());

                if (tasksToUpdate.Any())
                {
                    _logger.LogInformation("Found {Count} tasks that are overdue", tasksToUpdate.Count());
                    
                    foreach (var task in tasksToUpdate)
                    {
                        var oldStatus = task.Status;
                        task.Status = TaskEnum.OverDue.ToString();
                        task.UpdatedAt = now;
                        
                        await _projectTaskRepository.UpdateAsync(task);
                        
                        _logger.LogInformation(
                            "Updated task {TaskId} ('{TaskTitle}') from {OldStatus} to OverDue. EndDate was {EndDate}", 
                            task.Id, 
                            task.Title,
                            oldStatus,
                            task.EndDate);
                    }
                    
                    await _projectTaskRepository.SaveChangesAsync();
                    _logger.LogInformation("Successfully updated {Count} overdue tasks", tasksToUpdate.Count());
                }
                else
                {
                    _logger.LogInformation("No overdue tasks found");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating overdue tasks");
                throw; // Rethrow for Hangfire to retry if needed
            }
        }
    }
}
