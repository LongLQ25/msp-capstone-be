using MSP.Domain.Entities;

namespace MSP.Application.Abstracts
{
    public interface IMeetingRepository
    {
        Task<Meeting> CreateAsync(Meeting meeting);
        Task<Meeting?> GetByIdAsync(Guid id);
        Task<List<Meeting>> GetByProjectIdAsync(Guid projectId);
        Task<Meeting> UpdateAsync(Meeting meeting);
        Task<bool> PauseAsync(Guid id);
        Task<List<User>> GetAttendeesAsync(List<Guid> attendeeIds);
    }
}
