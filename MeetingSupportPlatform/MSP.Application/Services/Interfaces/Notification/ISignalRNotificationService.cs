using MSP.Application.Models.Responses.Notification;

namespace MSP.Application.Services.Interfaces.Notification
{
    /// <summary>
    /// Interface for real-time notification broadcasting via SignalR
    /// </summary>
    public interface ISignalRNotificationService
    {
        /// <summary>
        /// Send notification to a specific user via SignalR
        /// </summary>
        Task SendNotificationToUserAsync(Guid userId, NotificationResponse notification);

        /// <summary>
        /// Send notification to multiple users via SignalR
        /// </summary>
        Task SendNotificationToUsersAsync(IEnumerable<Guid> userIds, NotificationResponse notification);

        /// <summary>
        /// Send notification to a group (e.g., project members) via SignalR
        /// </summary>
        Task SendNotificationToGroupAsync(string groupName, NotificationResponse notification);

        /// <summary>
        /// Broadcast notification to all connected users via SignalR
        /// </summary>
        Task BroadcastNotificationAsync(NotificationResponse notification);

        /// <summary>
        /// Notify user about unread count update
        /// </summary>
        Task UpdateUnreadCountAsync(Guid userId, int unreadCount);
    }
}
