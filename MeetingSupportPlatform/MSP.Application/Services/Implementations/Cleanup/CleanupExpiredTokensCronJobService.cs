using Microsoft.Extensions.Logging;
using MSP.Application.Abstracts;

namespace MSP.Application.Services.Implementations.Cleanup
{
    /// <summary>
    /// Service ?? t? ??ng cleanup expired refresh tokens s? d?ng Hangfire
    /// Giúp b?o m?t và gi?m database bloat
    /// </summary>
    public class CleanupExpiredTokensCronJobService
    {
        private readonly IUserRepository _userRepository;
        private readonly ILogger<CleanupExpiredTokensCronJobService> _logger;

        public CleanupExpiredTokensCronJobService(
            IUserRepository userRepository,
            ILogger<CleanupExpiredTokensCronJobService> logger)
        {
            _userRepository = userRepository;
            _logger = logger;
        }

        /// <summary>
        /// Cleanup expired refresh tokens
        /// Method này s? ???c g?i b?i Hangfire Recurring Job
        /// </summary>
        public async Task CleanupExpiredTokensAsync()
        {
            try
            {
                _logger.LogInformation("Starting to cleanup expired refresh tokens at {Time}", DateTime.UtcNow);

                var now = DateTime.UtcNow;

                // L?y users có expired refresh tokens
                var usersWithExpiredTokens = await _userRepository.GetUsersWithExpiredRefreshTokensAsync(now);

                if (usersWithExpiredTokens.Any())
                {
                    _logger.LogInformation("Found {Count} users with expired refresh tokens", usersWithExpiredTokens.Count());

                    foreach (var user in usersWithExpiredTokens)
                    {
                        _logger.LogInformation(
                            "Cleaning expired refresh token for user {UserId} ({Email}). Expired at {ExpiredAt}",
                            user.Id,
                            user.Email,
                            user.RefreshTokenExpiresAtUtc);

                        // Clear refresh token
                        user.RefreshToken = null;
                        user.RefreshTokenExpiresAtUtc = null;

                        await _userRepository.UpdateAsync(user);
                    }

                    await _userRepository.SaveChangesAsync();
                    _logger.LogInformation("Successfully cleaned {Count} expired refresh tokens", usersWithExpiredTokens.Count());
                }
                else
                {
                    _logger.LogInformation("No expired refresh tokens found");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while cleaning up expired refresh tokens");
                throw; // Rethrow ?? Hangfire có th? retry n?u c?n
            }
        }
    }
}
