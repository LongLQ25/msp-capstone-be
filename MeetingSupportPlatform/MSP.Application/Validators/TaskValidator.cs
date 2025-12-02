using MSP.Shared.Enums;

namespace MSP.Application.Validators
{
    public static class TaskValidator
    {
        /// <summary>
        /// Validates if status transition is allowed
        /// </summary>
        public static bool IsValidStatusTransition(string currentStatus, string newStatus)
        {
            // Same status is always valid
            if (currentStatus == newStatus)
                return true;

            // Define forbidden transitions
            var forbiddenTransitions = new Dictionary<string, HashSet<string>>
            {
                // From Todo
                [TaskEnum.Todo.ToString()] = new HashSet<string>
                {
                    TaskEnum.ReadyToReview.ToString(),
                    TaskEnum.Done.ToString(),
                    TaskEnum.ReOpened.ToString()
                },
                
                // From InProgress
                [TaskEnum.InProgress.ToString()] = new HashSet<string>
                {
                    TaskEnum.Done.ToString(),
                    TaskEnum.ReOpened.ToString()
                },
                
                // From ReadyToReview
                [TaskEnum.ReadyToReview.ToString()] = new HashSet<string>
                {
                    TaskEnum.Todo.ToString(),
                    // TaskEnum.InProgress.ToString() - Commented: PM can request minor fixes
                },
                
                // From Reopened
                [TaskEnum.ReOpened.ToString()] = new HashSet<string>
                {
                    TaskEnum.ReadyToReview.ToString(),
                    TaskEnum.Done.ToString(),
                    TaskEnum.Todo.ToString()
                }
            };

            if (forbiddenTransitions.TryGetValue(currentStatus, out var forbiddenTargets))
            {
                return !forbiddenTargets.Contains(newStatus);
            }

            // If not in forbidden list, allow transition
            return true;
        }

        /// <summary>
        /// Get error message for invalid status transition
        /// </summary>
        public static string GetStatusTransitionError(string currentStatus, string newStatus)
        {
            return $"Cannot transition from '{currentStatus}' to '{newStatus}'. Invalid status flow.";
        }

        /// <summary>
        /// Validates task dates
        /// </summary>
        public static (bool isValid, string errorMessage) ValidateTaskDates(
            DateTime? taskStartDate,
            DateTime? taskEndDate,
            DateTime? projectStartDate,
            DateTime? projectEndDate,
            DateTime currentDate)
        {
            // Task dates are optional, skip validation if not provided
            if (!taskStartDate.HasValue || !taskEndDate.HasValue)
                return (true, string.Empty);

            // 1. Task.EndDate >= Task.StartDate
            if (taskEndDate.Value < taskStartDate.Value)
            {
                return (false, "Task end date must be greater than or equal to start date.");
            }

            // 2. Task creation date <= current date (StartDate can be in future for planning)
            // This is implicitly handled by CreatedAt field

            // 3. Task.StartDate >= Project.StartDate (if project has start date)
            if (projectStartDate.HasValue && taskStartDate.Value < projectStartDate.Value)
            {
                return (false, $"Task start date must be on or after project start date ({projectStartDate.Value:dd/MM/yyyy}).");
            }

            // 4. Task.EndDate <= Project.EndDate (if project has end date)
            if (projectEndDate.HasValue && taskEndDate.Value > projectEndDate.Value)
            {
                return (false, $"Task end date must be on or before project end date ({projectEndDate.Value:dd/MM/yyyy}).");
            }

            return (true, string.Empty);
        }

        /// <summary>
        /// Check if user has permission to change assignee
        /// </summary>
        public static bool CanChangeAssignee(string userRole)
        {
            // Only ProjectManager and Admin can change assignee
            return userRole == "ProjectManager" || userRole == "Admin";
        }

        /// <summary>
        /// Get error message for invalid assignee change
        /// </summary>
        public static string GetAssigneeChangeError()
        {
            return "Only Project Managers can change task assignee.";
        }
    }
}
