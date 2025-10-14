using MSP.Application.Abstracts;
using MSP.Domain.Entities;
using MSP.Infrastructure.Persistence.DBContext;
using Microsoft.EntityFrameworkCore;

namespace MSP.Infrastructure.Repositories
{
    public class MeetingRepository : IMeetingRepository
    {
        private readonly ApplicationDbContext _context;

        public MeetingRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Meeting> CreateAsync(Meeting meeting)
        {
            await _context.Meetings.AddAsync(meeting);
            await _context.SaveChangesAsync();
            return meeting;
        }

        public async Task<Meeting?> GetByIdAsync(Guid id)
        {
            return await _context.Meetings
                .Include(m => m.Attendees)
                .Include(m => m.CreatedBy)
                .FirstOrDefaultAsync(m => m.Id == id);
        }

        public async Task<List<Meeting>> GetByProjectIdAsync(Guid projectId)
        {
            return await _context.Meetings
                .Include(m => m.Attendees)
                .Include(m => m.CreatedBy)
                .Where(m => m.ProjectId == projectId)
                .OrderByDescending(m => m.StartTime)
                .ToListAsync();
        }

        public async Task<Meeting> UpdateAsync(Meeting meeting)
        {
            _context.Meetings.Update(meeting);
            await _context.SaveChangesAsync();
            return meeting;
        }

        public async Task<bool> PauseAsync(Guid id)
        {
            var meeting = await _context.Meetings.FindAsync(id);
            if (meeting == null) return false;

            meeting.Status = "Tạm dừng";
            meeting.UpdatedAt = DateTime.UtcNow;

            _context.Meetings.Update(meeting);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<List<User>> GetAttendeesAsync(List<Guid> attendeeIds)
        {
            return await _context.Users
                .Where(u => attendeeIds.Contains(u.Id))
                .ToListAsync();
        }
    }
}
