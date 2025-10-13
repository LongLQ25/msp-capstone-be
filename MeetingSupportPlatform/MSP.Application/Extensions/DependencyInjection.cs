using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MSP.Application.Services.Interfaces.Auth;
using MSP.Application.Services.Implementations.Auth;
using MSP.Application.Services.Interfaces.Notification;
using MSP.Application.Services.Implementations.Meeting;
using MSP.Application.Services.Interfaces.Meeting;
using MSP.Application.Services.Implementations.Summarize;
using MSP.Application.Services.Interfaces.Summarize;
using MSP.Application.Services.Interfaces.Project;
using MSP.Application.Services.Implementations.Project;

namespace MSP.Application.Extensions
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplicationService(this IServiceCollection services, IConfiguration configuration)
        {
            // Create DI
            services.AddScoped<IAccountService, AccountService>();
            services.AddScoped<INotificationService, MSP.Application.Services.Implementations.Notification.NotificationService>();
            services.AddScoped<ISummarizeTextService, SummarizeTextService>();
            services.AddScoped<IProjectService, ProjectService>();

            // Đăng ký StreamSettings từ appsettings.json
            services.Configure<StreamSettings>(
                configuration.GetSection("Stream"));

            // Đăng ký HttpClientFactory
            services.AddHttpClient();

            // Đăng ký StreamService với DI
            services.AddScoped<IStreamService, StreamService>();
            services.AddScoped<IWhisperService, WhisperService>();

            return services;
        }
    }
}
