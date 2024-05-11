using VideoMetaService.Interfaces;
using VideoMetaService.Services;

namespace VideoMetaService.Extensions
{
    public static class ServiceExtension
    {
        public static IServiceCollection AddService(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<IVideoMetadataService, VideoMetadataService>();
            services.AddSingleton<IConfiguration>(provider => configuration);
            return services;
        }
    }
}
