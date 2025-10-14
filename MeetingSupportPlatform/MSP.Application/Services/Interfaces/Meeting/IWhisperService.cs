using MSP.Application.Models.Responses.Meeting;

namespace MSP.Application.Services.Interfaces.Meeting
{
    public interface IWhisperService
    {
        Task<List<TranscriptionItem>> TranscribeVideoAsync(string videoPath);
    }
}
