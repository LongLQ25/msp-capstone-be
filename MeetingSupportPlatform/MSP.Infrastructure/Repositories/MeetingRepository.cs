using Microsoft.EntityFrameworkCore;
using MSP.Application.Repositories;
using MSP.Domain.Entities;
using MSP.Infrastructure.Persistence.DBContext;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MSP.Infrastructure.Repositories
{
    public class MeetingRepository(ApplicationDbContext context) : GenericRepository<Meeting, Guid>(context), IMeetingRepository
    {

        public async Task<IEnumerable<User>> GetAttendeesAsync(IEnumerable<Guid> attendeeIds)
        {
            return await _context.Users
                .Where(u => attendeeIds.Contains(u.Id))
                .ToListAsync();
        }

        public async Task<Meeting?> GetMeetingByIdAsync(Guid id)
        {
            return await _context.Meetings
                .Include(m => m.Attendees)
                .Include(m => m.CreatedBy)
                .Include(m => m.Project)
                .Include(m => m.Milestone)
                .FirstOrDefaultAsync(m => m.Id == id);
        }

        public async Task<IEnumerable<Meeting>> GetMeetingByProjectIdAsync(Guid projectId)
        {
            return await _context.Meetings
                .Include(m => m.Attendees)
                .Include(m => m.CreatedBy)
                .Include(m => m.Project)
                .Include(m => m.Milestone)
                .Where(m => m.ProjectId == projectId)
                .OrderBy(m => m.StartTime)
                .ToListAsync();
        }

        public async Task<bool> CancelMeetingAsync(Guid id)
        {
            var meeting = await _context.Meetings.FindAsync(id);
            if (meeting == null) return false;

            meeting.Status = MSP.Shared.Enums.MeetingEnum.Cancelled.ToString();
            meeting.UpdatedAt = DateTime.UtcNow;

            _context.Meetings.Update(meeting);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<IEnumerable<Meeting>> GetScheduledMeetingsToStartAsync(DateTime currentTime, string scheduledStatus)
        {
            // Query được optimize bằng cách filter trực tiếp ở database level
            // Chỉ lấy meetings có status Scheduled và đã đến StartTime
            return await _context.Meetings
                .Where(m =>
                    !m.IsDeleted &&
                    m.Status == scheduledStatus &&
                    m.StartTime <= currentTime)
                .ToListAsync();
        }

        public async Task<IEnumerable<Meeting>> GetOngoingMeetingsToFinishAsync(DateTime currentTime, string ongoingStatus)
        {
            // Query được optimize bằng cách filter trực tiếp ở database level
            // Chỉ lấy meetings có status Ongoing, không có EndTime và đã quá 1 giờ từ StartTime
            return await _context.Meetings
                .Where(m =>
                    !m.IsDeleted &&
                    m.Status == ongoingStatus &&
                    !m.EndTime.HasValue &&
                    m.StartTime.AddHours(1) <= currentTime)
                .ToListAsync();
        }

    }
}
