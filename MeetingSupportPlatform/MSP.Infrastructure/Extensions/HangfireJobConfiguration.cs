using Hangfire;
using Microsoft.AspNetCore.Builder;
using MSP.Application.Services.Implementations.Cleanup;
using MSP.Application.Services.Implementations.Meeting;
using MSP.Application.Services.Implementations.Project;
using MSP.Application.Services.Implementations.ProjectTask;
using MSP.Application.Services.Implementations.SubscriptionService;

namespace MSP.Infrastructure.Extensions
{
    /// <summary>
    /// Extension methods to configure Hangfire Recurring Jobs
    /// </summary>
    public static class HangfireJobConfiguration
    {
        /// <summary>
        /// Configure all Hangfire Recurring Jobs for the application
        /// </summary>
        /// <param name="app">IApplicationBuilder instance</param>
        /// <returns>IApplicationBuilder for method chaining</returns>
        public static IApplicationBuilder UseHangfireJobs(this IApplicationBuilder app)
        {
            // Get Vietnam timezone (UTC+7)
            var vietnamTimeZone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");

            // 1. Task Status Cron Job
            // Automatically check and update overdue tasks to OverDue status + send notifications
            RecurringJob.AddOrUpdate<TaskStatusCronJobService>(
                "update-overdue-tasks",
                service => service.UpdateOverdueTasksAsync(),
                Cron.Daily(8), // Run every day at 8:00 AM Vietnam time (start of work day)
                new RecurringJobOptions
                {
                    TimeZone = vietnamTimeZone
                });

            // 2. Meeting Status Cron Job
            // Automatically update meeting statuses
            // - Scheduled ? Ongoing when StartTime is reached
            // - Ongoing ? Finished when 1 hour has passed from StartTime
            RecurringJob.AddOrUpdate<MeetingStatusCronJobService>(
                "update-meeting-statuses",
                service => service.UpdateMeetingStatusesAsync(),
                "*/5 * * * *", // Run every 5 minutes
                new RecurringJobOptions
                {
                    TimeZone = vietnamTimeZone
                });

            // 3. Project Status Cron Job
            // Automatically update project statuses
            // - NotStarted to InProgress when StartDate is reached
            // - Send deadline warnings to Owner and Members (7 days before EndDate)
            RecurringJob.AddOrUpdate<ProjectStatusCronJobService>(
                "update-project-statuses",
                service => service.UpdateProjectStatusesAsync(),
                Cron.Daily(7), // Run every day at 7:00 AM Vietnam time (before work starts)
                new RecurringJobOptions
                {
                    TimeZone = vietnamTimeZone
                });

            // 4. Cleanup Expired Refresh Tokens
            // Automatically cleanup expired refresh tokens for security and reduce database bloat
            RecurringJob.AddOrUpdate<CleanupExpiredTokensCronJobService>(
                "cleanup-expired-tokens",
                service => service.CleanupExpiredTokensAsync(),
                Cron.Daily(2), // Run daily at 2:00 AM Vietnam time (low traffic period)
                new RecurringJobOptions
                {
                    TimeZone = vietnamTimeZone
                });

            // 5. Cleanup Expired Pending Invitations
            // Automatically cancel/expire pending organization invitations after 7 days
            RecurringJob.AddOrUpdate<CleanupPendingInvitationsCronJobService>(
                "cleanup-pending-invitations",
                service => service.CleanupExpiredInvitationsAsync(),
                Cron.Daily(3), // Run daily at 3:00 AM Vietnam time (low traffic period)
                new RecurringJobOptions
                {
                    TimeZone = vietnamTimeZone
                });

            // 6. Task Deadline Reminder
            // Send reminder notifications for tasks that are due in 1-2 days
            RecurringJob.AddOrUpdate<TaskReminderCronJobService>(
                "send-task-deadline-reminders",
                service => service.SendDeadlineRemindersAsync(),
                Cron.Daily(9), // Run daily at 9:00 AM Vietnam time (users have full day to handle)
                new RecurringJobOptions
                {
                    TimeZone = vietnamTimeZone
                });

            // 7. Project Completion Reminder
            // Send reminder notifications for projects that are past EndDate but still InProgress
            RecurringJob.AddOrUpdate<ProjectCompletionReminderCronJobService>(
                "send-project-completion-reminders",
                service => service.SendCompletionRemindersAsync(),
                Cron.Daily(10), // Run daily at 10:00 AM Vietnam time (PMs are active at work)
                new RecurringJobOptions
                {
                    TimeZone = vietnamTimeZone
                });

            // 8. Meeting Reminder
            // Send reminder notifications for meetings starting in 1 hour
            RecurringJob.AddOrUpdate<MeetingReminderCronJobService>(
                "send-meeting-reminders",
                service => service.SendMeetingRemindersAsync(),
                "*/10 * * * *", // Run every 10 minutes
                new RecurringJobOptions
                {
                    TimeZone = vietnamTimeZone
                });

            return app;
        }

        /// <summary>
        /// Configure Recurring Jobs with custom options
        /// </summary>
        /// <param name="app">IApplicationBuilder instance</param>
        /// <param name="taskStatusCronExpression">Cron expression for task status job</param>
        /// <param name="meetingStatusCronExpression">Cron expression for meeting status job</param>
        /// <returns>IApplicationBuilder for method chaining</returns>
        public static IApplicationBuilder UseHangfireJobs(
            this IApplicationBuilder app, 
            string meetingStatusCronExpression = "*/5 * * * *")
        {
            // Get Vietnam timezone (UTC+7)
            var vietnamTimeZone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");

            RecurringJob.AddOrUpdate<TaskStatusCronJobService>(
                "update-overdue-tasks",
                service => service.UpdateOverdueTasksAsync(),
                Cron.Daily(8), // 8:00 AM - start of work day
                new RecurringJobOptions
                {
                    TimeZone = vietnamTimeZone
                });

            RecurringJob.AddOrUpdate<MeetingStatusCronJobService>(
                "update-meeting-statuses",
                service => service.UpdateMeetingStatusesAsync(),
                meetingStatusCronExpression,
                new RecurringJobOptions
                {
                    TimeZone = vietnamTimeZone
                });

            RecurringJob.AddOrUpdate<ProjectStatusCronJobService>(
                "update-project-statuses",
                service => service.UpdateProjectStatusesAsync(),
                Cron.Daily(7), // 7:00 AM - before work starts
                new RecurringJobOptions
                {
                    TimeZone = vietnamTimeZone
                });

            RecurringJob.AddOrUpdate<CleanupExpiredTokensCronJobService>(
                "cleanup-expired-tokens",
                service => service.CleanupExpiredTokensAsync(),
                Cron.Daily(2), // 2:00 AM - low traffic period
                new RecurringJobOptions
                {
                    TimeZone = vietnamTimeZone
                });

            RecurringJob.AddOrUpdate<CleanupPendingInvitationsCronJobService>(
                "cleanup-pending-invitations",
                service => service.CleanupExpiredInvitationsAsync(),
                Cron.Daily(3), // 3:00 AM - low traffic period
                new RecurringJobOptions
                {
                    TimeZone = vietnamTimeZone
                });

            RecurringJob.AddOrUpdate<TaskReminderCronJobService>(
                "send-task-deadline-reminders",
                service => service.SendDeadlineRemindersAsync(),
                Cron.Daily(9), // 9:00 AM - users have full day to handle
                new RecurringJobOptions
                {
                    TimeZone = vietnamTimeZone
                });

            RecurringJob.AddOrUpdate<ProjectCompletionReminderCronJobService>(
                "send-project-completion-reminders",
                service => service.SendCompletionRemindersAsync(),
                Cron.Daily(10), // 10:00 AM - PMs are active at work
                new RecurringJobOptions
                {
                    TimeZone = vietnamTimeZone
                });

            RecurringJob.AddOrUpdate<SubscriptionStatusCronJobService>(
                "expire-subscriptions",
                service => service.ExpireSubscriptionsAsync(),
                Cron.Daily(1), // Run daily at 1:00 AM - low traffic period
                new RecurringJobOptions
                {
                    TimeZone = vietnamTimeZone
                });

            RecurringJob.AddOrUpdate<MeetingReminderCronJobService>(
                "send-meeting-reminders",
                service => service.SendMeetingRemindersAsync(),
                "*/10 * * * *", // Run every 10 minutes
                new RecurringJobOptions
                {
                    TimeZone = vietnamTimeZone
                });

            return app;
        }
    }
}
