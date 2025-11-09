using MSP.Application.Abstracts;
using MSP.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MSP.Application.Repositories
{
    public interface IMeetingRepository : IGenericRepository<Meeting, Guid>
    {
        Task<Meeting?> GetMeetingByIdAsync(Guid id);
        Task<IEnumerable<Meeting>> GetMeetingByProjectIdAsync(Guid projectId);
        Task<bool> CancelMeetingAsync(Guid id);
        Task<IEnumerable<User>> GetAttendeesAsync(IEnumerable<Guid> attendeeIds);
        
        /// <summary>
        /// Lấy danh sách meetings có status Scheduled và đã đến StartTime
        /// </summary>
        Task<IEnumerable<Meeting>> GetScheduledMeetingsToStartAsync(DateTime currentTime, string scheduledStatus);
        
        /// <summary>
        /// Lấy danh sách meetings có status Ongoing, không có EndTime và đã quá 1 giờ từ StartTime
        /// </summary>
        Task<IEnumerable<Meeting>> GetOngoingMeetingsToFinishAsync(DateTime currentTime, string ongoingStatus);
    }
}
