using Hangfire;
using Microsoft.AspNetCore.Builder;
using MSP.Application.Services.Implementations.ProjectTask;
using MSP.Application.Services.Implementations.Meeting;

namespace MSP.Application.Extensions
{
    /// <summary>
    /// Extension methods ?? c?u hình Hangfire Recurring Jobs
    /// </summary>
    public static class HangfireJobConfiguration
    {
        /// <summary>
        /// C?u hình t?t c? Hangfire Recurring Jobs cho ?ng d?ng
        /// </summary>
        /// <param name="app">IApplicationBuilder instance</param>
        /// <returns>IApplicationBuilder ?? chain ti?p</returns>
        public static IApplicationBuilder UseHangfireJobs(this IApplicationBuilder app)
        {
            // 1. Task Status Cron Job
            // T? ??ng ki?m tra và c?p nh?t task quá h?n thành OverDue
            RecurringJob.AddOrUpdate<TaskStatusCronJobService>(
                "update-overdue-tasks",
                service => service.UpdateOverdueTasksAsync(),
                "*/5 * * * *", // Ch?y m?i 5 phút
                new RecurringJobOptions
                {
                    TimeZone = TimeZoneInfo.Utc
                });

            // 2. Meeting Status Cron Job
            // T? ??ng c?p nh?t status c?a meetings
            // - Scheduled ? Ongoing khi ??n StartTime
            // - Ongoing ? Finished khi ?ã quá 1 gi? t? StartTime
            RecurringJob.AddOrUpdate<MeetingStatusCronJobService>(
                "update-meeting-statuses",
                service => service.UpdateMeetingStatusesAsync(),
                "* * * * *", // Ch?y m?i phút
                new RecurringJobOptions
                {
                    TimeZone = TimeZoneInfo.Utc
                });

            // TODO: Thêm các recurring jobs khác ? ?ây n?u c?n
            // Example:
            // RecurringJob.AddOrUpdate<AnotherService>(
            //     "another-job",
            //     service => service.DoSomethingAsync(),
            //     Cron.Daily, // Ho?c custom cron expression
            //     new RecurringJobOptions { TimeZone = TimeZoneInfo.Utc });

            return app;
        }

        /// <summary>
        /// C?u hình Recurring Jobs v?i custom options
        /// </summary>
        /// <param name="app">IApplicationBuilder instance</param>
        /// <param name="taskStatusCronExpression">Cron expression cho task status job</param>
        /// <param name="meetingStatusCronExpression">Cron expression cho meeting status job</param>
        /// <returns>IApplicationBuilder ?? chain ti?p</returns>
        public static IApplicationBuilder UseHangfireJobs(
            this IApplicationBuilder app, 
            string taskStatusCronExpression = "*/5 * * * *",
            string meetingStatusCronExpression = "* * * * *")
        {
            RecurringJob.AddOrUpdate<TaskStatusCronJobService>(
                "update-overdue-tasks",
                service => service.UpdateOverdueTasksAsync(),
                taskStatusCronExpression,
                new RecurringJobOptions
                {
                    TimeZone = TimeZoneInfo.Utc
                });

            RecurringJob.AddOrUpdate<MeetingStatusCronJobService>(
                "update-meeting-statuses",
                service => service.UpdateMeetingStatusesAsync(),
                meetingStatusCronExpression,
                new RecurringJobOptions
                {
                    TimeZone = TimeZoneInfo.Utc
                });

            return app;
        }
    }
}
